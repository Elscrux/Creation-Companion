using System.Reactive.Linq;
using Autofac;
using CommandLine;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Services.Plugin;
using DialogueExporter.Services.VoiceSheets;
using DialogueExporter.Services.VoiceSheets.Writer;
namespace DialogueExporter.Cli.Args;

[Verb("export-voice-sheets", HelpText = "Export voice sheets from a mod.")]
public record CommandLineEntryPointVoiceSheets : ICommandLineEntryPoint, IDataSourceArguments {
    [Option('m',
        "ModFilename",
        Required = true,
        HelpText = "File name of mod to export")]
    public string ModFilename { get; set; } = null!;

    [Option('d',
        "ModDataSource",
        Required = false,
        HelpText = "Path to a directory containing the mod")]
    public string ActiveDataSourcePath { get; set; } = null!;

    [Option("AdditionalDataSourcePaths",
        Required = false,
        HelpText = "Additional directories containing mod master files")]
    public IEnumerable<string> AdditionalDataSourcePaths { get; set; } = [];

    public bool IncludeDataDirectoryDataSource { get; set; } = true;

    [Option('o',
        "OutputDirectory",
        Required = true,
        HelpText = "Directory to export voice sheets to")]
    public string OutputDirectory { get; set; } = null!;

    [Option('i',
        "IncludeAlreadyVoiced",
        Required = false,
        HelpText = "Whether to include lines that already have voice files")]
    public bool IncludeAlreadyVoiced { get; set; } = false;

    public IReadOnlySet<string> Verbs { get; } = (HashSet<string>) ["export-voice-sheets"];

    public async Task<int> Run(string[] args) {
        return await Parser.Default.ParseArguments<CommandLineEntryPointVoiceSheets>(args)
            .MapResult(async cmd => {
                    Console.WriteLine("Starting export with arguments: " + cmd);

                    await using var container = CommandLineContainerSetup.Setup(cmd, cmd.ModFilename);
                    if (container is null) return -1;

                    // Wait until record references are loaded
                    var referenceService = container.Resolve<IReferenceService>();
                    await referenceService.IsLoadingRecordReferences.FirstAsync(x => !x);

                    var exportVoiceSheets = container.Resolve<ExportVoiceSheets>();
                    var editorEnvironment = container.Resolve<IEditorEnvironment>();
                    var writeXlsx = container.Resolve<WriteXlsx>();
                    var currentMod = editorEnvironment.LinkCache.PriorityOrder.First(l => l.ModKey.FileName == cmd.ModFilename);
                    var lines = exportVoiceSheets.GetLines(currentMod, cmd.IncludeAlreadyVoiced);
                    writeXlsx.Write(lines, cmd.OutputDirectory);
                    return 0;
                },
                _ => Task.FromResult(-1))
            .ConfigureAwait(false);
    }
}
