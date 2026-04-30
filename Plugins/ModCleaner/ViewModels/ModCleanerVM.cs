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
using ModCleaner.Models;
using ModCleaner.Models.FeatureFlag;
using ModCleaner.Services;
using ModCleaner.Services.FeatureFlag;
using ModCleaner.Views;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
using FeatureFlagItem = ModCleaner.Models.FeatureFlag.FeatureFlagItem;
using ILinkIdentifier = ModCleaner.Models.ILinkIdentifier;
using Key = Avalonia.Input.Key;
namespace ModCleaner.ViewModels;

public sealed record ExteriorCell(IWorldspaceGetter Worldspace, ICellGetter Cell);

public sealed record FormLinkWithEditorID(FormLinkIdentifier Link, string? EditorID) : IFormLinkIdentifier {
    public FormKey FormKey => Link.FormLink.FormKey;
    public Type Type => Link.FormLink.Type;
}

public sealed partial class ModCleanerVM : ViewModel {
    private readonly MainWindow _mainWindow;
    private readonly ILogger _logger;
    private readonly Services.ModCleaner _modCleaner;
    private readonly IEssentialRecordProvider _essentialRecordProvider;

    private Graph<ILinkIdentifier, Edge<ILinkIdentifier>>? _originalReferenceGraphOnlyRetained;
    private HashSet<ILinkIdentifier>? _retainedLinks;

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
    public ReactiveCommand<Unit, Unit> BuildRetainedLinksCommand { get; }
    public ReactiveCommand<Unit, Unit> CleanCommand { get; }
    public ReactiveCommand<Unit, Unit> SearchForSelectedPathCommand { get; }
    public ReactiveCommand<IList, Unit> SearchForRecordsCommand { get; }
    public Func<FeatureFlag, FeatureFlagEditorVM> FeatureFlagEditorVMFactory { get; }

    public IDataSource? SelectedDataSource => CleanAssets ? CleaningDataSourcePicker.SelectedDataSource : null;
    public IObservable<bool> RequirementsMet { get; }
    public IObservable<bool> CanClean { get; }

