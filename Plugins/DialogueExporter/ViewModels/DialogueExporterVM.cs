using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Services.Environment;
using DialogueExporter.Services.VaSynth;
using DialogueExporter.Services.VoiceSheets;
using DialogueExporter.Services.VoiceSheets.Writer;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace DialogueExporter.ViewModels;

public sealed partial class DialogueExporterVM : ViewModel {
    [Reactive] public partial string VoiceLineOutputFolder { get; set; } = string.Empty;
    [Reactive] public partial string VaSynthOutputFile { get; set; } = string.Empty;
    [Reactive] public partial string VoiceTypeMappingCsvFile { get; set; } = string.Empty;
    [Reactive] public partial string QuestFilterRegex { get; set; } = ".*";
    [Reactive] public partial string Vocoder { get; set; } = "hifi";
    [Reactive] public partial bool SkipAlreadyVoiced { get; set; } = true;

    public SingleModPickerVM ExportModPickerVM { get; }

    public ReactiveCommand<Unit, Unit> ExportVoiceSheets { get; }
    public ReactiveCommand<Unit, Unit> ExportVaSynth { get; }

    public DialogueExporterVM(
        IFileSystem fileSystem,
        IEditorEnvironment editorEnvironment,
        ExportVaSynth exportVaSynth,
        ExportVoiceSheets exportVoiceSheets,
        WriteXlsx writeXlsx,
        SingleModPickerVM singleModPickerVM) {
        ExportModPickerVM = singleModPickerVM;

        var voiceLineOutputValid = this.WhenAnyValue(x => x.VoiceLineOutputFolder).Select(x => !x.IsNullOrWhitespace());
        var xVaSynthOutputValid = this.WhenAnyValue(x => x.VaSynthOutputFile).Select(x => !x.IsNullOrWhitespace());
        var voiceTypeMappingCsvValid = this.WhenAnyValue(x => x.VoiceTypeMappingCsvFile).Select(x => !x.IsNullOrWhitespace());

        ExportVoiceSheets = ReactiveCommand.CreateRunInBackground(() => {
                var selectedMod = editorEnvironment.ResolveMod(ExportModPickerVM.SelectedMod?.ModKey);
                if (selectedMod is null) return;

                if (VoiceLineOutputFolder.IsNullOrWhitespace()) return;

                if (!fileSystem.Directory.Exists(VoiceLineOutputFolder)) {
                    fileSystem.Directory.CreateDirectory(VoiceLineOutputFolder);
                }

                var lines = exportVoiceSheets.GetLines(selectedMod, SkipAlreadyVoiced);
                writeXlsx.Write(lines, VoiceLineOutputFolder);
            },
            voiceLineOutputValid
                .CombineLatest(ExportModPickerVM.HasModSelected, (a, b) => a && b));

        var csvValid = this.WhenAnyValue(x => x.VoiceTypeMappingCsvFile).Select(csv => fileSystem.File.Exists(csv));

        ExportVaSynth = ReactiveCommand.CreateRunInBackground(() => {
                var selectedMod = editorEnvironment.ResolveMod(ExportModPickerVM.SelectedMod?.ModKey);
                if (selectedMod is null) return;

                if (!fileSystem.File.Exists(VoiceTypeMappingCsvFile)) return;

                if (VaSynthOutputFile.IsNullOrWhitespace()) return;

                var directoryPath = fileSystem.Path.GetDirectoryName(VaSynthOutputFile);
                if (directoryPath is null) return;

                if (!fileSystem.Directory.Exists(directoryPath)) {
                    fileSystem.Directory.CreateDirectory(directoryPath);
                }

                exportVaSynth.Export(selectedMod, VoiceTypeMappingCsvFile, VaSynthOutputFile, QuestFilterRegex, Vocoder);
            },
            xVaSynthOutputValid
                .CombineLatest(
                    ExportModPickerVM.HasModSelected,
                    csvValid,
                    voiceTypeMappingCsvValid,
                    (a, b, c, d) => a && b && c && d));
    }
}
