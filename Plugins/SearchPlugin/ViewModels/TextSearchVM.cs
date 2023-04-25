using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Threading;
using CreationEditor;
using CreationEditor.Avalonia.Models.GroupCollection;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.Plugin;
using DynamicData;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SearchPlugin.Models;
namespace SearchPlugin.ViewModels;

public sealed class TextSearchVM : ViewModel {
    private readonly PluginContext<ISkyrimMod, ISkyrimModGetter> _pluginContext;
    public ObservableCollection<RecordReferences<ISkyrimMod, ISkyrimModGetter>> References { get; } = new();

    public IList<SelectableSearcher> Searchers { get; }

    public GroupCollection<RecordReferences<ISkyrimMod, ISkyrimModGetter>> GroupCollection { get; }
    public Group<RecordReferences<ISkyrimMod, ISkyrimModGetter>> TypeGroup { get; }
    public Group<RecordReferences<ISkyrimMod, ISkyrimModGetter>> RecordGroup { get; }
    public HierarchicalTreeDataGridSource<object> TreeStructureSource { get; }

    public ReadOnlyObservableCollection<ModItem> Mods { get; }

    public ReactiveCommand<Unit, Unit> SearchCommand { get; }

    [Reactive] public string SearchText { get; set; } = string.Empty;
    [Reactive] public string ReplaceText { get; set; } = string.Empty;
    [Reactive] public bool Replace { get; set; }
    [Reactive] public bool CaseSensitive { get; set; }
    [Reactive] public bool IsBusy { get; set; }

    public StringComparison ComparisonType => CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

