using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using CreationEditor.Avalonia.Views.Record.Picker;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;
using CreationEditor.Skyrim.Avalonia.Resources.Constants;
using CreationEditor.Skyrim.Avalonia.Views.Record.Picker;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Skyrim.Avalonia.Resources.DataTemplates;

public sealed partial class ConditionDataTemplate : AvaloniaObject, IDataTemplate, IDisposable {
    private readonly CompositeDisposable _disposable = new();

    public static readonly StyledProperty<ILinkCache> LinkCacheProperty
        = AvaloniaProperty.Register<ConditionDataTemplate, ILinkCache>(nameof(LinkCache));

    public static readonly StyledProperty<IMajorRecordGetter?> ContextProperty
        = AvaloniaProperty.Register<ConditionDataTemplate, IMajorRecordGetter?>(nameof(Context));

    public static readonly StyledProperty<IQuestGetter?> QuestContextProperty
        = AvaloniaProperty.Register<ConditionDataTemplate, IQuestGetter?>(nameof(QuestContext));

    public static readonly StyledProperty<EditableCondition> ConditionProperty
        = AvaloniaProperty.Register<ConditionDataTemplate, EditableCondition>(nameof(Condition));

    public static readonly StyledProperty<IObservable<Unit>?> ValueChangedProperty
        = AvaloniaProperty.Register<ConditionDataTemplate, IObservable<Unit>?>(nameof(ValueChanged));

    public ILinkCache LinkCache {
        get => GetValue(LinkCacheProperty);
        set => SetValue(LinkCacheProperty, value);
    }

    public IMajorRecordGetter? Context {
        get => GetValue(ContextProperty);
        set => SetValue(ContextProperty, value);
    }

    public IQuestGetter? QuestContext {
        get => GetValue(QuestContextProperty);
        set => SetValue(QuestContextProperty, value);
    }

    public EditableCondition Condition {
        get => GetValue(ConditionProperty);
        set => SetValue(ConditionProperty, value);
    }

    public IObservable<Unit>? ValueChanged {
        get => GetValue(ValueChangedProperty);
        set => SetValue(ValueChangedProperty, value);
    }

    private readonly Dictionary<Type, ICustomConditionDataTemplate> _conditionTemplateCache;

    public ConditionDataTemplate() {
        _conditionTemplateCache = typeof(ICustomConditionDataTemplate)
            .GetAllSubClasses<ICustomConditionDataTemplate>()
            .ToDictionary(template => template.Type, template => template);

        foreach (var (_, template) in _conditionTemplateCache) {
            template.GetFormKeyPicker = GetFormKeyPicker;
        }

        // Set quest context to context if it is a quest
        this.WhenAnyValue(x => x.Context)
            .Subscribe(context => {
                if (context is IQuestGetter quest) {
                    QuestContext = quest;
                }
            })
            .DisposeWith(_disposable);
    }

    public bool Match(object? data) => data is ConditionData;

    public Control? Build(object? param) {
        if (param is null) return new TextBlock { Text = "Data is not available" };

        if (param is not ConditionData data) return new TextBlock { Text = $"Type {param.GetType()} not supported" };

        var substituteUsageContext = new SubstituteUsageContext(data);

        // Compile the list of parameters
        var parameterControls = EditableCondition.GetParameterProperties(data)
            .Select(parameter => GetControl(parameter, data, substituteUsageContext))
            .ToList();

        // Apply a condition template if it there is a matching one
        var dataType = data.GetType();
        var contextObservable = this.GetObservable(ContextProperty);
        var questContextObservable = this.GetObservable(QuestContextProperty);
        foreach (var (type, template) in _conditionTemplateCache) {
            if (!dataType.InheritsFrom(type)) continue;

            template.Apply(contextObservable, questContextObservable, Condition, data, parameterControls);
            break;
        }

        // Return an item repeater with the parameters
        return parameterControls.Count != 0
            ? new ItemsControl { ItemsSource = parameterControls }
            : null;
    }

