using System.Collections.ObjectModel;
using System.Reactive;
using Autofac;
using Avalonia.Threading;
using CreationEditor.Avalonia.Models.Selectables;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Services.Mutagen.Mod.Save;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using CreationEditor.Services.Plugin;
using CreationEditor.Skyrim.Definitions;
using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace DLCMapper;

public sealed class RecordReplacement : ReactiveObject, IReactiveSelectable {
    public RecordReplacement(IMajorRecordGetter modified, IMajorRecordGetter vanillaReplacement, string type) {
        Modified = modified;
        VanillaReplacement = vanillaReplacement;
        Type = type;
    }

    [Reactive] public bool IsSelected { get; set; }
    public IMajorRecordGetter Modified { get; }
    public IMajorRecordGetter VanillaReplacement { get; }
    public string Type { get; }
}

public sealed class VanillaDuplicateCleanerVM : ViewModel {
    private readonly PluginContext<ISkyrimMod, ISkyrimModGetter> _pluginContext;
    
    public IRecordReferenceController RecordReferenceController { get; }
    public ModPickerVM ModPickerVM { get; }
    public RecordSelectionVM RecordSelectionVM { get; }
    public ObservableCollection<RecordReplacement> Records { get; } = new();

    [Reactive] public bool IsBusy { get; set; }
    public ModKey ActiveModKey { get; set; } = ModKey.Null;

    public ReactiveCommand<Unit, Unit> Run { get; }

    public VanillaDuplicateCleanerVM(
        PluginContext<ISkyrimMod, ISkyrimModGetter> pluginContext) {
        _pluginContext = pluginContext;
        RecordReferenceController = _pluginContext.LifetimeScope.Resolve<IRecordReferenceController>();
        
        // _recordReferenceController.RegisterCreation();

        ModPickerVM = pluginContext.LifetimeScope.Resolve<ModPickerVM>();
        ModPickerVM.Filter = mod => !Enumerable.Contains(SkyrimDefinitions.SkyrimModKeys, mod.ModKey);
        ModPickerVM.MultiSelect = false;

        var emptyIdentifiers = TypedParameter.From<IEnumerable<IFormLinkIdentifier>>(Array.Empty<IFormLinkIdentifier>());
        var identifiersProvider = pluginContext.LifetimeScope.Resolve<RecordIdentifiersProvider>(emptyIdentifiers);
        RecordSelectionVM = new RecordSelectionVM(identifiersProvider);

        ModPickerVM.SelectedMods
            .Subscribe(mods => {
                Records.Clear();
                ActiveModKey = ModKey.Null;
                if (mods.Count == 0) return;

                ActiveModKey = mods.First().ModKey;
                ListEx.AddRange(Records, Process(ActiveModKey));
            });

        Run = ReactiveCommand.CreateRunInBackground(() => {
            Dispatcher.UIThread.Post(() => IsBusy = true);
            Save(ActiveModKey);
            Dispatcher.UIThread.Post(() => IsBusy = false);
        });
    }

