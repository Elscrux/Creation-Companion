using System.IO.Abstractions;
using System.Text;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using CreationEditor.Services.DataSource;
using Serilog;
namespace CreationEditor.Services.Integrations.Lip;

public sealed class CustomFuzPackagerRegistrationConfig : IConfigDefinition {
    public IReadOnlyList<IConfigOption> Options { get; } = [];
}

public sealed class CustomFuzPackagerRuntimeRuntimeConfig : IFuzGeneratorRuntimeConfigDefinition {
    public static TextConfigOption AudioExtensionOption { get; } = new(
        "Audio Extension",
        "The audio file extension to package into fuz files",
        ".xwm",
        true);

    public static BoolConfigOption OnlyGenerateIfLipExistsOption { get; } = new(
        "Only Generate If Lip Exists",
        "Fuz files will only be generated if a corresponding lip file exists",
        true);

    public IReadOnlyList<IConfigOption> Options { get; } = [
        AudioExtensionOption,
        OnlyGenerateIfLipExistsOption,
    ];
}

public sealed class NativeLipFuzer(IFileSystem fileSystem, ILogger logger) : ILipFuzer {
    private const string FuzeMagicString = "FUZE";
    private static readonly byte[] FuzeVersion = BitConverter.GetBytes(1u);
    private static readonly byte[] FuzeMagicStringBytes = new UTF8Encoding(true).GetBytes(FuzeMagicString);

    public static IntegrationDescriptor StaticDescriptor { get; } = new(
        new Guid("2ac18313-a8e5-459c-bd9c-c55d2f559db4"),
        "Native Lip Fuzer",
        "A native implementation for packing lip and audio files into fuz files.",
        "Lip Fuzer",
        typeof(ILipFuzer),
        new CustomFuzPackagerRegistrationConfig(),
        new CustomFuzPackagerRuntimeRuntimeConfig(),
        []
    );

    public IntegrationDescriptor Descriptor => StaticDescriptor;

    public string LipFileExtension => ".lip";
    public string FuzeFileExtension => ".fuz";

    public Task<IntegrationRunResult> PackageFuz(string dir, ConfigValues config, CancellationToken token) {
        var onlyGenerateIfLipExists = config.Get(CustomFuzPackagerRuntimeRuntimeConfig.OnlyGenerateIfLipExistsOption);
        var audioExtension = config.TryGet(CustomFuzPackagerRuntimeRuntimeConfig.AudioExtensionOption)
         ?? CustomFuzPackagerRuntimeRuntimeConfig.AudioExtensionOption.DefaultValue;

        fileSystem.Directory.EnumerateFiles(dir, "*" + audioExtension, SearchOption.AllDirectories)
            .AsParallel()
            .WithCancellation(token)
            .ForAll(audioFilePath => PackageFuz(audioFilePath, onlyGenerateIfLipExists));

        return Task.FromResult(new IntegrationRunResult(true, "Fuz files packaged successfully"));
    }

    public Task<IntegrationRunResult> PackageFuz(DataSourceFileLink file, ConfigValues config, CancellationToken token) {
        var onlyGenerateIfLipExists = config.Get(CustomFuzPackagerRuntimeRuntimeConfig.OnlyGenerateIfLipExistsOption);
        PackageFuz(file.FullPath, onlyGenerateIfLipExists);

        return Task.FromResult(new IntegrationRunResult(true, "Fuz file packaged successfully"));
    }

