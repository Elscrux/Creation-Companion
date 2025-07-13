using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Threading;
using CreationEditor;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.Models.Selectables;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim.Definitions;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using VanillaDuplicateCleaner.Models;
namespace VanillaDuplicateCleaner.ViewModels;

public sealed partial class VanillaDuplicateCleanerVM : ViewModel {
    private readonly IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> _editorEnvironment;
    private readonly IRecordController _recordController;
    public IReferenceService ReferenceService { get; }
    public SingleModPickerVM ModPickerVM { get; }
    public ObservableCollection<SelectableRecordDiff> Records { get; } = [];

    [Reactive] public partial bool IsBusy { get; set; }

    public ReactiveCommand<OrderedModItem, Unit> Run { get; }

    public VanillaDuplicateCleanerVM(
        IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
        IRecordController recordController,
        IReferenceService referenceService,
        SingleModPickerVM modPickerVM) {
        _editorEnvironment = editorEnvironment;
        _recordController = recordController;
        ReferenceService = referenceService;
        ModPickerVM = modPickerVM;
        ModPickerVM.Filter = mod => !SkyrimDefinitions.SkyrimModKeys.Contains(mod.ModKey);

        ModPickerVM.WhenAnyValue(x => x.SelectedMod)
            .Subscribe(mod => {
                if (mod is null || mod.ModKey.IsNull) {
                    Records.Clear();
                } else {
                    Records.ReplaceWith(Process(mod.ModKey).Select(diff => new SelectableRecordDiff { RecordDiff = diff }));
                }
            });

        Run = ReactiveCommand.CreateRunInBackground<OrderedModItem>(mod => {
            Dispatcher.UIThread.Post(() => IsBusy = true);
            Save(mod.ModKey);
            Dispatcher.UIThread.Post(() => IsBusy = false);
        });
    }

    private List<RecordDiff> Process(ModKey modKey) {
        // Collect records from mods that may be replaced
        var recordEqualsMasks = new HashSet<RecordEqualsMask>();
        var testingMod = _editorEnvironment.GetMod(modKey);
        foreach (var record in GetRecords(testingMod.AsEnumerable())) {
            if (record.EditorID is null) continue;

            var store = new RecordEqualsMask(record);
            recordEqualsMasks.Add(store);
        }

        // Find matching records in vanilla mods
        var mappedRecords = new List<RecordDiff>();
        var vanillaMods = _editorEnvironment.LinkCache.ListedOrder
            .Where(x => Enumerable.Contains(SkyrimDefinitions.SkyrimModKeys, x.ModKey))
            .ToArray();

        foreach (var vanillaRecord in GetRecords(vanillaMods)) {
            if (vanillaRecord.EditorID is null) continue;

            var vanillaEqualsMask = new RecordEqualsMask(vanillaRecord);
            if (recordEqualsMasks.TryGetValue(vanillaEqualsMask, out var match)) {
                mappedRecords.Add(new RecordDiff(match.Record, vanillaRecord));
            }
        }

        return mappedRecords;

        IEnumerable<IMajorRecordGetter> GetRecords(IEnumerable<IModGetter> mods) {
            var modList = mods.ToList();

            return modList.WinningOverrides<IActivatorGetter>().Cast<IMajorRecordGetter>()
                .Concat(modList.WinningOverrides<IContainerGetter>())
                .Concat(modList.WinningOverrides<IStaticGetter>())
                .Concat(modList.WinningOverrides<IFloraGetter>())
                .Concat(modList.WinningOverrides<ITreeGetter>());
        }
    }

    private void Save(ModKey modKey) {
        if (modKey.IsNull) return;

        var cleanedBaseMod = _editorEnvironment.AddNewMutableMod<ISkyrimMod>(ModKey.FromName("Cleaned" + modKey.Name, ModType.Plugin));
        var cleanedOtherMods = new List<ISkyrimMod>();

        var recordReplacements = Records
            .Where(r => r.IsSelected)
            .ToArray();

        // Replace mod references of records to be replaced with vanilla records
        foreach (var recordReplacement in recordReplacements) {
            var references = ReferenceService.GetRecordReferences(recordReplacement.RecordDiff.Old).ToArray();

            // Clean references
            foreach (var reference in references) {
                if (!_editorEnvironment.LinkCache.TryResolveContext(reference, out var context)) continue;

                // Add references to the relevant cleaned mod
                if (context.ModKey == modKey) {
                    context.GetOrAddAsOverride(cleanedBaseMod);
                } else {
                    var modName = $"Cleaned{context.ModKey.Name}From{modKey.Name}";
                    var cleanedMod = cleanedOtherMods.FindOrAdd(
                        mod => mod.ModKey.Name == modName,
                        () => _editorEnvironment.AddNewMutableMod<ISkyrimMod>(ModKey.FromName(modName, ModType.Plugin)));

                    context.GetOrAddAsOverride(cleanedMod);
                }
            }

            // Clean record
            _recordController.MarkForDeletion(recordReplacement.RecordDiff.Old, cleanedBaseMod, () => references);
        }

        // Collect remapping data
        var remapData = recordReplacements
            .ToDictionary(x => x.RecordDiff.Old.FormKey, x => x.RecordDiff.New.FormKey);

        // Remap links and save mods
        cleanedBaseMod.RemapLinks(remapData);
        foreach (var cleanedOtherMod in cleanedOtherMods) {
            cleanedOtherMod.RemapLinks(remapData);
        }
    }
}
