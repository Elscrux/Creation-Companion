using System.IO.Abstractions;
using Autofac;
using CommandLine;
using CreationEditor;
using CreationEditor.Avalonia.Modules;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Plugin;
using CreationEditor.Skyrim.Avalonia.Modules;
using LipGenerator.Services;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Strings;
using Serilog;
namespace LipGenerator.Cli.Args;

[Verb("generate-lip", HelpText = "Generate lip and fuz files from a mod.")]
public record CommandLineEntryPointLipGenerator : ICommandLineEntryPoint, IDataSourceArguments {
    [Option('d',
        "Directory",
        Required = true,
        HelpText = "Directory of the mod to detect the wav files from")]
    public string ActiveDataSourcePath { get; set; } = null!;

    [Option("AdditionalDataSourcePaths",
        Required = false,
        HelpText = "Additional directories containing mod master files")]
    public IEnumerable<string> AdditionalDataSourcePaths { get; set; } = [];

    [Option("IncludeDataDirectoryDataSource",
        Required = false,
        HelpText = "Use the game's data directory as a data source")]
    public bool IncludeDataDirectoryDataSource { get; set; } = true;

    [Option('m',
        "ModFilename",
        Required = true,
        HelpText = "File name of mod to generate lip files for")]
    public string ModFilename { get; set; } = null!;

    [Option("DeleteWavAndLipFiles",
        Default = true,
        HelpText = "Delete wav and lip files after generating fuz files")]
    public bool DeleteWavAndLipFiles { get; set; } = true;

    [Option("LipGeneratorTool", Default = LipGeneratorTool.LipGenerator, HelpText = "Tool to use for lip generation (FaceFXWrapper or LipGenerator)")]
    public LipGeneratorTool LipGeneratorTool { get; set; } = LipGeneratorTool.LipGenerator;

    [Option("LipFuzerTool", Default = LipFuzerTool.LipFuzer, HelpText = "Tool to use for fuz generation (LipFuzer)")]
    public LipFuzerTool LipFuzerTool { get; set; } = LipFuzerTool.LipFuzer;

    [Option("AudioFormat", Default = AudioFormatOption.XWM, HelpText = "Audio format for output (XWM or WAV)")]
    public AudioFormatOption AudioFormat { get; set; } = AudioFormatOption.XWM;

    [Option("Language", Default = Language.English, HelpText = "Language for lip generation")]
    public Language Language { get; set; } = Language.English;

    [Option("LipGeneratorPath", HelpText = @"Path to LipGenerator.exe (required when using LipGenerator tool)")]
    public string LipGeneratorPath { get; set; } = null!;

    [Option("GestureExaggeration", Default = 1.0f, HelpText = "Gesture exaggeration factor (0.5-3.0)")]
    public float GestureExaggeration { get; set; } = 1.0f;

    [Option("LipAnimSpeed", Default = 1.0f, HelpText = "Lip animation speed (0.5-2.0)")]
    public float LipAnimSpeed { get; set; } = 1.0f;

    [Option("LipAnimDelay", Default = 0.0f, HelpText = "Lip animation delay")]
    public float LipAnimDelay { get; set; } = 0.0f;

    [Option("FaceFxWrapperPath", HelpText = @"Path to FaceFXWrapper.exe, usually found under Tools\Audio\FaceFXWrapper.exe")]
    public string FaceFxWrapperPath { get; set; } = null!;

    [Option("FonixDataPath", HelpText = @"Path to FonixData.cdf, usually found under Data\Sound\Voice\Processing\FonixData.cdf")]
    public string FonixDataPath { get; set; } = null!;

    [Option("LipFuzerPath", HelpText = @"Path to LIPFuzer.exe, usually found under Tools\Audio\LIPFuzer.exe")]
    public string LipFuzerPath { get; set; } = null!;

    [Option("XwmEncoderPath", HelpText = @"Path to xwmaencode.exe, usually found under Tools\Audio\xwmaencode.exe")]
    public string XwmEncoderPath { get; set; } = null!;

    [Option("XwmBitrate", Default = 192000, HelpText = "XWM encoding bitrate")]
    public int XwmBitrate { get; set; } = 192000;

    public IReadOnlySet<string> Verbs { get; } = (HashSet<string>) ["generate-lip"];

