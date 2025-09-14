using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Threading;
using CreationEditor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.DataSource;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim;
using ModCleaner.Models;
using ModCleaner.Services;
using Mutagen.Bethesda;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
using ILinkIdentifier = ModCleaner.Models.ILinkIdentifier;
namespace ModCleaner.ViewModels;

public sealed record ExteriorCell(IWorldspaceGetter Worldspace, ICellGetter Cell);

public sealed partial class ModCleanerVM : ViewModel {
    private readonly ILogger _logger;

    public IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> EditorEnvironment { get; }
    public IReferenceService ReferenceService { get; }
    public SingleDataSourcePickerVM CleaningDataSourcePicker { get; }
    public SingleModPickerVM CleaningModPickerVM { get; }
    public MultiModPickerVM DependenciesModPickerVM { get; }

    [Reactive] public partial Graph<ILinkIdentifier, Edge<ILinkIdentifier>>? ReferenceGraph { get; set; }
    [Reactive] public partial Graph<ILinkIdentifier, Edge<ILinkIdentifier>>? DependencyGraph { get; set; }
    [Reactive] public partial List<IMajorRecordGetter>? RetainedRecords { get; set; }
    [Reactive] public partial HashSet<ILinkIdentifier>? RetainedLinks { get; set; }
    [Reactive] public partial HashSet<ExteriorCell>? InvalidExteriorCells { get; set; }
    [Reactive] public partial HashSet<IQuestGetter>? InvalidQuests { get; set; }
    [Reactive] public partial List<ILinkIdentifier>? Path { get; set; }
    [Reactive] public partial IMajorRecordGetter? SourceLink { get; set; }
    [Reactive] public partial IMajorRecordGetter? TargetLink { get; set; }

    [Reactive] public partial bool IsBusy { get; set; }

    public IObservable<bool> CanRun => CleaningModPickerVM.HasModSelected
        .CombineLatest(CleaningDataSourcePicker.HasDataSourceSelected, (a, b) => a && b);

    public ReactiveCommand<Unit, Unit> Run { get; }
    public ReactiveCommand<Unit, Unit> BuildRetainedLinks { get; }
    public ReactiveCommand<Unit, Unit> CleanMod { get; }
    public ReactiveCommand<IList, Unit> FindPath { get; }
    public ReactiveCommand<IList, Unit> SetLinks { get; }

