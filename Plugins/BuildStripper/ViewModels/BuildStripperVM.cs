using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;
using Avalonia.Threading;
using BuildStripper.Models;
using BuildStripper.Models.FeatureFlag;
using BuildStripper.Services;
using BuildStripper.Services.FeatureFlag;
using BuildStripper.Views;
using CreationEditor;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.DataSource;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Avalonia.Views;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim;
using DynamicData;
using DynamicData.Binding;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Mutagen.Bethesda.Skyrim.Records.Assets.VoiceType;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
using FeatureFlagItem = BuildStripper.Models.FeatureFlag.FeatureFlagItem;
using ILinkIdentifier = BuildStripper.Models.ILinkIdentifier;
using Key = Avalonia.Input.Key;
namespace BuildStripper.ViewModels;

public sealed record ExteriorCell(IWorldspaceGetter Worldspace, ICellGetter Cell);

public sealed record FormLinkWithEditorID(FormLinkIdentifier Link, string? EditorID) : IFormLinkIdentifier {
    public FormKey FormKey => Link.FormLink.FormKey;
    public Type Type => Link.FormLink.Type;
}

public sealed partial class BuildStripperVM : ViewModel {
    private readonly MainWindow _mainWindow;
    private readonly ILogger _logger;
    private readonly Services.BuildStripper _buildStripper;
    private readonly IEssentialRecordProvider _essentialRecordProvider;

    private Graph<ILinkIdentifier, Edge<ILinkIdentifier>>? _filteredGraph;
    private HashSet<ILinkIdentifier>? _retainedLinks;
    private IReadOnlyDictionary<IFormLinkIdentifier, Action<IMajorRecord>>? _postProcessSteps;

    public IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> EditorEnvironment { get; }
    public IReferenceService ReferenceService { get; }
    public SingleDataSourcePickerVM CleaningDataSourcePicker { get; }
    public SingleModPickerVM CleaningModPickerVM { get; }
    public MultiModPickerVM DependenciesModPickerVM { get; }
    public IFeatureFlagService FeatureFlagService { get; }

    public ReadOnlyObservableCollection<FeatureFlagItem> FeatureFlags { get; }
    public IObservableCollection<ILinkIdentifier> ExcludedLinks { get; } = new ObservableCollectionExtended<ILinkIdentifier>();

    [Reactive] public partial Graph<ILinkIdentifier, Edge<ILinkIdentifier>>? ReferenceGraph { get; set; }
    [Reactive] public partial Graph<ILinkIdentifier, Edge<ILinkIdentifier>>? DependencyGraph { get; set; }
    [Reactive] public partial List<FormLinkWithEditorID>? RetainedRecords { get; set; }
    [Reactive] public partial HashSet<ExteriorCell>? InvalidExteriorCells { get; set; }
    [Reactive] public partial HashSet<ICellGetter>? InteriorCells { get; set; }
    [Reactive] public partial HashSet<IQuestGetter>? InvalidQuests { get; set; }
    [Reactive] public partial HashSet<IVoiceTypeGetter>? InvalidVoiceTypes { get; set; }
    [Reactive] public partial List<ILinkIdentifier>? Path { get; set; }
    [Reactive] public partial FormLinkWithEditorID? SourceLink { get; set; }
    [Reactive] public partial FormLinkWithEditorID? TargetLink { get; set; }
    [Reactive] public partial bool CleanAssets { get; set; }

    [Reactive] public partial bool IsBusy { get; set; }

    public ReactiveCommand<Unit, Unit> BuildReferenceGraphCommand { get; }
    public ReactiveCommand<Unit, Task> BuildRetainedLinksCommand { get; }
    public ReactiveCommand<Unit, Unit> CleanCommand { get; }
    public ReactiveCommand<Unit, Unit> SearchForSelectedPathCommand { get; }
    public ReactiveCommand<IList, Unit> SearchForRecordsCommand { get; }
    public Func<FeatureFlag, FeatureFlagEditorVM> FeatureFlagEditorVMFactory { get; }

    public IDataSource? SelectedDataSource => CleanAssets ? CleaningDataSourcePicker.SelectedDataSource : null;
    public IObservable<bool> ReadyForProcessing { get; }
    public IObservable<bool> CanClean { get; }