    public async Task<int> Run(string[] args) {
        return await Parser.Default.ParseArguments<CommandLineEntryPointLipGenerator>(args)
            .MapResult(async cmd => {
                    Log.Information("Starting lip generation with arguments: {@Arguments}", cmd);

                    await using var container = CommandLineContainerSetup.Setup(cmd,
                        cmd.ModFilename,
                        builder => {
                            builder.RegisterModule<EditorModule>();
                            builder.RegisterModule<SkyrimModule>();
                            builder.RegisterModule<LipGeneratorModule>();
                        });
                    if (container is null) return -1;

                    var lipGeneratorFactory = container.Resolve<LipFileGeneratorFactory>();
                    var gameDirectoryProvider = container.Resolve<IGameDirectoryProvider>();
                    var fileSystem = container.Resolve<IFileSystem>();

                    var gameDirectory = gameDirectoryProvider.Path;
                    if (gameDirectory is not null) {
                        cmd.FaceFxWrapperPath = GetIfExists(gameDirectory, "Tools", "Audio", "FaceFXWrapper.exe");
                        cmd.XwmEncoderPath = GetIfExists(gameDirectory, "Tools", "Audio", "xwmaencode.exe");
                        cmd.FonixDataPath = GetIfExists(gameDirectory, "Data", "Sound", "Voice", "Processing", "FonixData.cdf");
                        cmd.LipGeneratorPath = GetIfExists(gameDirectory, "Tools", "LipGen", "LipGenerator", "LipGenerator.exe");
                        cmd.LipFuzerPath = GetIfExists(gameDirectory, "Tools", "LipGen", "LipFuzer", "LIPFuzer.exe");
                    }

                    string GetIfExists(params IEnumerable<string> paths) {
                        var combinedPath = fileSystem.Path.Combine(paths.ToArray());
                        if (fileSystem.File.Exists(combinedPath)) {
                            return combinedPath;
                        }

                        return null!;
                    }

                    // Create argument objects based on selected tools
                    ILipGeneratorArgs lipGenArgs = cmd.LipGeneratorTool switch {
                        LipGeneratorTool.FaceFXWrapper => new FaceFxWrapperArgs(
                            cmd.FaceFxWrapperPath ?? throw new ArgumentException("FaceFxWrapperPath must be provided when using FaceFXWrapper tool"),
                            cmd.FonixDataPath ?? throw new ArgumentException("FonixDataPath (FonixData.cdf) path must be provided when using FaceFXWrapper tool"),
                            cmd.Language),
                        LipGeneratorTool.LipGenerator => new LipGeneratorArgs(
                            cmd.LipGeneratorPath ?? throw new ArgumentException("LipGeneratorPath must be provided when using LipGenerator tool"),
                            cmd.Language,
                            cmd.GestureExaggeration,
                            cmd.LipAnimSpeed,
                            cmd.LipAnimDelay),
                        _ => throw new NotSupportedException($"Unsupported lip generator tool: {cmd.LipGeneratorTool}")
                    };

                    IFuzGeneratorArgs fuzGenArgs = cmd.LipFuzerTool switch {
                        LipFuzerTool.LipFuzer => new LipFuzerArgs(
                            cmd.LipFuzerPath ?? throw new ArgumentException("LipFuzerPath must be provided when using LipFuzer tool")),
                        _ => throw new NotSupportedException($"Unsupported fuz generator tool: {cmd.LipFuzerTool}")
                    };

                    IAudioEncoderArgs? xwmEncoderArgs = cmd.AudioFormat == AudioFormatOption.XWM
                        ? new XwmEncoderArgs(
                            cmd.XwmEncoderPath ?? throw new ArgumentException("XwmEncoderPath must be provided when using XWM audio format"),
                            cmd.XwmBitrate)
                        : null; // Skip encoding for WAV

                    var lipGenerator = lipGeneratorFactory.Create(lipGenArgs, fuzGenArgs, xwmEncoderArgs);

                    var dataSourceService = container.Resolve<IDataSourceService>();
                    lipGenerator.Run(dataSourceService.ActiveDataSource, cmd.DeleteWavAndLipFiles);

                    Log.Information("Lip file generation complete");
                    return 0;
                },
                _ => Task.FromResult(-1))
            .ConfigureAwait(false);
    }
}
