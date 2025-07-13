using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Threading;
using CreationEditor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim;
using CreationEditor.Skyrim.Definitions;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace PromoteToMaster.ViewModels;

public sealed partial class PromoteToMasterVM : ViewModel {
    public static readonly IReadOnlyList<AssetPromotionMode> AssetPromotionModes = Enum.GetValues<AssetPromotionMode>();

    private readonly IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> _editorEnvironment;
    private readonly IAssetController _assetController;

    public IReadOnlyList<IReferencedRecord> RecordsToPromote { get; }

    public IReferenceService ReferenceService { get; }
    public IRecordController RecordController { get; }
    public SingleModPickerVM InjectedRecordCreationMod { get; }
    public SingleModPickerVM InjectToMod { get; }
    public SingleModPickerVM EditMod { get; }

    public IObservableCollection<RecordPromotionChange> RecordPromotionChanges { get; } = new ObservableCollectionExtended<RecordPromotionChange>();
    public IObservableCollection<AssetPromotionChange> AssetPromotionChanges { get; } = new ObservableCollectionExtended<AssetPromotionChange>();

    [Reactive] public partial string? RemovePrefix { get; set; }
    [Reactive] public partial string? AddPrefix { get; set; }

    [Reactive] public partial bool ForceDelete { get; set; }

    public ReactiveCommand<Unit, Unit> SettingsConfirmed { get; }
    public ReactiveCommand<Unit, Unit> Run { get; }

    public AssetPromotionMode AssetPromotionMode { get; set; } = AssetPromotionMode.ForceMove;
    public IObservable<bool> NoAssetTarget { get; }
    public IObservableCollection<IDataSource> AssetOrigins { get; } = new ObservableCollectionExtended<IDataSource>();
    public IObservableCollection<IDataSource> AssetTargets { get; } = new ObservableCollectionExtended<IDataSource>();

    public PromoteToMasterVM(
        IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
        IDataSourceService dataSourceService,
        IReferenceService referenceService,
        IRecordController recordController,
        IAssetController assetController,
        SingleModPickerVM injectToMod,
        SingleModPickerVM injectedRecordCreationMod,
        SingleModPickerVM editMod,
        IReadOnlyList<IReferencedRecord> recordsToPromote) {
        _editorEnvironment = editorEnvironment;
        _assetController = assetController;
        RecordsToPromote = recordsToPromote;
        ReferenceService = referenceService;
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

        dataSourceService
            .DataSourcesChanged
            .Subscribe(dataSources => {
                dataSources = dataSources.Where(FilterDataSources).ToList();
                AssetOrigins.Load(dataSources.Except(AssetTargets));
                AssetTargets.Load(dataSources.Intersect(AssetTargets));
            })
            .DisposeWith(this);

        var allModsSelected = InjectToMod.HasModSelected.CombineLatest(
            InjectedRecordCreationMod.HasModSelected,
            EditMod.HasModSelected,
            (a, b, c) => a && b && c);
        SettingsConfirmed = ReactiveCommand.CreateRunInBackground(() => {
                if (InjectToMod.SelectedMod is null) return;

                // Only run this once
                IReadOnlyList<RecordPromotionChange> recordPromotionChanges;
                if (RecordPromotionChanges.Count == 0) {
                    var injectionTarget = _editorEnvironment.GetMod(InjectToMod.SelectedMod.ModKey);
                    recordPromotionChanges = GetAffectedRecords(RecordsToPromote, injectionTarget).ToList();
                    Dispatcher.UIThread.Post(() => RecordPromotionChanges.Load(recordPromotionChanges));
                } else {
                    recordPromotionChanges = RecordPromotionChanges.ToList();
                }

                var promotedRecords = recordPromotionChanges
                    .Where(x => x.ChangeType == RecordPromotionChangeType.Deleted)
                    .Select(x => x.Record)
                    .ToArray();
                var assetPromotionChanges = GetAffectedAssets(promotedRecords).ToList();
                Dispatcher.UIThread.Post(() => AssetPromotionChanges.Load(assetPromotionChanges));
            },
            allModsSelected
        );

        Run = ReactiveCommand.CreateRunInBackground(() => {
                Save(RemovePrefix is null
                    ? record => record.EditorID
                    : record => CalculateEditorID(record.EditorID, RemovePrefix, AddPrefix));
            },
            allModsSelected);

        var assetTargetCountChanges = this.WhenAnyValue(x => x.AssetTargets.Count);

        NoAssetTarget = assetTargetCountChanges.Select(count => count == 0);

        // Make sure there is only at max one target data source
        assetTargetCountChanges
            .Where(count => count > 1)
            .Subscribe(_ => {
                AssetOrigins.AddRange(AssetTargets.Skip(1).ToArray());
                AssetTargets.RemoveRange(1, AssetTargets.Count - 1);
            })
            .DisposeWith(this);
    }

    private static bool FilterDataSources(IDataSource dataSource) {
        return !dataSource.IsReadOnly;
    }

    private bool MutableModsFilter(IModKeyed mod) {
        // Vanilla masters shouldn't be edited
        if (MemoryExtensions.Contains(SkyrimDefinitions.SkyrimModKeys, mod.ModKey)) return false;

        // The mod should be mutable
        if (_editorEnvironment.LinkCache.GetMod(mod.ModKey) is not IMod) return false;

        // No record should already be promoted to the mod
        return RecordsToPromote.Select(x => x.FormKey.ModKey)
            .Distinct()
            .All(modKey => modKey != mod.ModKey);
    }

