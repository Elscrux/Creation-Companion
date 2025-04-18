﻿using System.Collections;
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
using CreationEditor.Services.Query;
using CreationEditor.Services.Query.Select;
using CreationEditor.Services.Query.Where;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins.Cache;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views.Query;

public partial class QueryConditionsView : ActivatableUserControl {
    public static readonly StyledProperty<IObservableCollection<IQueryCondition>> QueryConditionsProperty
        = AvaloniaProperty.Register<QueryConditionsView, IObservableCollection<IQueryCondition>>(nameof(QueryConditions));

    public IObservableCollection<IQueryCondition> QueryConditions {
        get => GetValue(QueryConditionsProperty);
        set => SetValue(QueryConditionsProperty, value);
    }

    public static readonly StyledProperty<ILinkCache> LinkCacheProperty
        = AvaloniaProperty.Register<QueryView, ILinkCache>(nameof(LinkCache));

    public ILinkCache LinkCache {
        get => GetValue(LinkCacheProperty);
        set => SetValue(LinkCacheProperty, value);
    }

    public static readonly StyledProperty<ITreeDataGridSource<IQueryCondition>> ConditionSourceProperty
        = AvaloniaProperty.Register<QueryConditionsView, ITreeDataGridSource<IQueryCondition>>(nameof(ConditionTreeSource));

    public ITreeDataGridSource<IQueryCondition> ConditionTreeSource {
        get => GetValue(ConditionSourceProperty);
        set => SetValue(ConditionSourceProperty, value);
    }

    public static readonly StyledProperty<IQueryConditionFactory> ConditionFactoryProperty
        = AvaloniaProperty.Register<QueryConditionsView, IQueryConditionFactory>(nameof(ConditionFactory));

    public IQueryConditionFactory ConditionFactory {
        get => GetValue(ConditionFactoryProperty);
        set => SetValue(ConditionFactoryProperty, value);
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
            QueryConditions.Add(ConditionFactory.Create(ContextType));
        });

        RemoveCondition = ReactiveCommand.Create<IList>(conditions => {
            foreach (var condition in conditions.OfType<IQueryCondition>()) {
                QueryConditions.Remove(condition);
            }
        });
    }

    protected override void WhenActivated() {
        this.WhenAnyValue(x => x.QueryConditions)
            .NotNull()
            .Subscribe(UpdateDataGrid);
    }

    private void UpdateDataGrid(IObservableCollection<IQueryCondition> queryConditions) {
        ConditionTreeSource = new FlatTreeDataGridSource<IQueryCondition>(queryConditions) {
            Columns = {
                new TemplateColumn<IQueryCondition>(
                    "Field",
                    new FuncDataTemplate<IQueryCondition>((condition, _) => {
                        if (condition is null) return null;

                        var queryFieldProvider = new ReflectionIQueryFieldProvider();
                        return new ComboBox {
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            DataContext = condition,
                            ItemTemplate = new FuncDataTemplate<IQueryField>((field, _) => {
                                if (field is null) return null;

                                return new TextBlock { Text = field.Name };
                            }),
                            ItemsSource = queryFieldProvider.FromType(condition.FieldSelector.RecordType),
                            [!SelectingItemsControl.SelectedItemProperty] =
                                new Binding($"{nameof(IQueryCondition.FieldSelector)}.{nameof(IQueryCondition.FieldSelector.SelectedField)}"),
                        };
                    })),
                new TemplateColumn<IQueryCondition>(
                    "Not",
                    new FuncDataTemplate<IQueryCondition>((condition, _) => {
                        if (condition is null) return null;

                        return new CheckBox {
                            DataContext = condition,
                            Classes = { "CheckmarkOnly" },
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            [!ToggleButton.IsCheckedProperty] = new Binding(nameof(IQueryCondition.Negate)),
                        };
                    }),
                    options: new TemplateColumnOptions<IQueryCondition> {
                        CanUserResizeColumn = false,
                        CanUserSortColumn = false,
                    }),
                new TemplateColumn<IQueryCondition>(
                    "Function",
                    new FuncDataTemplate<IQueryCondition>((condition, _) => {
                        if (condition is null) return null;

                        return new ComboBox {
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            DataContext = condition,
                            ItemTemplate = new FuncDataTemplate<IQueryCompareFunction>((function, _) => {
                                if (function is null) return null;

                                return new TextBlock { Text = function.Operator };
                            }),
                            ItemsSource = condition.CompareFunctions,
                            [!SelectingItemsControl.SelectedItemProperty] = new Binding($"{nameof(condition.SelectedCompareFunction)}"),
                        };
                    })),
                new TemplateColumn<IQueryCondition>(
                    "Value",
                    new FuncDataTemplate<IQueryCondition>((condition, _) => {
                        if (condition is null) return null;

                        var valueEditorTemplate = new QueryConditionFieldEditorTemplate {
                            [!QueryConditionFieldEditorTemplate.LinkCacheProperty] = this.GetObservable(LinkCacheProperty).ToBinding(),
                            [!QueryConditionFieldEditorTemplate.ConditionFactoryProperty] = this.GetObservable(ConditionFactoryProperty).ToBinding(),
                        };

                        var contentControl = new ContentControl {
                            [!ContentProperty] = condition.WhenAnyValue(x => x.ConditionState).ToBinding(),
                            DataTemplates = { valueEditorTemplate },
                        };

                        return contentControl;
                    })),
                new TemplateColumn<IQueryCondition>(
                    string.Empty,
                    new FuncDataTemplate<IQueryCondition>((condition, _) => {
                        if (condition is null) return null;

                        return new ToggleButton {
                            DataContext = condition,
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            [!ToggleButton.IsCheckedProperty] = new Binding(nameof(IQueryCondition.IsOr)),
                            [!ContentProperty] = condition.WhenAnyValue(x => x.IsOr).Select(x => x ? "Or" : "And").ToBinding(),
                        };
                    })),
            },
        };
    }
}