    private Control GetControl(PropertyInfo parameter, ConditionData data, SubstituteUsageContext substituteUsageContext) {
        Control control;
        if (parameter.PropertyType.BaseType == typeof(Enum)) {
            var values = Enum.GetValues(parameter.PropertyType)
                .Cast<Enum>()
                .OrderBy(x => x.ToString())
                .ToArray();
            control = new ComboBox {
                DataContext = data,
                ItemsSource = values,
                [!SelectingItemsControl.SelectedItemProperty] = new Binding(parameter.Name),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                [ToolTip.TipProperty] = parameter.Name,
            };
            ValueChanged = control.GetObservable(SelectingItemsControl.SelectedItemProperty).Unit();
        } else if (parameter.PropertyType == typeof(string)) {
            control = new TextBox {
                DataContext = data,
                [!TextBlock.TextProperty] = new Binding(parameter.Name),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                [ToolTip.TipProperty] = parameter.Name,
            };
            ValueChanged = control.GetObservable(TextBlock.TextProperty).Unit();
        } else if (parameter.PropertyType == typeof(int)) {
            control = new NumericUpDown {
                DataContext = data,
                Minimum = int.MinValue,
                Maximum = int.MaxValue,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = new Binding(parameter.Name),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                [ToolTip.TipProperty] = parameter.Name,
            };
            ValueChanged = control.GetObservable(NumericUpDown.ValueProperty).Unit();
        } else if (parameter.PropertyType == typeof(float)) {
            control = new NumericUpDown {
                DataContext = data,
                Minimum = decimal.MinValue,
                Maximum = decimal.MaxValue,
                FormatString = "N4",
                [!NumericUpDown.ValueProperty] = new Binding(parameter.Name),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                [ToolTip.TipProperty] = parameter.Name,
            };
            ValueChanged = control.GetObservable(NumericUpDown.ValueProperty).Unit();
        } else if (parameter.PropertyType.GetInterfaces().Contains(typeof(IFormLinkContainerGetter))) {
            control = GetFormKeyPicker(
                data,
                parameter.Name,
                $"{nameof(FormLinkOrIndex<>.Link)}.{nameof(FormLinkOrIndex<>.Link.FormKey)}",
                parameter.PropertyType.GenericTypeArguments);
        } else if (parameter.PropertyType.GetInterfaces().Contains(typeof(IFormLinkGetter))) {
            control = GetFormKeyPicker(
                data,
                parameter.Name,
                nameof(IFormLinkGetter.FormKey),
                parameter.PropertyType.GenericTypeArguments);
        } else {
            control = new TextBlock { Text = $"Type {parameter.PropertyType} not recognized" };
        }

        return DecorateWithSubstitutePickers(
            control,
            data,
            parameter.Name,
            substituteUsageContext,
            parameter.PropertyType.GenericTypeArguments.Length == 0
                ? parameter.PropertyType.AsEnumerable()
                : parameter.PropertyType.GenericTypeArguments);
    }

    private Control GetFormKeyPicker(ConditionData data, string parameter, string parameterMember, IEnumerable<Type> types) {
        var scopedTypes = types.ToArray();

        if (scopedTypes.AllInheritFromAny(RecordTypeConstants.AllPlacedInterfaceTypes)) {
            var placedPickerButton = new PlacedPickerButton {
                DataContext = data,
                [PlacedPickerButton.ScopedTypesProperty] = types,
                [!PlacedPickerButton.PlacedProperty] = new Binding($"{parameter}.{nameof(FormLinkOrIndex<>.Link)}"),
                [!PlacedPickerButton.LinkCacheProperty] = this[!LinkCacheProperty],
                [ToolTip.TipProperty] = parameter,
            };
            ValueChanged = placedPickerButton.GetObservable(PlacedPickerButton.PlacedProperty).Unit();
            return placedPickerButton;
        }

        var formKeyPicker = new FormKeyPicker {
            [!AFormKeyPicker.LinkCacheProperty] = this[!LinkCacheProperty],
            ShowFormKeyBox = false,
            DataContext = data,
            ScopedTypes = scopedTypes,
            [!AFormKeyPicker.FormKeyProperty] = new Binding($"{parameter}.{parameterMember}"),
            [ToolTip.TipProperty] = parameter,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };
        ValueChanged = formKeyPicker.GetObservable(AFormKeyPicker.FormKeyProperty).Unit();
        return formKeyPicker;
    }

