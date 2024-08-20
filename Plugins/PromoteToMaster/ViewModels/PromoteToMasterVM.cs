using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Threading;
using CreationEditor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using CreationEditor.Skyrim.Definitions;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace PromoteToMaster.ViewModels;

public sealed class PromoteToMasterVM : ViewModel {
    private readonly IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> _editorEnvironment;

    public IReadOnlyList<IReferencedRecord> RecordsToPromote { get; }

    public IRecordReferenceController RecordReferenceController { get; }
    public IRecordController RecordController { get; }
    public SingleModPickerVM InjectedRecordCreationMod { get; }
    public SingleModPickerVM InjectToMod { get; }
    public SingleModPickerVM EditMod { get; }

    public IObservableCollection<RecordPromotionChange> RecordPromotionChanges { get; } = new ObservableCollectionExtended<RecordPromotionChange>();

    [Reactive] public string? RemovePrefix { get; set; }
    [Reactive] public string? AddPrefix { get; set; }

    [Reactive] public bool ForceDelete { get; set; }

    public ReactiveCommand<Unit, Unit> SettingsConfirmed { get; }
    public ReactiveCommand<Unit, Unit> Run { get; }

    public PromoteToMasterVM(
        IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
        IRecordReferenceController recordReferenceController,
        IRecordController recordController,
        SingleModPickerVM injectToMod,
        SingleModPickerVM injectedRecordCreationMod,
        SingleModPickerVM editMod,
        IReadOnlyList<IReferencedRecord> recordsToPromote) {
        _editorEnvironment = editorEnvironment;
        RecordsToPromote = recordsToPromote;
        RecordReferenceController = recordReferenceController;
        RecordController = recordController;

        InjectToMod = injectToMod;
        InjectToMod.Filter = FilterInjectToMod;

        InjectedRecordCreationMod = injectedRecordCreationMod;
        InjectedRecordCreationMod.CanCreateNewMod = true;
        InjectedRecordCreationMod.Filter = MutableModsFilter;

        EditMod = editMod;
        EditMod.SelectMod(_editorEnvironment.ActiveMod.ModKey);
        EditMod.CanCreateNewMod = true;
        EditMod.Filter = MutableModsFilter;

        var allModsSelected = InjectToMod.HasModSelected.CombineLatest(
            InjectedRecordCreationMod.HasModSelected,
            EditMod.HasModSelected,
            (a, b, c) => a && b && c);
        SettingsConfirmed = ReactiveCommand.CreateRunInBackground(() => {
                // Only run this once
                if (RecordPromotionChanges.Count > 0) return;

                var recordPromotionChanges = GetAffectedRecords(RecordsToPromote).ToList();
                Dispatcher.UIThread.Post(() => RecordPromotionChanges.ReplaceWith(recordPromotionChanges));
            },
            allModsSelected
        );

        Run = ReactiveCommand.CreateRunInBackground(() => {
                Save(RemovePrefix is null
                    ? record => record.EditorID
                    : record => CalculateEditorID(record.EditorID, RemovePrefix, AddPrefix));
            },
            allModsSelected);
    }

    private bool MutableModsFilter(IModKeyed mod) {
        // Vanilla masters shouldn't be edited
        if (SkyrimDefinitions.SkyrimModKeys.Contains(mod.ModKey)) return false;

        // The mod should be mutable
        if (_editorEnvironment.LinkCache.GetMod(mod.ModKey) is not IMod) return false;

        // No record should already be promoted to the mod
        return RecordsToPromote.Select(x => x.FormKey.ModKey)
            .Distinct()
            .All(modKey => modKey != mod.ModKey);
    }

    private bool FilterInjectToMod(IModKeyed mod) {
        // Vanilla masters are not valid injection targets 
        if (SkyrimDefinitions.SkyrimModKeys.Contains(mod.ModKey)) return false;

        // The injection target allows all records to be promoted to it
        return RecordsToPromote.Select(x => x.FormKey.ModKey)
            .Distinct()
            .All(modKey => _editorEnvironment
                .LinkCache.GetMod(modKey)
                .MasterReferences.Any(master => master.Master == mod.ModKey));
    }

    private static string? CalculateEditorID(string? editorId, string removePrefix, string? addPrefix) {
        if (editorId is null) return null;
        if (!editorId.StartsWith(removePrefix, StringComparison.OrdinalIgnoreCase)) return editorId;

        editorId = editorId[removePrefix.Length..];
        return addPrefix + editorId;
    }

    private void Save(Func<IMajorRecordGetter, string?> editorIdMapper) {
        if (InjectToMod.SelectedMod is null || InjectedRecordCreationMod.SelectedMod is null || EditMod.SelectedMod is null) return;

        var injectionTarget = _editorEnvironment.GetMod(InjectToMod.SelectedMod.ModKey);
        var newRecordMod = _editorEnvironment.GetMutableMod(InjectedRecordCreationMod.SelectedMod.ModKey);
        var editMod = _editorEnvironment.GetMutableMod(EditMod.SelectedMod.ModKey);

        var recordReferenceDictionary = RecordsToPromote.ToDictionary(x => x.FormKey, x => x);
        RecordController.InjectRecords(
            RecordsToPromote.Select(r => r.Record).ToList(),
            injectionTarget,
            newRecordMod,
            editMod,
            formKey => recordReferenceDictionary[formKey].References,
            editorIdMapper,
            ForceDelete);
    }

    public IEnumerable<RecordPromotionChange> GetAffectedRecords(IReadOnlyCollection<IReferencedRecord> recordsToBePromoted) {
        var includedRecords = new HashSet<FormKey>(recordsToBePromoted.Select(x => x.FormKey));

        // Promoted records will be deleted
        foreach (var referencedRecord in recordsToBePromoted) {
            yield return new RecordPromotionChange(referencedRecord.Record, PromotionChangeType.Deleted);
        }

        // References will relink to the promoted records
        foreach (var record in recordsToBePromoted) {
            var references = RecordReferenceController.GetReferences(record.FormKey).ToArray();
            if (references.Length == 0) continue;

            foreach (var reference in references) {
                if (includedRecords.Contains(reference.FormKey)) continue;
                if (!_editorEnvironment.LinkCache.TryResolveContext(reference, out var context)) continue;

                yield return new RecordPromotionChange(context.Record, PromotionChangeType.Modified);
            }
        }
    }
}

public sealed record RecordPromotionChange(IMajorRecordGetter Record, PromotionChangeType ChangeType);

public enum PromotionChangeType {
    Modified,
    Deleted,
}