    public TextSearchVM(PluginContext<ISkyrimMod, ISkyrimModGetter> pluginContext) {
        _pluginContext = pluginContext;

        Searchers = new ObservableCollection<SelectableSearcher>(
            typeof(ITextSearcher<ISkyrimMod, ISkyrimModGetter>)
                .GetAllSubClass<ITextSearcher<ISkyrimMod, ISkyrimModGetter>>()
                .Select(searcher => new SelectableSearcher(searcher)));

        Mods = this.WhenAnyValue(x => x._pluginContext.EditorEnvironment.LinkCacheChanged)
            .Switch()
            .Select(x => x.ListedOrder.AsObservableChangeSet())
            .Switch()
            .Transform(mod => new ModItem(mod.ModKey) { IsSelected = true })
            .ToObservableCollection(this);

        TypeGroup = new Group<RecordReferences<ISkyrimMod, ISkyrimModGetter>>(references => references.TextSearcher, true);
        RecordGroup = new Group<RecordReferences<ISkyrimMod, ISkyrimModGetter>>(references => references.Record, true);
        GroupCollection = new GroupCollection<RecordReferences<ISkyrimMod, ISkyrimModGetter>>(References, TypeGroup, RecordGroup);

        SearchCommand = ReactiveCommand.CreateFromTask(async () => {
                if (SearchText.IsNullOrWhitespace()) return;

                Dispatcher.UIThread.Post(() => IsBusy = true);

                References.Clear();

                var selectedModKeys = Mods
                    .Where(mod => mod.IsSelected)
                    .Select(mod => mod.ModKey);

                var selectedMods = _pluginContext.EditorEnvironment.LinkCache.PriorityOrder
                    .Where(mod => selectedModKeys.Contains(mod.ModKey))
                    .ToList();

                var recordReferencesEnumerable = await Task.WhenAll(
                    (Searchers
                        .Where(searcher => searcher.IsSelected)
                        .Select(searcher => searcher.Searcher)
                        .OfType<ITextSearcher<ISkyrimMod, ISkyrimModGetter>>()
                        .Select(searcher => Task.Run(() => {
                            // Collect all references of a text searcher from all mods
                            var refs = selectedMods
                                .SelectMany(mod => searcher.GetTextReference(mod, SearchText, ComparisonType))
                                .DistinctBy(x => x.Record is IFormKeyGetter formKeyGetter ? (IComparable) formKeyGetter.FormKey.ToString() : Guid.NewGuid())
                                .ToList();

                            // Replace text if requested
                            if (Replace) {
                                foreach (var recordReferences in refs) {
                                    // foreach (var reference in recordReferences.References) {
                                    recordReferences.Diff.New = recordReferences.Diff.Old.Replace(SearchText, ReplaceText, ComparisonType);
                                    // }
                                }
                            }

                            return Task.FromResult(refs);
                        }))));

                var recordRefs = recordReferencesEnumerable.SelectMany(x => x).ToList();
                Dispatcher.UIThread.Post(() => {
                    References.AddRange(recordRefs);
                    IsBusy = false;
                });
            }
        );

        TreeStructureSource = new HierarchicalTreeDataGridSource<object>(GroupCollection.Items) {
            Columns = {
                new HierarchicalExpanderColumn<object>(
                    new TemplateColumn<object>(
                        "Text",
                        new FuncDataTemplate<object>((obj, _) => obj switch {
                            RecordReferences<ISkyrimMod, ISkyrimModGetter> recordReferences => new TextBox {
                                DataContext = recordReferences.Diff,
                                [!TextBox.TextProperty] = new Binding(nameof(TextDiff.New)),
                                [ScrollViewer.HorizontalScrollBarVisibilityProperty] = ScrollBarVisibility.Auto,
                                VerticalAlignment = VerticalAlignment.Center,
                                MinHeight = 50,
                                MaxHeight = 500,
                            },
                            GroupInstance groupInstance => new TextBlock {
                                Text = groupInstance.Class is IMajorRecordQueryableGetter record ? record.GetName() : groupInstance.Class.ToString(),
                                VerticalAlignment = VerticalAlignment.Center,
                            },
                            _ => null
                        }),
                        new GridLength(500, GridUnitType.Pixel), new TemplateColumnOptions<object>() {
                            IsTextSearchEnabled = true,
                            TextSearchValueSelector = obj => obj is RecordReferences<ISkyrimMod, ISkyrimModGetter> recordReferences ? recordReferences.Diff.New : null
                        }
                    ),
                    x => x switch {
                        GroupInstance groupInstance => groupInstance.Items,
                        _ => null
                    },
                    x => x is GroupInstance),
                new TemplateColumn<object>("EditorID", new FuncDataTemplate<object>((obj, _) => {
                    return obj switch {
                        RecordReferences<ISkyrimMod, ISkyrimModGetter> { Record: IMajorRecordGetter record } => new TextBlock {
                            Text = record.EditorID,
                            VerticalAlignment = VerticalAlignment.Center,
                        },
                        _ => null
                    };
                })),
                new TemplateColumn<object>("FormKey", new FuncDataTemplate<object>((obj, _) => {
                    return obj switch {
                        RecordReferences<ISkyrimMod, ISkyrimModGetter> { Record: IFormKeyGetter formKeyGetter } => new TextBlock {
                            Text = formKeyGetter.FormKey.ToString(),
                            VerticalAlignment = VerticalAlignment.Center,
                        },
                        _ => null
                    };
                })),
                new TemplateColumn<object>(null, new FuncDataTemplate<object>((obj, _) => obj switch {
                    RecordReferences<ISkyrimMod, ISkyrimModGetter> recordReferences => new Button {
                        DataContext = recordReferences,
                        [!Visual.IsVisibleProperty] = recordReferences.Diff.IsDifferent.ToBinding(),
                        Content = "Replace",
                        Command = ReactiveCommand.Create(() => ReplaceRecordReferences(recordReferences)),
                    },
                    GroupInstance groupInstance => new Button {
                        DataContext = groupInstance,
                        [!Visual.IsVisibleProperty] = groupInstance.Items
                            .ObserveCollectionChanges().Unit()
                            .StartWith(Unit.Default)
                            .Select(_ => {
                                return CheckRec(groupInstance.Items);

                                // Check if any children have diffs
                                IObservable<bool> CheckRec(ObservableCollection<object> objects) {
                                    var collectionChanges = objects
                                        .ObserveCollectionChanges().Unit()
                                        .StartWith(Unit.Default);

                                    if (objects.FirstOrDefault() is RecordReferences<ISkyrimMod, ISkyrimModGetter>) {
                                        return collectionChanges
                                            .Select(_ => objects.OfType<RecordReferences<ISkyrimMod, ISkyrimModGetter>>().Select(x => x.Diff.IsDifferent))
                                            .Select(x => x.CombineLatest().Select(i => i.Any(b => b)))
                                            .Switch();
                                    }

                                    return collectionChanges
                                        .Select(_ => {
                                            var observables = objects
                                                .OfType<GroupInstance>()
                                                .Select(x => CheckRec(x.Items));

                                            return observables.CombineLatest().Select(i => i.Any(b => b));
                                        })
                                        .Switch();
                                }
                            })
                            .Switch()
                            .ToBinding(),
                        [!ContentControl.ContentProperty] = groupInstance.Items
                            .ObserveCollectionChanges().Unit()
                            .StartWith(Unit.Default)
                            .Select(_ => {
                                return CheckRec(groupInstance.Items);

                                // Sum of children with diffs
                                IObservable<int> CheckRec(ObservableCollection<object> objects) {
                                    var collectionChanges = objects
                                        .ObserveCollectionChanges().Unit()
                                        .StartWith(Unit.Default);

                                    if (objects.FirstOrDefault() is RecordReferences<ISkyrimMod, ISkyrimModGetter>) {
                                        return collectionChanges
                                            .Select(_ => objects.OfType<RecordReferences<ISkyrimMod, ISkyrimModGetter>>().Select(x => x.Diff.IsDifferent))
                                            .Select(x => x.CombineLatest().Select(i => i.Count(b => b)))
                                            .Switch();
                                    }

                                    return collectionChanges
                                        .Select(_ => {
                                            var observables = objects
                                                .OfType<GroupInstance>()
                                                .Select(x => CheckRec(x.Items));

                                            return observables.CombineLatest().Select(i => i.Sum());
                                        })
                                        .Switch();
                                }
                            })
                            .Switch()
                            .Select(count => $"Replace {count}")
                            .ToBinding(),
                        Command = ReactiveCommand.Create(() => {
                            foreach (var recordReferences in groupInstance.GetItems<RecordReferences<ISkyrimMod, ISkyrimModGetter>>().ToList()) {
                                ReplaceRecordReferences(recordReferences);
                            }
                        }),
                    },
                    _ => null
                }))
            }
        };
    }

    private void ReplaceRecordReferences(RecordReferences<ISkyrimMod, ISkyrimModGetter> recordReferences) {
        var diff = recordReferences.Diff;
        if (diff.Old == diff.New) return;

        recordReferences.TextSearcher.ReplaceTextReference(
            recordReferences.Record,
            _pluginContext.EditorEnvironment.LinkCache,
            _pluginContext.EditorEnvironment.ActiveMod,
            diff.Old,
            diff.New,
            ComparisonType);

        References.Remove(recordReferences);
    }
}
