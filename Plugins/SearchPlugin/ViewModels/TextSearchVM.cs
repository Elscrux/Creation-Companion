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
using CreationEditor.Services.Environment;
using DynamicData;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SearchPlugin.Models;
namespace SearchPlugin.ViewModels;

public sealed class TextSearchVM<TMod, TModGetter> : ViewModel, ITextSearchVM
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    where TMod : class, TModGetter, IContextMod<TMod, TModGetter> {
    private readonly IEditorEnvironment<TMod, TModGetter> _editorEnvironment;

    public IList<SelectableSearcher> Searchers { get; }
    public ObservableCollectionExtended<TextReference> References { get; } = [];

    public GroupCollection<TextReference> GroupCollection { get; }
    public Group<TextReference> TypeGroup { get; }
    public Group<TextReference> RecordGroup { get; }
    public HierarchicalTreeDataGridSource<object> TreeStructureSource { get; }

    public ReadOnlyObservableCollection<ModItem> Mods { get; }

    public ReactiveCommand<Unit, Unit> SearchCommand { get; }

    [Reactive] public string? SearchText { get; set; }
    [Reactive] public string? ReplaceText { get; set; }
    [Reactive] public bool Replace { get; set; }
    [Reactive] public bool CaseSensitive { get; set; }
    [Reactive] public bool IsBusy { get; set; }

    public StringComparison ComparisonType => CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

    public TextSearchVM(
        IEditorEnvironment<TMod, TModGetter> editorEnvironment) {
        _editorEnvironment = editorEnvironment;

        Searchers = new ObservableCollectionExtended<SelectableSearcher>(
            typeof(ITextSearcher<TMod, TModGetter>)
                .GetAllSubClasses<ITextSearcher<TMod, TModGetter>>()
                .Select(searcher => new SelectableSearcher(searcher)));

        Mods = _editorEnvironment.LinkCacheChanged
            .Select(x => x.ListedOrder.AsObservableChangeSet())
            .Switch()
            .Transform(mod => new ModItem(mod.ModKey) { IsSelected = true })
            .ToObservableCollection(this);

        TypeGroup = new Group<TextReference>(references => references.TextSearcher, true);
        RecordGroup = new Group<TextReference>(references => references.Record, true);
        GroupCollection = new GroupCollection<TextReference>(References, TypeGroup, RecordGroup).DisposeWith(ActivatedDisposable);

        SearchCommand = ReactiveCommand.CreateFromTask(Search);

        TreeStructureSource = new HierarchicalTreeDataGridSource<object>(GroupCollection.Items) {
            Columns = {
                new HierarchicalExpanderColumn<object>(
                    new TemplateColumn<object>(
                        "Text",
                        new FuncDataTemplate<object>((obj, _) => obj switch {
                            TextReference recordReferences => new TextBox {
                                DataContext = recordReferences.Diff,
                                [!TextBox.TextProperty] = new Binding(nameof(TextDiff.New)),
                                [ScrollViewer.HorizontalScrollBarVisibilityProperty] = ScrollBarVisibility.Auto,
                                VerticalAlignment = VerticalAlignment.Center,
                                MinHeight = 50,
                                MaxHeight = 500,
                                AcceptsReturn = true,
                            },
                            GroupInstance groupInstance => new TextBlock {
                                Text = groupInstance.Class is IMajorRecordQueryableGetter record ? record.GetName() : groupInstance.Class.ToString(),
                                VerticalAlignment = VerticalAlignment.Center,
                            },
                            _ => null,
                        }),
                        null,
                        new GridLength(500, GridUnitType.Pixel),
                        new TemplateColumnOptions<object> {
                            IsTextSearchEnabled = true,
                            TextSearchValueSelector = obj => obj is TextReference recordReferences ? recordReferences.Diff.New : null,
                        }
                    ),
                    x => x switch {
                        GroupInstance groupInstance => groupInstance.Items,
                        _ => null,
                    },
                    x => x is GroupInstance),
                new TemplateColumn<object>(
                    "EditorID",
                    new FuncDataTemplate<object>((obj, _) => {
                        return obj switch {
                            TextReference { Record: IMajorRecordGetter record } => new TextBlock {
                                Text = record.EditorID,
                                VerticalAlignment = VerticalAlignment.Center,
                            },
                            _ => null,
                        };
                    })),
                new TemplateColumn<object>(
                    "FormKey",
                    new FuncDataTemplate<object>((obj, _) => {
                        return obj switch {
                            TextReference { Record: IFormKeyGetter formKeyGetter } => new TextBlock {
                                Text = formKeyGetter.FormKey.ToString(),
                                VerticalAlignment = VerticalAlignment.Center,
                            },
                            _ => null,
                        };
                    })),
                new TemplateColumn<object>(
                    null,
                    new FuncDataTemplate<object>((obj, _) => obj switch {
                        TextReference recordReferences => new Button {
                            DataContext = recordReferences,
                            [!Visual.IsVisibleProperty] = recordReferences.Diff.IsDifferent.ToBinding(),
                            Content = "Replace",
                            Command = ReactiveCommand.CreateFromTask(async () => {
                                await ReplaceRecordReferences(recordReferences);
                                References.Remove(recordReferences);
                            }),
                        },
                        GroupInstance groupInstance => new Button {
                            DataContext = groupInstance,
                            [!Visual.IsVisibleProperty] = groupInstance.Items
                                .SelectWhenCollectionChanges(() => {
                                    return CheckRec(groupInstance.Items);

                                    // Check if any children have diffs
                                    IObservable<bool> CheckRec(IObservableCollection<object> objects) {
                                        var collectionChanges = objects.WhenCollectionChanges();

                                        if (objects.FirstOrDefault() is TextReference) {
                                            return collectionChanges
                                                .Select(_ => objects.OfType<TextReference>().Select(x => x.Diff.IsDifferent))
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
                                .ToBinding(),
                            [!ContentControl.ContentProperty] = groupInstance.Items
                                .SelectWhenCollectionChanges(() => {
                                    return CheckRec(groupInstance.Items);

                                    // Sum of children with diffs
                                    IObservable<int> CheckRec(IObservableCollection<object> objects) {
                                        var collectionChanges = objects.WhenCollectionChanges();

                                        if (objects.FirstOrDefault() is TextReference) {
                                            return collectionChanges
                                                .Select(_ => objects.OfType<TextReference>().Select(x => x.Diff.IsDifferent))
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
                                .Select(count => $"Replace {count}")
                                .ToBinding(),
                            Command = ReactiveCommand.CreateFromTask(async () => {
                                var recordReferencesList = groupInstance.GetItems<TextReference>().ToList();

                                foreach (var recordReferences in recordReferencesList) {
                                    await ReplaceRecordReferences(recordReferences);
                                }

                                References.RemoveRange(recordReferencesList);
                            }),
                        },
                        _ => null,
                    })),
            },
        };
    }

    private async Task Search() {
        if (SearchText is null) return;

        Dispatcher.UIThread.Post(() => IsBusy = true);

        References.Clear();

        var selectedModKeys = Mods.Where(mod => mod.IsSelected)
            .Select(mod => mod.ModKey);

        var selectedMods = _editorEnvironment.LinkCache.ResolveMods(selectedModKeys).ToList();

        var recordReferencesEnumerable = await Task.WhenAll(Searchers.Where(searcher => searcher.IsSelected)
            .Select(searcher => searcher.Searcher)
            .OfType<ITextSearcher<TMod, TModGetter>>()
            .Select(searcher => Task.Run(() => {
                // Collect all references of a text searcher from all mods
                var refs = selectedMods.SelectMany(mod => searcher.GetTextReference(mod, SearchText, ComparisonType))
                    .Distinct()
                    .ToList();

                // Replace text if requested
                if (Replace) {
                    foreach (var recordReferences in refs) {
                        recordReferences.Diff.New = recordReferences.Diff.Old.Replace(SearchText, ReplaceText, ComparisonType);
                    }
                }

                return Task.FromResult(refs);
            })));

        var recordRefs = recordReferencesEnumerable.SelectMany(x => x).ToList();
        Dispatcher.UIThread.Post(() => {
            References.AddRange(recordRefs);
            IsBusy = false;
        });
    }

    private Task ReplaceRecordReferences(TextReference textReference) {
        var diff = textReference.Diff;
        if (diff.Old == diff.New) return Task.CompletedTask;

        return Task.Run(() =>
            textReference.TextSearcher.ReplaceTextReference(
                textReference.Record,
                _editorEnvironment.LinkCache,
                _editorEnvironment.ActiveMod,
                diff.Old,
                diff.New,
                ComparisonType));
    }
}
