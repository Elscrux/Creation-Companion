using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Linq;
using CreationEditor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References;
using DialogueExporter.Services.VaSynth;
using DialogueExporter.Services.VoiceSheets;
using DialogueExporter.Services.VoiceSheets.Writer;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace DialogueExporter.ViewModels;

public sealed partial class DialogueExporterVM : ViewModel {
    private readonly IFileSystem _fileSystem;
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly ExportVaSynth _exportVaSynth;
    private readonly ExportVoiceSheets _exportVoiceSheets;
    private readonly WriteXlsx _writeXlsx;
    public IReadOnlyList<string> VocoderOptions { get; } = ["hifi", "quickanddirty", "waveglow", "waveglowBIGq"];

    [Reactive] public partial string VoiceLineOutputFolder { get; set; } = string.Empty;
    [Reactive] public partial string VaSynthOutputFile { get; set; } = string.Empty;
    [Reactive] public partial string VoiceTypeMappingCsvFile { get; set; } = string.Empty;
    [Reactive] public partial string QuestFilterRegex { get; set; } = ".*";
    [Reactive] public partial string Vocoder { get; set; } = "hifi";
    [Reactive] public partial bool SkipAlreadyVoiced { get; set; } = true;

    public IReferenceService ReferenceService { get; }
    public SingleModPickerVM ExportModPickerVM { get; }

    public ReactiveCommand<Unit, Unit> ExportVoiceSheetsCommand { get; }
    public ReactiveCommand<Unit, Unit> ExportVaSynthCommand { get; }
    public IObservable<bool> CanExportVoiceSheets { get; set; }
    public IObservable<bool> CanExportVaSynth { get; set; }

    public DialogueExporterVM(
        IFileSystem fileSystem,
        IReferenceService referenceService,
        IEditorEnvironment editorEnvironment,
        ExportVaSynth exportVaSynth,
        ExportVoiceSheets exportVoiceSheets,
        WriteXlsx writeXlsx,
        SingleModPickerVM singleModPickerVM) {
        _fileSystem = fileSystem;
        _editorEnvironment = editorEnvironment;
        _exportVaSynth = exportVaSynth;
        _exportVoiceSheets = exportVoiceSheets;
        _writeXlsx = writeXlsx;
        ReferenceService = referenceService;
        ExportModPickerVM = singleModPickerVM;

        var voiceLineOutputValid = this.WhenAnyValue(x => x.VoiceLineOutputFolder).Select(x => !x.IsNullOrWhitespace());
        var xVaSynthOutputValid = this.WhenAnyValue(x => x.VaSynthOutputFile).Select(x => !x.IsNullOrWhitespace());
        var voiceTypeMappingCsvValid = this.WhenAnyValue(x => x.VoiceTypeMappingCsvFile).Select(x => !x.IsNullOrWhitespace());
        var referencesLoaded = referenceService.IsLoadingRecordReferences.Negate();
        CanExportVoiceSheets = referencesLoaded
            .CombineLatest(
                ExportModPickerVM.HasModSelected,
                voiceLineOutputValid,
                (a, b, c) => a && b && c)
            .ObserveOnGui();
        ExportVoiceSheetsCommand = ReactiveCommand.CreateRunInBackground(ExportVoiceSheets, CanExportVoiceSheets);

        var csvValid = this.WhenAnyValue(x => x.VoiceTypeMappingCsvFile).Select(csv => fileSystem.File.Exists(csv));
        var vocoderValid = this.WhenAnyValue(x => x.Vocoder).Select(vocoder => !vocoder.IsNullOrEmpty());
        CanExportVaSynth = referencesLoaded
            .CombineLatest(
                ExportModPickerVM.HasModSelected,
                xVaSynthOutputValid,
                csvValid,
                vocoderValid,
                voiceTypeMappingCsvValid,
                (a, b, c, d, e, f) => a && b && c && d && e && f)
            .ObserveOnGui();
        ExportVaSynthCommand = ReactiveCommand.CreateRunInBackground(ExportVaSynth, CanExportVaSynth);
    }

    private void ExportVoiceSheets() {
        var selectedMod = _editorEnvironment.ResolveMod(ExportModPickerVM.SelectedMod?.ModKey);
        if (selectedMod is null) return;

        if (VoiceLineOutputFolder.IsNullOrWhitespace()) return;

        if (!_fileSystem.Directory.Exists(VoiceLineOutputFolder)) {
            _fileSystem.Directory.CreateDirectory(VoiceLineOutputFolder);
        }

        var lines = _exportVoiceSheets.GetLines(selectedMod, SkipAlreadyVoiced);
        _writeXlsx.Write(lines, VoiceLineOutputFolder);
    }

    private void ExportVaSynth() {
        var selectedMod = _editorEnvironment.ResolveMod(ExportModPickerVM.SelectedMod?.ModKey);
        if (selectedMod is null) return;

        if (!_fileSystem.File.Exists(VoiceTypeMappingCsvFile)) return;

        if (VaSynthOutputFile.IsNullOrWhitespace()) return;

        var directoryPath = _fileSystem.Path.GetDirectoryName(VaSynthOutputFile);
        if (directoryPath is null) return;

        if (!_fileSystem.Directory.Exists(directoryPath)) {
            _fileSystem.Directory.CreateDirectory(directoryPath);
        }

        _exportVaSynth.Export(selectedMod, VoiceTypeMappingCsvFile, VaSynthOutputFile, QuestFilterRegex, Vocoder);
    }
}