    public ModCleanerVM(
        Func<FeatureFlag, FeatureFlagEditorVM> featureFlagEditorVMFactory,
        MainWindow mainWindow,
        ILogger logger,
        IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
        Services.ModCleaner modCleaner,
        IReferenceService referenceService,
        IEssentialRecordProvider essentialRecordProvider,
        SingleDataSourcePickerVM cleaningDataSourcePicker,
        SingleModPickerVM cleaningModPickerVM,
        MultiModPickerVM dependenciesModPickerVM,
        IFeatureFlagService featureFlagService) {
        _mainWindow = mainWindow;
        _logger = logger;
        _modCleaner = modCleaner;
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

        RequirementsMet = CleaningModPickerVM.HasModSelected
            .CombineLatest(FeatureFlagService.FeatureFlagsChanged, (a, _) => a && FeatureFlagService.FeatureFlags.Values.Any(x => x));

        CanClean = this.WhenAnyValue(x => x.CleanAssets)
            .CombineLatest(
                CleaningDataSourcePicker.HasDataSourceSelected,
                (cleanAssets, dataSourceSelected) => !cleanAssets || dataSourceSelected);

        BuildReferenceGraphCommand = ReactiveCommand.CreateRunInBackground(BuildReferenceGraph, RequirementsMet);
        BuildRetainedLinksCommand = ReactiveCommand.CreateRunInBackground(BuildRetainedLinks, RequirementsMet);
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
        var assetDialog = new TaskDialog {
            Title = $"Feature Flag {featureFlag.Name}",
            Content = new FeatureFlagEditor(flagEditorVM) {
                Width = 1200
            },
            XamlRoot = _mainWindow,
            Buttons = {
                new TaskDialogButton {
                    Text = "Save",
                    DialogResult = TaskDialogStandardResult.OK,
                },
                TaskDialogButton.CancelButton,
            },
            Classes = {
                "No"
            },
            Styles = {
                new Style(x => x.OfType<TaskDialog>().Class("No").Template().OfType<Border>().Name("ContentRoot")) {
                    Setters = {
                        new Setter(Layoutable.MaxWidthProperty, 1500.0),
                    },
                },
            },
            MinWidth = 1200,
            KeyBindings = {
                new KeyBinding {
                    Gesture = new KeyGesture(Key.Enter),
                    Command = TaskDialogButton.OKButton.Command,
                },
                new KeyBinding {
                    Gesture = new KeyGesture(Key.Escape),
                    Command = TaskDialogButton.CancelButton.Command,
                },
            },
        };

        if (await assetDialog.ShowAsync() is TaskDialogStandardResult.OK) {
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

    private void UpdateInvalidRecords(HashSet<ILinkIdentifier> retainedLinks) {
        if (!GetModAndDependencies(out var mod, out _)) return;

        // Checking if there is any exterior cell retained that shouldn't be retained
        var (invalidExteriorCells, invalidInteriorCells) = GetInvalidCells(retainedLinks, mod);

        var invalidQuests = GetInvalidQuests(retainedLinks, mod);

        var voiceTypesWithoutSounds = GetVoiceTypesWithoutSounds(retainedLinks, mod);

        // Retain dialog without voiced lines?

        Dispatcher.UIThread.Post(() => {
            InvalidExteriorCells = invalidExteriorCells;
            InteriorCells = invalidInteriorCells;
            InvalidQuests = invalidQuests;
            InvalidVoiceTypes = voiceTypesWithoutSounds;
        });
    }

    private void SearchForSelectedPath() {
        if (SourceLink is null || TargetLink is null) return;

        FindShortestPath(SourceLink.Link, TargetLink.Link);
    }

    private (HashSet<ExteriorCell> invalidExteriorCells, HashSet<ICellGetter> invalidInteriorCells) GetInvalidCells(
        HashSet<ILinkIdentifier> retainedLinks,
        ISkyrimModGetter mod) {
        var invalidExteriorCells = new HashSet<ExteriorCell>();
        var interiorCells = new HashSet<ICellGetter>();
        foreach (var linkIdentifier in retainedLinks) {
            if (linkIdentifier is not FormLinkIdentifier formLinkIdentifier) continue;
            if (formLinkIdentifier.FormLink.Type != typeof(ICellGetter)) continue;
            if (!EditorEnvironment.LinkCache.TryResolve<ICellGetter>(formLinkIdentifier.FormLink.FormKey, out var cell)) continue;
            if (cell.FormKey.ModKey != mod.ModKey) continue;

            var worldspace = cell.GetWorldspace(EditorEnvironment.LinkCache);
            if (worldspace is null || cell.Grid is null) {
                if (cell.GetExteriorDoorsGoingIntoInteriorRecursively(EditorEnvironment.LinkCache)
                    .All(placedContext => {
                        if (placedContext.Record.Placement is null) return true;

                        var cellCoordinates = placedContext.Record.Placement.GetCellCoordinates();
                        if (!placedContext.TryGetParent<IWorldspaceGetter>(out var worldspace)) return true;

                        var cell = worldspace.GetCell(cellCoordinates);
                        if (cell is null) return true;

                        return !retainedLinks.Contains(new FormLinkIdentifier(cell.ToFormLinkInformation()));
                    })) {
                    interiorCells.Add(cell);
                }
            } else if (_essentialRecordProvider.IsInvalidExteriorCell(worldspace.ToLinkGetter(), cell)) {
                var notInRangeOfValidCells = Enumerable.Range(-3, 7)
                    .SelectMany(dx => Enumerable.Range(-3, 7).Select(dy => (dx, dy)))
                    .Select(offset => worldspace.GetCell(new P2Int(cell.Grid.Point.X + offset.dx, cell.Grid.Point.Y + offset.dy)))
                    .WhereNotNull()
                    .All(neighborCell => _essentialRecordProvider.IsInvalidExteriorCell(worldspace.ToLinkGetter(), neighborCell));

                if (notInRangeOfValidCells) {
                    invalidExteriorCells.Add(new ExteriorCell(worldspace, cell));
                }
            }
        }

        return (invalidExteriorCells, interiorCells);
    }

    private HashSet<IQuestGetter> GetInvalidQuests(
        HashSet<ILinkIdentifier> retainedLinks,
        ISkyrimModGetter mod) {
        var invalidQuests = new HashSet<IQuestGetter>();
        foreach (var linkIdentifier in retainedLinks) {
            if (linkIdentifier is not FormLinkIdentifier formLinkIdentifier) continue;
            if (_essentialRecordProvider.IsEssentialRecord(formLinkIdentifier.FormLink)) continue;
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
        var directory = path.Combine(SkyrimSoundAssetType.Instance.BaseFolder, "Voice");
        if (!SelectedDataSource.DirectoryExists(directory)) return [];

        var voiceTypesWithSounds = SelectedDataSource
            .EnumerateDirectories(directory)
            .SelectMany(modPath => SelectedDataSource.EnumerateDirectories(modPath))
            .Select(voiceTypePath => path.GetFileName(voiceTypePath.Path))
            .Select(voiceType => EditorEnvironment.LinkCache.TryResolve<IVoiceTypeGetter>(voiceType, out var voiceTypeRecord)
                ? voiceTypeRecord
                : null)
            .WhereNotNull()
            .ToHashSet();

        return mod.EnumerateMajorRecords<IVoiceTypeGetter>()
            .Where(voiceType => !voiceTypesWithSounds.Contains(voiceType))
            .Where(voiceType => retainedLinks.Contains(new FormLinkIdentifier(voiceType.ToFormLinkInformation())))
            .ToHashSet();
    }

    private void FindShortestPath(ILinkIdentifier source, ILinkIdentifier target) {
        if (_originalReferenceGraphOnlyRetained is null) return;

        Dispatcher.UIThread.Post(() => IsBusy = true);

        var path = _originalReferenceGraphOnlyRetained.ShortestPath(source, target);

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
        var graph = _modCleaner.BuildGraph(mod, dependencies);
        Dispatcher.UIThread.Post(() => ReferenceGraph = graph);
        Dispatcher.UIThread.Post(() => IsBusy = false);
    }

    public void BuildRetainedLinks() {
        if (CleaningModPickerVM.SelectedMod is null) return;
        if (ReferenceGraph is null) return;
        if (!GetModAndDependencies(out var mod, out var dependencies)) return;

        Dispatcher.UIThread.Post(() => IsBusy = true);

        var (retainedLinks, dependencyGraph) = _modCleaner.FindRetainedRecords(ReferenceGraph, mod, dependencies, ExcludedLinks.ToHashSet());
        _retainedLinks = retainedLinks;

        var retainedRecords = retainedLinks.OfType<FormLinkIdentifier>()
            .Where(x => x.FormLink.FormKey.ModKey == mod.ModKey)
            .Select(link => EditorEnvironment.LinkCache.TryResolveIdentifier(link.FormLink, out var editorId) && editorId is not null
                ? new FormLinkWithEditorID(link, editorId) : null)
            .WhereNotNull()
            .OrderBy(r => r.EditorID)
            .ToList();

        var formKey = FormKey.Factory("0D49FD:BSHeartland.esm");
        if (retainedRecords.Any(x => x.FormKey == formKey)) {
            Console.WriteLine();
        }

        var referenceGraphOnlyRetained = new Graph<ILinkIdentifier, Edge<ILinkIdentifier>>();
        foreach (var link in retainedLinks) {
            if (ReferenceGraph.OutgoingEdges.TryGetValue(link, out var outgoing)) {
                foreach (var edge in outgoing) {
                    referenceGraphOnlyRetained.AddEdge(edge);
                }
            }
        }

        _originalReferenceGraphOnlyRetained = referenceGraphOnlyRetained;

        Dispatcher.UIThread.Post(() => {
            RetainedRecords = retainedRecords;
            DependencyGraph = dependencyGraph;
        });

        UpdateInvalidRecords(retainedLinks);

        Dispatcher.UIThread.Post(() => {
            IsBusy = false;
        });
    }

    private void Clean() {
        if (_retainedLinks is null) return;
        if (!GetModAndDependencies(out var mod, out _)) return;

        Dispatcher.UIThread.Post(() => IsBusy = true);
        _modCleaner.Clean(mod, _retainedLinks, SelectedDataSource);
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