    private void PackageFuz(string audioFilePath, bool onlyGenerateIfLipExists) {
        var lipFile = fileSystem.Path.ChangeExtension(audioFilePath, LipFileExtension);
        if (!fileSystem.File.Exists(lipFile)) {
            if (onlyGenerateIfLipExists) return;

            logger.Here().Warning("Lip file does not exist for audio file {AudioFilePath}", audioFilePath);
        }

        var fuzPath = fileSystem.Path.ChangeExtension(audioFilePath, FuzeFileExtension);

        using var fileStream = fileSystem.File.OpenWrite(fuzPath);

        // Write header
        fileStream.Write(FuzeMagicStringBytes, 0, FuzeMagicStringBytes.Length);
        fileStream.Write(FuzeVersion, 0, FuzeVersion.Length);

        // Write lip
        var bytes = BitConverter.GetBytes((uint) lipFile.Length);
        fileStream.Write(bytes, 0, bytes.Length);
        var lipBytes = fileSystem.File.ReadAllBytes(lipFile);
        fileStream.Write(lipBytes, 0, lipBytes.Length);

        // Write audio
        bytes = fileSystem.File.ReadAllBytes(audioFilePath);
        fileStream.Write(bytes, 0, bytes.Length);
    }

    public Task<IntegrationRunResult> ExtractFuz(string dir, ConfigValues config, CancellationToken token) {
        var audioExtension = config.TryGet(CustomFuzPackagerRuntimeRuntimeConfig.AudioExtensionOption)
         ?? CustomFuzPackagerRuntimeRuntimeConfig.AudioExtensionOption.DefaultValue;

        fileSystem.Directory.EnumerateFiles(dir, "*" + FuzeFileExtension, SearchOption.AllDirectories)
            .AsParallel()
            .WithCancellation(token)
            .ForAll(fuzFilePath => ExtractFuz(fuzFilePath, audioExtension));

        return Task.FromResult(new IntegrationRunResult(true, "Fuz files extracted successfully"));
    }

    public Task<IntegrationRunResult> ExtractFuz(DataSourceFileLink file, ConfigValues config, CancellationToken token) {
        var audioExtension = config.TryGet(CustomFuzPackagerRuntimeRuntimeConfig.AudioExtensionOption)
         ?? CustomFuzPackagerRuntimeRuntimeConfig.AudioExtensionOption.DefaultValue;

        ExtractFuz(file.FullPath, audioExtension);

        return Task.FromResult(new IntegrationRunResult(true, "Fuz file extracted successfully"));
    }

    private void ExtractFuz(string fuzFilePath, string audioExtension) {
        var fuzFileInfo = fileSystem.FileInfo.New(fuzFilePath);

        var minSize = FuzeVersion.Length + FuzeMagicStringBytes.Length + 4;
        if (fuzFileInfo.Length < minSize) return;

        using var fuzFile = fuzFileInfo.OpenRead();
        var fuzReader = new BinaryReader(fuzFile);

        // Read header
        var magicString = new string(fuzReader.ReadChars(FuzeMagicStringBytes.Length));
        fuzReader.ReadUInt32(); // Skip version

        // Validate header
        var lipLength = fuzReader.ReadUInt32();
        var audioLength = fuzFileInfo.Length - (minSize + lipLength);
        if (magicString != FuzeMagicString) {
            logger.Here().Error(
                "Invalid fuz file {FuzFilePath}: magic string is {MagicString} but expected {ExpectedMagicString}",
                fuzFilePath,
                magicString,
                FuzeMagicString);
            return;
        }

        if (audioLength <= 0) {
            logger.Here().Error("Invalid fuz file {FuzFilePath}: audio length is {AudioLength} but expected greater than 0", fuzFilePath, audioLength);
            return;
        }

        // Read lip
        var lipFilePath = fileSystem.Path.ChangeExtension(fuzFilePath, LipFileExtension);
        var lipData = fuzReader.ReadBytes((int) lipLength);
        if (lipLength != 0) {
            using var binaryWriter = new BinaryWriter(fileSystem.FileStream.New(lipFilePath, FileMode.Create));
            binaryWriter.Write(lipData);
        }

        // Read audio
        var audioData = fuzReader.ReadBytes((int) audioLength);
        var audioFilePath = fileSystem.Path.ChangeExtension(fuzFilePath, audioExtension);
        using var audioWriter = new BinaryWriter(fileSystem.FileStream.New(audioFilePath, FileMode.Create));
        audioWriter.Write(audioData);
    }
}
