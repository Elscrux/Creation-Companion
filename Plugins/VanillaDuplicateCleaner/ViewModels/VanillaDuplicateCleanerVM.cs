using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Threading;
using CreationEditor;
using CreationEditor.Avalonia.Models.Record;
using CreationEditor.Avalonia.Models.Selectables;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Mod.Save;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using CreationEditor.Skyrim.Definitions;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VanillaDuplicateCleaner.Models;
namespace VanillaDuplicateCleaner.ViewModels;

public sealed class VanillaDuplicateCleanerVM : ViewModel {
    private readonly IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> _editorEnvironment;
    private readonly IModSaveService _modSaveService;
    public IRecordReferenceController RecordReferenceController { get; }
    public ModPickerVM ModPickerVM { get; }
    public ObservableCollection<SelectableRecordDiff> Records { get; } = [];

    [Reactive] public bool IsBusy { get; set; }

    public ReactiveCommand<ModKey, Unit> Run { get; }

    public VanillaDuplicateCleanerVM(
        IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
        IRecordReferenceController recordReferenceController,
        IModSaveService modSaveService,
        ModPickerVM modPickerVM) {
        _editorEnvironment = editorEnvironment;
        _modSaveService = modSaveService;
        RecordReferenceController = recordReferenceController;
        ModPickerVM = modPickerVM;
        ModPickerVM.Filter = mod => !SkyrimDefinitions.SkyrimModKeys.Contains(mod.ModKey);
        ModPickerVM.MultiSelect = false;

        ModPickerVM.SelectedModKey
            .Subscribe(modKey => {
                Records.Clear();
                if (modKey.IsNull) return;

                Records.AddRange(Process(modKey).Select(diff => new SelectableRecordDiff(diff)));
            });

        Run = ReactiveCommand.CreateRunInBackground<ModKey>(modKey => {
            Dispatcher.UIThread.Post(() => IsBusy = true);
            Save(modKey);
            Dispatcher.UIThread.Post(() => IsBusy = false);
        });
    }

    private List<RecordDiff> Process(ModKey modKey) {
        // Collect records from mods that may be replaced
        var recordEqualsMasks = new HashSet<RecordEqualsMask>();
        var testingMod = _editorEnvironment.LinkCache.PriorityOrder.First(mod => mod.ModKey == modKey);
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

        var cleanedBaseMod = new SkyrimMod(ModKey.FromName("Cleaned" + modKey.Name, ModType.Plugin), SkyrimRelease.SkyrimSE);
        var cleanedOtherMods = new List<SkyrimMod>();

        var recordReplacements = Records
            .Where(r => r.IsSelected)
            .ToArray();

        // Replace mod references of records to be replaced with vanilla records
        foreach (var recordReplacement in recordReplacements) {
            var references = RecordReferenceController.GetReferences(recordReplacement.RecordDiff.Old.FormKey).ToArray();

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
                        () => new SkyrimMod(ModKey.FromName(modName, ModType.Plugin), SkyrimRelease.SkyrimSE));

                    context.GetOrAddAsOverride(cleanedMod);
                }
            }

            // Clean record
            recordReplacement.RecordDiff.Old.MarkForDeletion(
                _editorEnvironment.LinkCache,
                cleanedBaseMod,
                () => references);
        }

        // Collect remapping data
        var remapData = recordReplacements
            .ToDictionary(x => x.RecordDiff.Old.FormKey, x => x.RecordDiff.New.FormKey);

        // Remap links and save mods
        FinalizeMod(cleanedBaseMod);
        foreach (var cleanedOtherMod in cleanedOtherMods) {
            FinalizeMod(cleanedOtherMod);
        }

        void FinalizeMod(IMod mod) {
            mod.RemapLinks(remapData);
            _modSaveService.SaveMod(mod);
        }
    }
}