    private IEnumerable<RecordReplacement> Process(ModKey testingModKey) {
        IEnumerable<IMajorRecordGetter> GetRecords(IEnumerable<IModGetter> mods) {
            var modList = mods.ToList();

            return modList.WinningOverrides<IActivatorGetter>().Cast<IMajorRecordGetter>()
                .Concat(modList.WinningOverrides<IContainerGetter>())
                .Concat(modList.WinningOverrides<IStaticGetter>())
                .Concat(modList.WinningOverrides<IFloraGetter>())
                .Concat(modList.WinningOverrides<ITreeGetter>());
        }

        // Collect records from mods that may be replaced
        var recordEqualsMasks = new HashSet<RecordEqualsMask>();
        var testingMod = _pluginContext.EditorEnvironment.LinkCache.PriorityOrder.First(mod => mod.ModKey == testingModKey);
        foreach (var record in GetRecords(testingMod.AsEnumerable())) {
            if (record.EditorID == null) continue;

            var store = new RecordEqualsMask(record);
            recordEqualsMasks.Add(store);
        }

        // Find matching records in vanilla mods
        var mappedRecords = new List<RecordReplacement>();
        var vanillaMods = _pluginContext.EditorEnvironment.LinkCache.ListedOrder
            .Where(x => Enumerable.Contains(SkyrimDefinitions.SkyrimModKeys, x.ModKey))
            .ToArray();

        foreach (var vanillaRecord in GetRecords(vanillaMods)) {
            if (vanillaRecord.EditorID == null) continue;

            var vanillaEqualsMask = new RecordEqualsMask(vanillaRecord);
            if (recordEqualsMasks.TryGetValue(vanillaEqualsMask, out var match)) {
                mappedRecords.Add(new RecordReplacement(match.Record, vanillaRecord, match.Record.Registration.Name));
            }
        }

        return mappedRecords;
    }

    private void Save(ModKey modKey) {
        if (modKey.IsNull) return;

        var recordReferenceController = _pluginContext.LifetimeScope.Resolve<IRecordReferenceController>();
        var saveService = _pluginContext.LifetimeScope.Resolve<IModSaveService>();
        var cleanedBaseMod = new SkyrimMod(ModKey.FromName("Cleaned" + modKey.Name, ModType.Plugin), SkyrimRelease.SkyrimSE);
        var cleanedOtherMods = new List<SkyrimMod>();

        var recordReplacements = Records
            .Where(r => r.IsSelected)
            .ToArray();

        // Replace mod references of records to be replaced with vanilla records
        foreach (var recordReplacement in recordReplacements) {
            var references = recordReferenceController.GetReferences(recordReplacement.Modified.FormKey).ToArray();

            // Clean references
            foreach (var usageFormLink in references) {
                if (!_pluginContext.EditorEnvironment.LinkCache.TryResolveContext(usageFormLink, out var context)) continue;

                // Add references to the relevant cleaned mod
                if (context.ModKey == modKey) {
                    context.GetOrAddAsOverride(cleanedBaseMod);
                } else {
                    var modName = $"Cleaned{context.ModKey.Name}From{modKey.Name}";
                    var cleanedMod = cleanedOtherMods.FirstOrDefault(m => m.ModKey.Name == modName);
                    if (cleanedMod == null) {
                        cleanedMod = new SkyrimMod(ModKey.FromName(modName, ModType.Plugin), SkyrimRelease.SkyrimSE);
                        cleanedOtherMods.Add(cleanedMod);
                    }

                    context.GetOrAddAsOverride(cleanedMod);
                }
            }

            // Clean record
            if (_pluginContext.EditorEnvironment.LinkCache.TryResolveContext(recordReplacement.Modified.ToLinkFromRuntimeType(), out var recordContext)) {
                var overrideRecord = recordContext.GetOrAddAsOverride(cleanedBaseMod);

                if (references.Length == 0) {
                    // No references available - we can delete this record
                    overrideRecord.IsDeleted = true;
                } else {
                    // Otherwise mark record for deletion
                    if (overrideRecord.EditorID?.StartsWith("x", StringComparison.OrdinalIgnoreCase) is false) {
                        overrideRecord.EditorID = "xDELETE" + overrideRecord.EditorID;
                    }
                }
            }
        }

        // Collect remapping data
        var remapData = recordReplacements
            .ToDictionary(x => x.Modified.FormKey, x => x.VanillaReplacement.FormKey);

        // Remap links and save mods
        FinalizeMod(cleanedBaseMod);
        foreach (var cleanedOtherMod in cleanedOtherMods) {
            FinalizeMod(cleanedOtherMod);
        }

        void FinalizeMod(IMod mod) {
            mod.RemapLinks(remapData);
            saveService.SaveMod(_pluginContext.EditorEnvironment.LinkCache, mod);
        }
    }
}