    public BuildStripperVM(
        Func<FeatureFlag, FeatureFlagEditorVM> featureFlagEditorVMFactory,
        MainWindow mainWindow,
        ILogger logger,
        IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
        Services.BuildStripper buildStripper,
        IReferenceService referenceService,
        IEssentialRecordProvider essentialRecordProvider,
        SingleDataSourcePickerVM cleaningDataSourcePicker,
        SingleModPickerVM cleaningModPickerVM,
        MultiModPickerVM dependenciesModPickerVM,
        IFeatureFlagService featureFlagService) {
        _mainWindow = mainWindow;
        _logger = logger;
        _buildStripper = buildStripper;
        _essentialRecordProvider = essentialRecordProvider;
        EditorEnvironment = editorEnvironment;
        ReferenceService = referenceService;
        CleaningModPickerVM = cleaningModPickerVM;
        DependenciesModPickerVM = dependenciesModPickerVM;
        FeatureFlagEditorVMFactory = featureFlagEditorVMFactory;
        FeatureFlagService = featureFlagService;
        CleaningDataSourcePicker = cleaningDataSourcePicker;
        CleaningDataSourcePicker.Filter = dataSource => !dataSource.IsReadOnly;

        FeatureFlags = featureFlagService.FeatureFlagsChanged
            .Select(_ => featureFlagService.FeatureFlags.Select(kv => new FeatureFlagItem(kv.Key, kv.Value)).AsObservableChangeSet())
            .Switch()
            .ToObservableCollection(this);

        FeatureFlags
            .ToObservableChangeSet()
            .AutoRefresh(x => x.IsSelected)
            .ToCollection()
            .Subscribe(OnFeatureFlagsChanged)
            .DisposeWith(this);

        DependenciesModPickerVM.Filter = _ => false;
        CleaningModPickerVM.SelectedModChanged
            .Subscribe(OnCleaningModSelected)
            .DisposeWith(this);

        ReadyForProcessing = CleaningModPickerVM.HasModSelected
            .CombineLatest(FeatureFlagService.FeatureFlagsChanged, (a, _) => a && FeatureFlagService.FeatureFlags.Values.Any(x => x));

        CanClean = this.WhenAnyValue(x => x.CleanAssets)
            .CombineLatest(
                CleaningDataSourcePicker.HasDataSourceSelected,
                (cleanAssets, dataSourceSelected) => !cleanAssets || dataSourceSelected);

        BuildReferenceGraphCommand = ReactiveCommand.CreateRunInBackground(BuildReferenceGraph, ReadyForProcessing);
        BuildRetainedLinksCommand = ReactiveCommand.CreateRunInBackground(BuildRetainedLinks, ReadyForProcessing);
        CleanCommand = ReactiveCommand.CreateRunInBackground(Clean, CanClean);
        SearchForSelectedPathCommand = ReactiveCommand.CreateRunInBackground(SearchForSelectedPath);
        SearchForRecordsCommand = ReactiveCommand.CreateRunInBackground<IList>(SearchForRecords);
    }

    private void SearchForRecords(IList parameter) {
        if (parameter is not [FormLinkIdentifier source, IMajorRecordGetter targetRecord]) return;

        if (!EditorEnvironment.LinkCache.TryResolveIdentifier(source.FormLink, out var editorId)) return;

        var target = new FormLinkIdentifier(targetRecord.ToStandardizedIdentifier());

        Dispatcher.UIThread.Post(() => {
            SourceLink = new FormLinkWithEditorID(source, editorId);
            TargetLink = new FormLinkWithEditorID(target, targetRecord.EditorID);
        });

        FindShortestPath(source, target);
    }

    [ReactiveCommand]
    private async Task EditFeatureFlag(FeatureFlag featureFlag) {
        var flagEditorVM = FeatureFlagEditorVMFactory(featureFlag);
        var assetDialog = new FATaskDialog {
            Title = $"Feature Flag {featureFlag.Name}",
            Content = new FeatureFlagEditor(flagEditorVM) {
                Width = 1200
            },
            XamlRoot = _mainWindow,
            Buttons = {
                new FATaskDialogButton {
                    Text = "Save",
                    DialogResult = FATaskDialogStandardResult.OK,
                },
                FATaskDialogButton.CancelButton,
            },
            Classes = {
                "No"
            },
            Styles = {
                new Style(x => x.OfType<FATaskDialog>().Class("No").Template().OfType<Border>().Name("ContentRoot")) {
                    Setters = {
                        new Setter(Layoutable.MaxWidthProperty, 1500.0),
                    },
                },
            },
            MinWidth = 1200,
            KeyBindings = {
                new KeyBinding {
                    Gesture = new KeyGesture(Key.Enter),
                    Command = FATaskDialogButton.OKButton.Command,
                },
                new KeyBinding {
                    Gesture = new KeyGesture(Key.Escape),
                    Command = FATaskDialogButton.CancelButton.Command,
                },
            },
        };

        if (await assetDialog.ShowAsync() is FATaskDialogStandardResult.OK) {
            FeatureFlagService.RemoveFeatureFlag(featureFlag);
            FeatureFlagService.AddFeatureFlag(flagEditorVM.GetFeatureFlag());
        }
    }