    private Control DecorateWithSubstitutePickers(
        Control control,
        ConditionData data,
        string parameter,
        SubstituteUsageContext substituteUsageContext,
        IEnumerable<Type> types) {
        // Use specific types for package data functions otherwise use the types from the parameter
        var scopedTypes = (data switch {
            IGetWithinPackageLocationConditionDataGetter => RecordTypeConstants.PackageDataLocationTypes,
            IGetNumericPackageDataConditionDataGetter => RecordTypeConstants.PackageDataNumericTypes,
            IIsNullPackageDataConditionDataGetter => RecordTypeConstants.PackageDataTypes,
            _ => types,
        }).ToArray();

        // Index names based on convention 
        var parameterIsAlwaysAlias = parameter.Contains("AliasIndex", StringComparison.OrdinalIgnoreCase);
        var parameterIsAlwaysPackageData = parameter.Contains("PackageDataIndex", StringComparison.OrdinalIgnoreCase);

        var useAliases = parameterIsAlwaysAlias
         || (QuestContext is not null && scopedTypes.AnyInheritsFromAny(RecordTypeConstants.AllAliasTypes));
        var usePackageData = parameterIsAlwaysPackageData
         || (Context is IPackageGetter && scopedTypes.AnyInheritsFromAny(RecordTypeConstants.PackageDataTypes));

        if (!useAliases && !usePackageData) return control;

        var indexDataContext = new IndexDataContext(data, substituteUsageContext, parameter);

        Grid grid;
        if (useAliases && usePackageData) {
            // Handle the case where both aliases and package data are available
            grid = new Grid {
                ColumnDefinitions = {
                    new ColumnDefinition(new GridLength(24)),
                    new ColumnDefinition(new GridLength(24)),
                    new ColumnDefinition(),
                },
            };

            var (usesAlias, aliasCheckBox, aliasPicker) = GetAliasPicker(0, 2);
            var (usesPackageData, packageDataCheckBox, packageDataPicker) = GetPackageDataPicker(1, 2);
            var usesDefault = usesAlias.CombineLatest(usesPackageData, (quest, packageData) => quest is not true && packageData is not true);

            control[!Visual.IsVisibleProperty] = usesDefault.ToBinding();
            control[Grid.ColumnProperty] = 2;
            grid.Children.Add(aliasCheckBox);
            grid.Children.Add(packageDataCheckBox);
            grid.Children.Add(aliasPicker);
            grid.Children.Add(packageDataPicker);
            grid.Children.Add(control);
        } else {
            // Use either aliases or package data
            grid = new Grid {
                ColumnDefinitions = {
                    new ColumnDefinition(new GridLength(24)),
                    new ColumnDefinition(),
                },
            };

            var (usesSubstitute, checkBox, picker) = useAliases
                ? GetAliasPicker(0, 1)
                : GetPackageDataPicker(0, 1);

            // Only show alias or package data picker if the parameter is always of that type
            if (parameterIsAlwaysAlias || parameterIsAlwaysPackageData) {
                picker.IsVisible = true;
                return picker;
            }

            control[!Visual.IsVisibleProperty] = usesSubstitute.Negate().ToBinding();
            control[Grid.ColumnProperty] = 1;
            grid.Children.Add(checkBox);
            grid.Children.Add(picker);
            grid.Children.Add(control);
        }

        return grid;

        (IObservable<bool?>, CheckBox, Control) GetPackageDataPicker(int checkBoxColumn, int pickerColumn) {
            var usesPackageDataCheckBox = new CheckBox {
                DataContext = substituteUsageContext,
                ZIndex = -1,
                [Grid.ColumnProperty] = checkBoxColumn,
                [ToolTip.TipProperty] = "Use Package Data",
                [!ToggleButton.IsCheckedProperty] = new Binding(nameof(SubstituteUsageContext.UsePackageData)),
            };
            var usesPackageData = usesPackageDataCheckBox.GetObservable(ToggleButton.IsCheckedProperty);

            var packageDataPicker = new PackageDataPicker {
                DataContext = indexDataContext,
                [Grid.ColumnProperty] = pickerColumn,
                [!Visual.IsVisibleProperty] = usesPackageData.ToBinding(),
                [!PackageDataPicker.PackageProperty] = this[!ContextProperty],
                [!PackageDataPicker.PackageDataIndexProperty] = new Binding(nameof(IndexDataContext.PackageDataIndex), BindingMode.TwoWay),
                [PackageDataPicker.ScopedTypesProperty] = scopedTypes,
                [!PackageDataPicker.LinkCacheProperty] = this[!LinkCacheProperty],
                [ToolTip.TipProperty] = parameter,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            return (usesPackageData, usesPackageDataCheckBox, packageDataPicker);
        }
        (IObservable<bool?>, CheckBox, Control) GetAliasPicker(int checkBoxColumn, int pickerColumn) {
            var usesAliasesCheckBox = new CheckBox {
                DataContext = substituteUsageContext,
                ZIndex = -1,
                [Grid.ColumnProperty] = checkBoxColumn,
                [ToolTip.TipProperty] = "Use Alias",
                [!ToggleButton.IsCheckedProperty] = new Binding(nameof(SubstituteUsageContext.UseAliases)),
            };
            var usesAliases = usesAliasesCheckBox.GetObservable(ToggleButton.IsCheckedProperty);

            var type = QuestAlias.TypeEnum.Reference;
            if (scopedTypes.AnyInheritsFrom(typeof(ILocationGetter))) {
                type = QuestAlias.TypeEnum.Location;
            }
            var questAliasPicker = new QuestAliasPicker {
                DataContext = indexDataContext,
                [Grid.ColumnProperty] = pickerColumn,
                [!Visual.IsVisibleProperty] = usesAliases.ToBinding(),
                [!QuestAliasPicker.QuestProperty] = this[!QuestContextProperty],
                [!QuestAliasPicker.AliasIndexProperty] = new Binding(nameof(IndexDataContext.AliasIndex), BindingMode.TwoWay),
                [QuestAliasPicker.ScopedAliasTypesProperty] = type.AsEnumerable(),
                [ToolTip.TipProperty] = parameter,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            return (usesAliases, usesAliasesCheckBox, questAliasPicker);
        }
    }

    public sealed partial class SubstituteUsageContext : ReactiveObject, IDisposable {
        private readonly CompositeDisposable _disposable = new();

        [Reactive] public partial bool UseAliases { get; set; }
        [Reactive] public partial bool UsePackageData { get; set; }

        public SubstituteUsageContext(ConditionData data) {
            UseAliases = data.UseAliases;
            UsePackageData = data.UsePackageData;

            this.WhenAnyValue(x => x.UseAliases)
                .WhereTrue()
                .Subscribe(_ => UsePackageData = false)
                .DisposeWith(_disposable);

            this.WhenAnyValue(x => x.UsePackageData)
                .WhereTrue()
                .Subscribe(_ => UseAliases = false)
                .DisposeWith(_disposable);
        }

        public void Dispose() => _disposable.Dispose();
    }

    public sealed partial class IndexDataContext : ReactiveObject, IDisposable {
        private readonly CompositeDisposable _disposable = new();

        [Reactive] public partial uint? AliasIndex { get; set; }
        [Reactive] public partial sbyte? PackageDataIndex { get; set; }

        public IndexDataContext(ConditionData data, SubstituteUsageContext substituteUsageContext, string parameter) {
            if (data.UseAliases && data.TryGetProperty<uint?>($"{parameter}.{nameof(FormLinkOrIndex<>.Index)}", out var value)) {
                AliasIndex = value;
            } else if (data.UsePackageData && data.TryGetProperty($"{parameter}.{nameof(FormLinkOrIndex<>.Index)}", out value)) {
                PackageDataIndex = (sbyte?) value;
            }

            SetupReactiveIndex(data,
                parameter,
                nameof(IConditionDataGetter.UseAliases),
                substituteUsageContext,
                x => x.UseAliases,
                x => x.AliasIndex);
            SetupReactiveIndex(data,
                parameter,
                nameof(IConditionDataGetter.UsePackageData),
                substituteUsageContext,
                x => x.UsePackageData,
                x => x.PackageDataIndex);
        }

        private void SetupReactiveIndex<T>(
            ConditionData data,
            string parameter,
            string useProperty,
            SubstituteUsageContext substituteUsageContext,
            Expression<Func<SubstituteUsageContext, bool>> useExpression,
            Expression<Func<IndexDataContext, T>> indexExpression) {

            var setExpression = new Action<T>(index => data.TrySetProperty(
                $"{parameter}.{nameof(FormLinkOrIndex<>.Index)}",
                Convert.ToUInt32(Convert.ToInt32(index) < 0 ? 0 : index)));

            var substituteUsedChanged = substituteUsageContext.WhenAnyValue(useExpression);

            substituteUsedChanged
                .Subscribe(use => data.TrySetProperty(useProperty, use))
                .DisposeWith(_disposable);

            substituteUsedChanged
                .WhereTrue()
                .CombineLatest(this.WhenAnyValue(indexExpression), (_, index) => index)
                .Subscribe(setExpression)
                .DisposeWith(_disposable);

            this.WhenAnyValue(indexExpression)
                .Subscribe(setExpression)
                .DisposeWith(_disposable);
        }

        public void Dispose() => _disposable.Dispose();
    }

    public void Dispose() => _disposable.Dispose();
}