    public ModCleanerVM(
        ILogger logger,
        IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
        Services.ModCleaner modCleaner,
        IReferenceService referenceService,
        IEssentialRecordProvider essentialRecordProvider,
        SingleDataSourcePickerVM cleaningDataSourcePicker,
        SingleModPickerVM cleaningModPickerVM,
        MultiModPickerVM dependenciesModPickerVM) {
        _logger = logger;
        EditorEnvironment = editorEnvironment;
        ReferenceService = referenceService;
        CleaningModPickerVM = cleaningModPickerVM;
        DependenciesModPickerVM = dependenciesModPickerVM;
        CleaningDataSourcePicker = cleaningDataSourcePicker;
        CleaningDataSourcePicker.Filter = dataSource => !dataSource.IsReadOnly;

        DependenciesModPickerVM.Filter = _ => false;
        CleaningModPickerVM.SelectedModChanged
            .Subscribe(cleanMod => {
                if (cleanMod is null) {
                    DependenciesModPickerVM.Filter = _ => false;
                    return;
                }

                DependenciesModPickerVM.Filter = dependency => editorEnvironment.Environment.ResolveMod(dependency.ModKey)?
                    .ModHeader.MasterReferences.Any(m => cleanMod.ModKey == m.Master) is true;

                // Set all dependencies to selected by default
                foreach (var modItem in DependenciesModPickerVM.Mods) {
                    modItem.IsSelected = true;
                }
            })
            .DisposeWith(this);

        Run = ReactiveCommand.CreateRunInBackground(() => {
            if (!GetModAndDependencies(out var mod, out var dependencies)) return;

            Dispatcher.UIThread.Post(() => IsBusy = true);
            var graph = modCleaner.BuildGraph(mod, dependencies);
            Dispatcher.UIThread.Post(() => ReferenceGraph = graph);
            Dispatcher.UIThread.Post(() => IsBusy = false);
        });

        BuildRetainedLinks = ReactiveCommand.CreateRunInBackground(() => {
            if (CleaningModPickerVM.SelectedMod is null || CleaningDataSourcePicker.SelectedDataSource is null) return;
            if (ReferenceGraph is null) return;
            if (!GetModAndDependencies(out var mod, out var dependencies)) return;

            Dispatcher.UIThread.Post(() => IsBusy = true);

            var (includedLinks, dependencyGraph) = modCleaner.FindRetainedRecords(ReferenceGraph, mod, dependencies);
            var includedRecords = includedLinks
                .OfType<FormLinkIdentifier>()
                .Select(x => x.FormLink)
                .Where(x => x.FormKey.ModKey == mod.ModKey)
                .Select(link => editorEnvironment.LinkCache.TryResolve(link, out var record) ? record : null)
                .WhereNotNull()
                .Where(r => r.EditorID is not null)
                .OrderBy(r => r.EditorID)
                .ToList();

            Dispatcher.UIThread.Post(() => {
                RetainedLinks = includedLinks;
                RetainedRecords = includedRecords;
            });

            var counts = new Dictionary<string, int>();
            using (var edges = new StreamWriter(@"C:\Users\nickp\Downloads\edges-test2.csv")) {
                foreach (var link in includedLinks) {
                    if (link is not FormLinkIdentifier l) continue;
                    if (!ReferenceGraph.OutgoingEdges.TryGetValue(link, out var outgoing)) continue;

                    foreach (var edge in outgoing) {
                        if (includedLinks.Contains(edge.Target) && edge.Target is FormLinkIdentifier t) {
                            // Remove links that are irrelevant for understanding the links and where they come from
                            if (l.FormLink.FormKey.ModKey == Skyrim.ModKey || t.FormLink.FormKey.ModKey == Skyrim.ModKey) continue;
                            if (l.FormLink.FormKey.ModKey == Update.ModKey || t.FormLink.FormKey.ModKey == Update.ModKey) continue;
                            if (l.FormLink.FormKey.ModKey == Dawnguard.ModKey || t.FormLink.FormKey.ModKey == Dawnguard.ModKey) continue;
                            if (l.FormLink.FormKey.ModKey == HearthFires.ModKey || t.FormLink.FormKey.ModKey == HearthFires.ModKey) continue;
                            if (l.FormLink.FormKey.ModKey == Dragonborn.ModKey || t.FormLink.FormKey.ModKey == Dragonborn.ModKey) continue;
                            if (l.FormLink.Type.Name == "ICellGetter" && t.FormLink.Type.Name.StartsWith("IPlaced")) continue;
                            if (l.FormLink.Type.Name == "IArmorAddonGetter" && t.FormLink.Type.Name.StartsWith("IRaceGetter")) continue;
                            if (l.FormLink.Type.Name == "INpcGetter" && t.FormLink.Type.Name.StartsWith("IHeadPartGetter")) continue;
                            if (l.FormLink.Type.Name == "IDialogTopicGetter" && t.FormLink.Type.Name.StartsWith("IDialogBranchGetter")) continue;

                            edges.WriteLine($"{l.FormLink.FormKey} - {l.FormLink.Type.Name},{t.FormLink.FormKey} - {t.FormLink.Type.Name}");

                            var typeName = l.FormLink.Type.Name + " - " + t.FormLink.Type.Name;
                            counts[typeName] = counts.GetOrDefault(typeName) + 1;
                        }
                    }
                }
            }
            using (var edges = new StreamWriter(@"C:\Users\nickp\Downloads\dependency-graph.csv")) {
                foreach (var (vertex, origins) in dependencyGraph.IncomingEdges) {
                    edges.WriteLine($"{vertex} <- {string.Join(", ", origins.Select(x => x.Source.ToString()))}");
                }
            }
            foreach (var (key, value) in counts.OrderByDescending(x => x.Value)) {
                Console.WriteLine(key + ": " + value);
            }

            // Checking if there is any exterior cell included that shouldn't be included
            var invalidExteriorCells = new HashSet<ExteriorCell>();
            foreach (var linkIdentifier in includedLinks) {
                if (linkIdentifier is not FormLinkIdentifier formLinkIdentifier) continue;
                if (formLinkIdentifier.FormLink.Type != typeof(ICellGetter)) continue;
                if (!editorEnvironment.LinkCache.TryResolve<ICellGetter>(formLinkIdentifier.FormLink.FormKey, out var cell)) continue;

                var worldspace = cell.GetWorldspace(editorEnvironment.LinkCache);
                if (worldspace is null || cell.Grid is null) continue;

                if (essentialRecordProvider.IsInvalidExteriorCell(worldspace.ToLinkGetter(), cell)) {
                    invalidExteriorCells.Add(new ExteriorCell(worldspace, cell));
                }
            }

            var invalidQuests = new HashSet<IQuestGetter>();
            var essentialRecords = essentialRecordProvider.GetEssentialRecords().ToHashSet();
            foreach (var linkIdentifier in includedLinks) {
                if (linkIdentifier is not FormLinkIdentifier formLinkIdentifier) continue;
                if (formLinkIdentifier.FormLink.FormKey.ModKey != mod.ModKey) continue;
                if (formLinkIdentifier.FormLink.Type != typeof(IQuestGetter)) continue;
                if (essentialRecords.Contains(formLinkIdentifier.FormLink)) continue;
                if (!editorEnvironment.LinkCache.TryResolve<IQuestGetter>(formLinkIdentifier.FormLink.FormKey, out var quest)) continue;

                invalidQuests.Add(quest);
            }

            Dispatcher.UIThread.Post(() => {
                InvalidExteriorCells = invalidExteriorCells;
                InvalidQuests = invalidQuests;
                DependencyGraph = dependencyGraph;
                IsBusy = false;
            });
        });

        CleanMod = ReactiveCommand.CreateRunInBackground(() => {
            if (ReferenceGraph is null || RetainedLinks is null) return;
            if (CleaningDataSourcePicker.SelectedDataSource is null) return;
            if (!GetModAndDependencies(out var mod, out _)) return;

            Dispatcher.UIThread.Post(() => IsBusy = true);
            modCleaner.Clean(mod, RetainedLinks, CleaningDataSourcePicker.SelectedDataSource);
            Dispatcher.UIThread.Post(() => IsBusy = false);
        });

        FindPath = ReactiveCommand.CreateRunInBackground<IList>(parameter => {
            if (ReferenceGraph is null) return;
            if (parameter is not [IMajorRecordGetter source, IMajorRecordGetter target]) return;

            Dispatcher.UIThread.Post(() => IsBusy = true);

            var sourceLink = new FormLinkIdentifier(source.ToStandardizedIdentifier());
            var targetLink = new FormLinkIdentifier(target.ToStandardizedIdentifier());
            var path = ReferenceGraph.CalculateShortestPath(sourceLink, targetLink);

            Dispatcher.UIThread.Post(() => {
                if (path is null || path.Count == 0) {
                    Path = null;
                } else {
                    Path = path.Select(x => x.Item1).ToList();
                }

                IsBusy = false;
            });
        }, CanRun);

        SetLinks = ReactiveCommand.CreateRunInBackground<IList>(parameter => {
            if (parameter is not [FormLinkIdentifier source, IMajorRecordGetter target]) return;

            Dispatcher.UIThread.Post(() => {
                SourceLink = editorEnvironment.LinkCache.TryResolve(source.FormLink, out var sourceRecord)
                    ? sourceRecord
                    : null;
                TargetLink = target;
            });
        }, CanRun);
    }

    private bool GetModAndDependencies([MaybeNullWhen(false)] out ISkyrimModGetter mod, [MaybeNullWhen(false)] out List<ModKey> dependencies) {
        if (CleaningModPickerVM.SelectedMod is null || CleaningDataSourcePicker.SelectedDataSource is null) {
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
}