    [ReactiveCommand]
    private void AddFeatureFlag() {
        FeatureFlagService.AddFeatureFlag(new FeatureFlag("NewFeatureFlag",
            CleaningModPickerVM.SelectedMod?.ModKey ?? ModKey.Null,
            [],
            []));
    }

    [ReactiveCommand]
    private void DeleteFeatureFlags(object? o) {
        if (o is not IList removeList) return;

        foreach (var featureFlag in removeList.OfType<FeatureFlagItem>()) {
            FeatureFlagService.RemoveFeatureFlag(featureFlag.FeatureFlag);
        }
    }

    [ReactiveCommand]
    private void RetainRecord(FormLinkIdentifier link) {
        ExcludedLinks.Remove(link);
    }

    [ReactiveCommand]
    private void ExcludeRecord(FormLinkIdentifier link) {
        if (!ExcludedLinks.Contains(link)) {
            ExcludedLinks.Add(link);
        }
    }

    private async Task UpdateInvalidRecords(HashSet<ILinkIdentifier> retainedLinks) {
        if (!GetModAndDependencies(out var mod, out _)) return;

        // Checking if there is any exterior cell retained that shouldn't be retained
        var invalidCellsTask = Task.Run(() => GetInvalidCells(retainedLinks, mod));
        var invalidQuestsTask = Task.Run(() => GetInvalidQuests(retainedLinks, mod));
        var voiceTypesWithoutSoundsTask = Task.Run(() => GetVoiceTypesWithoutSounds(retainedLinks, mod));

        await Task.WhenAll(invalidCellsTask, invalidQuestsTask, voiceTypesWithoutSoundsTask);

        Dispatcher.UIThread.Post(() => {
            InvalidExteriorCells = invalidCellsTask.Result.invalidExteriorCells;
            InteriorCells = invalidCellsTask.Result.invalidInteriorCells;
            InvalidQuests = invalidQuestsTask.Result;
            InvalidVoiceTypes = voiceTypesWithoutSoundsTask.Result;
        });
    }

    private void SearchForSelectedPath() {
        if (SourceLink is null || TargetLink is null) return;

        FindShortestPath(SourceLink.Link, TargetLink.Link);
    }

    private (HashSet<ExteriorCell> invalidExteriorCells, HashSet<ICellGetter> invalidInteriorCells) GetInvalidCells(
        HashSet<ILinkIdentifier> retainedLinks,
        ISkyrimModGetter mod) {
        var retainedExteriorCells = _essentialRecordProvider.EnumerateRetainedExteriorCells(mod.ModKey, EditorEnvironment.LinkCache);
        var invalidExteriorCells = new HashSet<ExteriorCell>();
        var invalidInteriorCells = new HashSet<ICellGetter>();
        foreach (var linkIdentifier in retainedLinks) {
            if (linkIdentifier is not FormLinkIdentifier formLinkIdentifier) continue;
            if (formLinkIdentifier.FormLink.Type != typeof(ICellGetter)) continue;
            if (!EditorEnvironment.LinkCache.TryResolve<ICellGetter>(formLinkIdentifier.FormLink.FormKey, out var cell)) continue;
            if (cell.FormKey.ModKey != mod.ModKey) continue;
            if (_essentialRecordProvider.IsEssentialRecord(mod.ModKey, formLinkIdentifier.FormLink)) continue;

            var worldspace = cell.GetWorldspace(EditorEnvironment.LinkCache);
            if (worldspace is null || cell.Grid is null) {
                // For an interior cell, check if it has any exterior doors going into interior cells that are not retained
                if (cell.GetExteriorDoorsGoingIntoInteriorRecursively(EditorEnvironment.LinkCache)
                    .All(placedContext => {
                        if (placedContext.Record.Placement is null) return true;

                        var cellCoordinates = placedContext.Record.Placement.GetCellCoordinates();
                        if (!placedContext.TryGetParent<IWorldspaceGetter>(out var w)) return true;

                        var c = w.GetCell(cellCoordinates);
                        if (c is null) return true;

                        return !retainedLinks.Contains(new FormLinkIdentifier(c.ToFormLinkInformation()));
                    })) {
                    invalidInteriorCells.Add(cell);
                }
            } else {
                // For an exterior cell, check if it within the range of valid cells for the worldspace, if not, add it to the invalid exterior cells
                retainedExteriorCells.TryGetValue(worldspace.FormKey, out var validCellsForWorldspace);
                if (validCellsForWorldspace is not null && validCellsForWorldspace.Any(x => x.Cell.FormKey == cell.FormKey)) continue;

                invalidExteriorCells.Add(new ExteriorCell(worldspace, cell));
            }
        }

        return (invalidExteriorCells, invalidInteriorCells);
    }

