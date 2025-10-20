using Autofac;
using CommandLine;
using CreationEditor;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Plugin;
using DialogueExporter.Services.VaSynth;
using Mutagen.Bethesda.Plugins;
namespace DialogueExporter.Cli.Args;

[Verb("export-va-synth", HelpText = "Export batch file for xVASynth.")]
public record CommandLineEntryPointVaSynth : ICommandLineEntryPoint, IDataSourceArguments {
    [Option('m',
        "ModFilename",
        Required = true,
        HelpText = "File name of mod to export")]
    public string ModFilename { get; set; } = null!;

    [Option('q',
        "QuestEditorRegex",
        Required = false,
        HelpText = "Regex to match quest editor IDs for voice files. Defaults to '.*' (all quests).")]
    public string QuestEditorRegex { get; set; } = ".*";

    [Option('o',
        "OutputDirectory",
        Required = true,
        HelpText = "Directory to export voice sheets to")]
    public string OutputDirectory { get; set; } = null!;

    [Option('d',
        "ModDirectory",
        Required = false,
        Default = "The data directory",
        HelpText = "Directory of the mod to detect the voice files from")]
    public string ActiveDataSourcePath { get; set; } = null!;

    [Option("AdditionalDataSourcePaths",
        Required = false,
        HelpText = "Additional directories containing mod master files")]
    public IEnumerable<string> AdditionalDataSourcePaths { get; set; } = [];

    [Option('v',
        "VoiceTypeMappingCsvFile",
        Required = true,
        HelpText = "CSV file mapping voice types to xVASynth voice IDs. Format: 'FemaleCommander,sk_femalecommander'")]
    public string VoiceTypeMappingCsvFile { get; set; } = null!;

    [Option("IncludeDataDirectoryDataSource",
        Required = false,
        HelpText = "Use the game's data directory as a data source")]
    public bool IncludeDataDirectoryDataSource { get; set; } = true;

    [Option("Vocoder",
        Required = false,
        Default = "hifi",
        HelpText = "Vocoder to use in xVASynth")]
    public string Vocoder { get; set; } = null!;

    public IReadOnlySet<string> Verbs { get; } = (HashSet<string>) ["export-va-synth"];

    public async Task<int> Run(string[] args) {
        return await Parser.Default.ParseArguments<CommandLineEntryPointVaSynth>(args)
            .MapResult(cmd => {
                    Console.WriteLine("Starting VA Synth Export with arguments: " + cmd);
                    using var container = CommandLineContainerSetup.Setup(cmd, cmd.ModFilename);
                    if (container is null) return Task.FromResult(-1);

                    var exportVaSynth = container.Resolve<ExportVaSynth>();
                    var editorEnvironment = container.Resolve<IEditorEnvironment>();
                    var modKey = ModKey.FromFileName(cmd.ModFilename);
                    var mod = editorEnvironment.LinkCache.ResolveMod(modKey);
                    if (mod is null) {
                        Console.WriteLine("Mod " + cmd.ModFilename + " could not be found in the data source.");
                        return Task.FromResult(-1);
                    }

                    exportVaSynth.Export(mod, cmd.VoiceTypeMappingCsvFile, cmd.OutputDirectory, cmd.QuestEditorRegex, cmd.Vocoder);
                    return Task.FromResult(0);
                },
                _ => Task.FromResult(-1))
            .ConfigureAwait(false);
    }
}
