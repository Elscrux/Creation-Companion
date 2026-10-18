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

    public IReadOnlyList<IConfigOption> Options { get; } = [
        AudioExtensionOption,
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
        var audioExtension = config.TryGet(CustomFuzPackagerRuntimeRuntimeConfig.AudioExtensionOption)
         ?? CustomFuzPackagerRuntimeRuntimeConfig.AudioExtensionOption.DefaultValue;

        fileSystem.Directory.EnumerateFiles(dir, "*" + audioExtension, SearchOption.AllDirectories)
            .AsParallel()
            .WithCancellation(token)
            .ForAll(PackageFuz);

        return Task.FromResult(new IntegrationRunResult(true, "Fuz files packaged successfully"));
    }

    public Task<IntegrationRunResult> PackageFuz(DataSourceFileLink file, ConfigValues config, CancellationToken token) =>
        Task.FromResult(new IntegrationRunResult(true, "Fuz file packaged successfully"));

    public void PackageFuz(string audioFilePath) {
        var lipFile = fileSystem.Path.ChangeExtension(audioFilePath, LipFileExtension);
        if (!fileSystem.File.Exists(lipFile)) {
            logger.Here().Information("Lip file does not exist for audio file {AudioFilePath}, skipping fuz packaging", audioFilePath);
            return;
        }

        var fuzPath = fileSystem.Path.ChangeExtension(audioFilePath, FuzeFileExtension);

        using var fileStream = fileSystem.File.Create(fuzPath);

        // Write header
        fileStream.Write(FuzeMagicStringBytes, 0, FuzeMagicStringBytes.Length);
        fileStream.Write(FuzeVersion, 0, FuzeVersion.Length);

        // Write lip
        using var lipStream = fileSystem.File.OpenRead(lipFile);
        var bytes = BitConverter.GetBytes((uint) lipStream.Length);
        fileStream.Write(bytes);
        lipStream.CopyTo(fileStream);

        // Write audio
        using var audioStream = fileSystem.File.OpenRead(lipFile);
        audioStream.CopyTo(fileStream);
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

    public void ExtractFuz(string fuzFilePath, string audioExtension) {
        var fuzFileInfo = fileSystem.FileInfo.New(fuzFilePath);

        var minSize = FuzeVersion.Length + FuzeMagicStringBytes.Length + 4;
        if (fuzFileInfo.Length < minSize) return;

        using var fuzFile = fuzFileInfo.OpenRead();
        var fuzReader = new BinaryReader(fuzFile);

        // Read header - magic string and version
        Span<byte> header = stackalloc byte[8];
        fuzReader.ReadExactly(header);
        if (!header[..4].SequenceEqual(FuzeMagicStringBytes)) {
            logger.Here().Error(
                "Invalid fuz file {FuzFilePath}: magic string is {MagicString} but expected {ExpectedMagicString}",
                fuzFilePath,
                Encoding.UTF8.GetString(header[..4]),
                FuzeMagicString);
            return;
        }

        // Validate header
        var lipLength = fuzReader.ReadUInt32();
        var audioLength = fuzFileInfo.Length - (minSize + lipLength);

        if (audioLength <= 0) {
            logger.Here().Error("Invalid fuz file {FuzFilePath}: audio length is {AudioLength} but expected greater than 0", fuzFilePath, audioLength);
            return;
        }

        // Read lip
        if (lipLength != 0) {
            var lipFilePath = fileSystem.Path.ChangeExtension(fuzFilePath, LipFileExtension);
            var lipData = fuzReader.ReadBytes((int) lipLength);
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