    private HashSet<IQuestGetter> GetInvalidQuests(
        HashSet<ILinkIdentifier> retainedLinks,
        ISkyrimModGetter mod) {
        var invalidQuests = new HashSet<IQuestGetter>();
        foreach (var linkIdentifier in retainedLinks) {
            if (linkIdentifier is not FormLinkIdentifier formLinkIdentifier) continue;
            if (_essentialRecordProvider.IsEssentialRecord(mod.ModKey, formLinkIdentifier.FormLink)) continue;
            if (formLinkIdentifier.FormLink.FormKey.ModKey != mod.ModKey) continue;
            if (formLinkIdentifier.FormLink.Type != typeof(IQuestGetter)) continue;
            if (!EditorEnvironment.LinkCache.TryResolve<IQuestGetter>(formLinkIdentifier.FormLink.FormKey, out var quest)) continue;

            invalidQuests.Add(quest);
        }

        return invalidQuests;
    }

    private HashSet<IVoiceTypeGetter> GetVoiceTypesWithoutSounds(HashSet<ILinkIdentifier> retainedLinks, ISkyrimModGetter mod) {
        if (SelectedDataSource is null) return [];

        var path = SelectedDataSource.FileSystem.Path;
        var voiceDirectory = path.Combine(SkyrimSoundAssetType.Instance.BaseFolder, "Voice");
        if (!SelectedDataSource.DirectoryExists(voiceDirectory)) return [];

        var voiceTypesWithSounds = SelectedDataSource
            .EnumerateDirectories(voiceDirectory)
            .SelectMany(modPath => SelectedDataSource.EnumerateDirectories(modPath.DataRelativePath))
            .Select(voiceTypePath => voiceTypePath.Name)
            .Select(voiceType => EditorEnvironment.LinkCache.TryResolve<IVoiceTypeGetter>(voiceType, out var voiceTypeRecord)
                ? voiceTypeRecord
                : null)
            .WhereNotNull()
            .ToHashSet();

        var assetLinkCache = EditorEnvironment.LinkCache.CreateImmutableAssetLinkCache();
        var voiceTypeAssetLookup = assetLinkCache.GetComponent<VoiceTypeAssetLookup>();
        var voiceTypes = mod.EnumerateMajorRecords<IDialogResponsesGetter>()
            .SelectMany(voiceTypeAssetLookup.GetSpeakers)
            .Select(x => x.TryResolve(EditorEnvironment.LinkCache, out var voiceType) ? voiceType : null)
            .WhereNotNull()
            .Select(x => x.Voice)
            .DistinctBy(x => x.FormKey)
            .Select(x => x.TryResolve(EditorEnvironment.LinkCache, out var voiceType) ? voiceType : null)
            .WhereNotNull()
            .ToHashSet();

        return mod.EnumerateMajorRecords<IVoiceTypeGetter>()
            // Only report voice types where no sounds are recorded
            .Where(voiceType => !voiceTypesWithSounds.Contains(voiceType))
            // Don't report voice types that are not retained
            .Where(voiceType => retainedLinks.Contains(new FormLinkIdentifier(voiceType.ToFormLinkInformation())))
            // Ensure that the voice type actually has lines that need to be voiced and don't always use sounds instead
            .Where(voiceTypes.Contains)
            .ToHashSet();
    }

