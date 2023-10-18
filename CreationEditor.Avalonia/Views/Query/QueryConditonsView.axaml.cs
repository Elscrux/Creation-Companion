using System.Collections;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using CreationEditor.Avalonia.DataTemplates;
using CreationEditor.Services.Query.Select;
using CreationEditor.Services.Query.Where;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins.Cache;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views.Query;

public partial class QueryConditionsView : ActivatableUserControl {
    public static readonly StyledProperty<IObservableCollection<IQueryConditionEntry>> QueryConditionsProperty
        = AvaloniaProperty.Register<QueryConditionsView, IObservableCollection<IQueryConditionEntry>>(nameof(QueryConditions));

    public IObservableCollection<IQueryConditionEntry> QueryConditions {
        get => GetValue(QueryConditionsProperty);
        set => SetValue(QueryConditionsProperty, value);
    }

    public static readonly StyledProperty<ILinkCache> LinkCacheProperty
        = AvaloniaProperty.Register<QueryView, ILinkCache>(nameof(LinkCache));

    public ILinkCache LinkCache {
        get => GetValue(LinkCacheProperty);
        set => SetValue(LinkCacheProperty, value);
    }

    public static readonly StyledProperty<ITreeDataGridSource<IQueryConditionEntry>> ConditionSourceProperty
        = AvaloniaProperty.Register<QueryConditionsView, ITreeDataGridSource<IQueryConditionEntry>>(nameof(ConditionTreeSource));

    public ITreeDataGridSource<IQueryConditionEntry> ConditionTreeSource {
        get => GetValue(ConditionSourceProperty);
        set => SetValue(ConditionSourceProperty, value);
    }

    public static readonly StyledProperty<IQueryConditionEntryFactory> ConditionEntryFactoryProperty
        = AvaloniaProperty.Register<QueryConditionsView, IQueryConditionEntryFactory>(nameof(ConditionEntryFactory));

    public IQueryConditionEntryFactory ConditionEntryFactory {
        get => GetValue(ConditionEntryFactoryProperty);
        set => SetValue(ConditionEntryFactoryProperty, value);
    }

    public static readonly StyledProperty<Type> ContextTypeProperty
        = AvaloniaProperty.Register<QueryConditionsView, Type>(nameof(ContextType));

    public Type ContextType {
        get => GetValue(ContextTypeProperty);
        set => SetValue(ContextTypeProperty, value);
    }

    public static readonly StyledProperty<ReactiveCommand<Unit, Unit>> AddConditionProperty
        = AvaloniaProperty.Register<QueryConditionsView, ReactiveCommand<Unit, Unit>>(nameof(AddCondition));

    public ReactiveCommand<Unit, Unit> AddCondition {
        get => GetValue(AddConditionProperty);
        set => SetValue(AddConditionProperty, value);
    }

    public static readonly StyledProperty<ReactiveCommand<IList, Unit>> RemoveConditionProperty
        = AvaloniaProperty.Register<QueryConditionsView, ReactiveCommand<IList, Unit>>(nameof(RemoveCondition));

    public ReactiveCommand<IList, Unit> RemoveCondition {
        get => GetValue(RemoveConditionProperty);
        set => SetValue(RemoveConditionProperty, value);
    }

    public QueryConditionsView() {
        InitializeComponent();

        AddCondition = ReactiveCommand.Create(() => {
            QueryConditions.Add(ConditionEntryFactory.Create(ContextType));
        });

        RemoveCondition = ReactiveCommand.Create<IList>(conditions => {
            foreach (var condition in conditions.OfType<IQueryConditionEntry>()) {
                QueryConditions.Remove(condition);
            }
        });
    }

    protected override void WhenActivated() {
        this.WhenAnyValue(x => x.QueryConditions)
            .NotNull()
            .Subscribe(UpdateDataGrid);
    }

    private void UpdateDataGrid(IObservableCollection<IQueryConditionEntry> queryConditions) {
        ConditionTreeSource = new FlatTreeDataGridSource<IQueryConditionEntry>(queryConditions) {
            Columns = {
                new TemplateColumn<IQueryConditionEntry>(
                    "Field",
                    new FuncDataTemplate<IQueryConditionEntry>((conditionEntry, _) => {
                        if (conditionEntry is null) return null;

                        return new ComboBox {
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            DataContext = conditionEntry,
                            ItemTemplate = new FuncDataTemplate<IQueryField>((field, _) => {
                                if (field is null) return null;

                                return new TextBlock { Text = field.Name };
                            }),
                            ItemsSource = conditionEntry.FieldSelector.Fields,
                            [!SelectingItemsControl.SelectedItemProperty] = new Binding($"{nameof(IQueryConditionEntry.FieldSelector)}.{nameof(IQueryConditionEntry.FieldSelector.SelectedField)}"),
                        };
                    })),
                new TemplateColumn<IQueryConditionEntry>(
                    "Not",
                    new FuncDataTemplate<IQueryConditionEntry>((conditionEntry, _) => {
                        if (conditionEntry is null) return null;

                        return new CheckBox {
                            DataContext = conditionEntry,
                            Classes = { "CheckmarkOnly" },
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            [!ToggleButton.IsCheckedProperty] = new Binding(nameof(IQueryConditionEntry.Negate)),
                        };
                    }),
                    options: new TemplateColumnOptions<IQueryConditionEntry> {
                        CanUserResizeColumn = false,
                        CanUserSortColumn = false,
                    }),
                new TemplateColumn<IQueryConditionEntry>(
                    "Function",
                    new FuncDataTemplate<IQueryConditionEntry>((conditionEntry, _) => {
                        if (conditionEntry is null) return null;

                        return new ComboBox {
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            DataContext = conditionEntry,
                            ItemTemplate = new FuncDataTemplate<ICompareFunction>((function, _) => {
                                if (function is null) return null;

                                return new TextBlock { Text = function.Operator };
                            }),
                            ItemsSource = conditionEntry.CompareFunctions,
                            [!SelectingItemsControl.SelectedItemProperty] = new Binding($"{nameof(conditionEntry.SelectedCompareFunction)}"),
                        };
                    })),
                new TemplateColumn<IQueryConditionEntry>(
                    "Value",
                    new FuncDataTemplate<IQueryConditionEntry>((conditionEntry, _) => {
                        if (conditionEntry is null) return null;

                        var valueEditorTemplate = new ConditionValueEditorTemplate {
                            [!ConditionValueEditorTemplate.LinkCacheProperty] = this.GetObservable(LinkCacheProperty).ToBinding(),
                            [!ConditionValueEditorTemplate.ConditionEntryFactoryProperty] = this.GetObservable(ConditionEntryFactoryProperty).ToBinding(),
                        };

                        var contentControl = new ContentControl {
                            [!ContentProperty] = conditionEntry.WhenAnyValue(x => x.ConditionState).ToBinding(),
                            DataTemplates = { valueEditorTemplate },
                        };

                        return contentControl;
                    })),
                new TemplateColumn<IQueryConditionEntry>(
                    string.Empty,
                    new FuncDataTemplate<IQueryConditionEntry>((condition, _) => {
                        if (condition is null) return null;

                        return new ToggleButton {
                            DataContext = condition,
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            [!ToggleButton.IsCheckedProperty] = new Binding(nameof(IQueryConditionEntry.IsOr)),
                            [!ContentProperty] = condition.WhenAnyValue(x => x.IsOr).Select(x => x ? "Or" : "And").ToBinding(),
                        };
                    })),
            }
        };
    }
}