    private bool FilterInjectToMod(IModKeyed mod) {
        // Vanilla masters are not valid injection targets 
        if (MemoryExtensions.Contains(SkyrimDefinitions.SkyrimModKeys, mod.ModKey)) return false;

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
            formKey => recordReferenceDictionary[formKey].RecordReferences,
            editorIdMapper,
            ForceDelete);

        if (AssetTargets is not [var assetTarget]) return;

        foreach (var promotion in AssetPromotionChanges) {
            var targetLink = new DataSourceFileLink(assetTarget, promotion.FileLink.DataRelativePath);

            switch (promotion.ChangeType) {
                case AssetPromotionChangeType.Moved:
                    _assetController.Move(promotion.FileLink, targetLink);
                    break;
                case AssetPromotionChangeType.Copied:
                    _assetController.Copy(promotion.FileLink, targetLink);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }

    public IEnumerable<RecordPromotionChange> GetAffectedRecords(
        IReadOnlyCollection<IReferencedRecord> recordsToBePromoted,
        ISkyrimModGetter injectionTarget) {
        var allMastersAndInjection = injectionTarget.GetTransitiveMasters(_editorEnvironment.GameEnvironment, true).ToArray();

        // Gather all linked records that are not already in the target mod
        var includedRecords = new HashSet<IMajorRecordGetter>(recordsToBePromoted.Select(x => x.Record));
        var recordsToCheck = new Queue<IMajorRecordGetter>();
        recordsToCheck.Enqueue(recordsToBePromoted.Select(x => x.Record));
        while (recordsToCheck.Count > 0) {
            var record = recordsToCheck.Dequeue();

            // Enqueue all linked records to check them as well
            foreach (var link in record.EnumerateFormLinks()) {
                if (allMastersAndInjection.Contains(link.FormKey.ModKey)) continue;
                if (!_editorEnvironment.LinkCache.TryResolve(link, out var linkedRecord)) continue;

                if (includedRecords.Add(linkedRecord)) {
                    recordsToCheck.Enqueue(linkedRecord);
                }
            }
        }

        // Promoted records will be deleted
        foreach (var record in includedRecords) {
            yield return new RecordPromotionChange(record, RecordPromotionChangeType.Deleted);
        }

        // References will relink to the promoted records
        var includedFormKeys = includedRecords.Select(x => x.FormKey).ToArray();
        foreach (var record in recordsToBePromoted) {
            var references = ReferenceService.GetRecordReferences(record).ToArray();
            if (references.Length == 0) continue;

            foreach (var reference in references) {
                if (includedFormKeys.Contains(reference.FormKey)) continue;
                if (!_editorEnvironment.LinkCache.TryResolveContext(reference, out var context)) continue;

                yield return new RecordPromotionChange(context.Record, RecordPromotionChangeType.Modified);
            }
        }
    }

    public IEnumerable<AssetPromotionChange> GetAffectedAssets(IReadOnlyCollection<IMajorRecordGetter> records) {
        if (AssetPromotionMode == AssetPromotionMode.Nothing) yield break;
        if (AssetTargets.Count != 1) yield break;

        var recordIdentifiers = records.Select(r => r.ToFormLinkInformation()).ToArray();
        var allReferencedAssets = records
            .SelectMany(r => r.EnumerateAssetLinks(AssetLinkQuery.Listed | AssetLinkQuery.Inferred))
            .ToArray();

        var allReferencedDataRelativePaths = allReferencedAssets
            .Select(assetLink => assetLink.DataRelativePath)
            .ToHashSet();

        foreach (var assetLink in allReferencedAssets) {
            var dataSource = AssetOrigins.FirstOrDefault(dataSource => dataSource.FileExists(assetLink.DataRelativePath));
            if (dataSource is null) continue;

            var dataSourceLink = new DataSourceFileLink(dataSource, assetLink.DataRelativePath);

            switch (AssetPromotionMode) {
                case AssetPromotionMode.Copy:
                    yield return new AssetPromotionChange(dataSourceLink, AssetPromotionChangeType.Copied);

                    break;
                case AssetPromotionMode.Move:
                    var allAssetRefsIncluded = ReferenceService
                        .GetAssetReferences(assetLink)
                        .All(path => allReferencedDataRelativePaths.Contains(path));

                    var allRecordRefsIncluded = ReferenceService
                        .GetRecordReferences(assetLink)
                        .All(recordIdentifiers.Contains);

                    if (allAssetRefsIncluded && allRecordRefsIncluded) {
                        yield return new AssetPromotionChange(dataSourceLink, AssetPromotionChangeType.Moved);
                    } else {
                        yield return new AssetPromotionChange(dataSourceLink, AssetPromotionChangeType.Copied);
                    }

                    break;
                case AssetPromotionMode.ForceMove:
                    yield return new AssetPromotionChange(dataSourceLink, AssetPromotionChangeType.Moved);

                    break;
            }
        }
    }
}

public sealed record RecordPromotionChange(IMajorRecordGetter Record, RecordPromotionChangeType ChangeType);
public sealed record AssetPromotionChange(DataSourceFileLink FileLink, AssetPromotionChangeType ChangeType);

public enum RecordPromotionChangeType {
    Modified,
    Deleted,
}

public enum AssetPromotionChangeType {
    Moved,
    Copied,
}

public enum AssetPromotionMode {
    Nothing,
    Copy,
    Move,
    ForceMove,
}