    private void FindShortestPath(ILinkIdentifier source, ILinkIdentifier target) {
        if (_filteredGraph is null) return;

        Dispatcher.UIThread.Post(() => IsBusy = true);

        var path = _filteredGraph.ShortestPath(source, target);

        Dispatcher.UIThread.Post(() => {
            Path = path;
            IsBusy = false;
        });
    }

    private bool GetModAndDependencies([MaybeNullWhen(false)] out ISkyrimModGetter mod, [MaybeNullWhen(false)] out List<ModKey> dependencies) {
        if (CleaningModPickerVM.SelectedMod is null) {
            mod = null;
            dependencies = null;
            return false;
        }

        mod = EditorEnvironment.ResolveMod(CleaningModPickerVM.SelectedMod.ModKey);
        if (mod is null) {
            _logger.Error("{Mod} not found in load order", CleaningModPickerVM.SelectedMod.ModKey);
            dependencies = null;
            return false;
        }

        dependencies = DependenciesModPickerVM.Mods.Select(x => x.ModKey).ToList();
        return true;
    }

    private void BuildReferenceGraph() {
        if (!GetModAndDependencies(out var mod, out var dependencies)) return;

        Dispatcher.UIThread.Post(() => IsBusy = true);
        var graph = _buildStripper.BuildGraph(SelectedDataSource, mod, dependencies);
        Dispatcher.UIThread.Post(() => ReferenceGraph = graph);
        Dispatcher.UIThread.Post(() => IsBusy = false);
    }

    public async Task BuildRetainedLinks() {
        if (CleaningModPickerVM.SelectedMod is null) return;
        if (ReferenceGraph is null) return;
        if (!GetModAndDependencies(out var mod, out var dependencies)) return;

        Dispatcher.UIThread.Post(() => IsBusy = true);

        var excludedQuests = mod.Quests
            .Select(q => q.ToFormLinkInformation())
            .Where(q => !_essentialRecordProvider.IsEssentialRecord(mod.ModKey, q))
            .Select(q => new FormLinkIdentifier(q));
        var allExcluded = ExcludedLinks.Concat(excludedQuests);
        var (filteredGraph, postProcessSteps) = _buildStripper.FindRetainedRecords(_essentialRecordProvider, ReferenceGraph, SelectedDataSource, mod, dependencies, allExcluded.ToHashSet());
        _filteredGraph = filteredGraph.Build();
        _retainedLinks = filteredGraph.Build().Vertices.ToHashSet();
        var dependencyGraph = filteredGraph.BuildDependencyGraph();
        _postProcessSteps = postProcessSteps;

        var retainedRecords = _retainedLinks.OfType<FormLinkIdentifier>()
            .Where(x => x.FormLink.FormKey.ModKey == mod.ModKey)
            .Select(link => EditorEnvironment.LinkCache.TryResolveIdentifier(link.FormLink, out var editorId) && editorId is not null
                ? new FormLinkWithEditorID(link, editorId) : null)
            .WhereNotNull()
            .OrderBy(r => r.EditorID)
            .ToList();

        await UpdateInvalidRecords(_retainedLinks);

        Dispatcher.UIThread.Post(() => {
            RetainedRecords = retainedRecords;
            DependencyGraph = dependencyGraph;
            IsBusy = false;
        });
    }

    private void Clean() {
        if (_retainedLinks is null) return;
        if (!GetModAndDependencies(out var mod, out _)) return;

        Dispatcher.UIThread.Post(() => IsBusy = true);
        _buildStripper.Clean(mod, _retainedLinks, SelectedDataSource, _postProcessSteps ?? new Dictionary<IFormLinkIdentifier, Action<IMajorRecord>>());
        Dispatcher.UIThread.Post(() => IsBusy = false);
    }

    private void OnFeatureFlagsChanged(IReadOnlyCollection<FeatureFlagItem> flags) {
        foreach (var flag in flags) {
            FeatureFlagService.SetFeatureEnabled(flag.FeatureFlag, flag.IsSelected);
        }
    }

    private void OnCleaningModSelected(OrderedModItem? cleanMod) {
        if (cleanMod is null) {
            DependenciesModPickerVM.Filter = _ => false;
            return;
        }

        DependenciesModPickerVM.Filter = dependency => EditorEnvironment.Environment.ResolveMod(dependency.ModKey)?
            .ModHeader.MasterReferences.Any(m => cleanMod.ModKey == m.Master) is true;

        // Set all dependencies to selected by default
        foreach (var modItem in DependenciesModPickerVM.Mods) {
            modItem.IsSelected = true;
        }
    }
}
