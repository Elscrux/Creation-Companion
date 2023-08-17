using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using CreationEditor.Avalonia.Views.Query;
using CreationEditor.Avalonia.Views.Record.Picker;
using CreationEditor.Services.Query.Where;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Noggog;
namespace CreationEditor.Avalonia.DataTemplates;

public sealed class ConditionValueEditorTemplate : AvaloniaObject, IDataTemplate {
    public static readonly StyledProperty<ILinkCache> LinkCacheProperty
        = AvaloniaProperty.Register<ConditionValueEditorTemplate, ILinkCache>(nameof(LinkCache));

    public ILinkCache LinkCache {
        get => GetValue(LinkCacheProperty);
        set => SetValue(LinkCacheProperty, value);
    }

    public static readonly StyledProperty<IQueryConditionEntryFactory> ConditionEntryFactoryProperty
        = AvaloniaProperty.Register<ConditionValueEditorTemplate, IQueryConditionEntryFactory>(nameof(ConditionEntryFactory));

    public IQueryConditionEntryFactory ConditionEntryFactory {
        get => GetValue(ConditionEntryFactoryProperty);
        set => SetValue(ConditionEntryFactoryProperty, value);
    }

    public bool Match(object? data) => data is IQueryCondition;

    public Control Build(object? param) {
        if (param is not IQueryCondition condition) return new TextBlock { Text = $"No Condition is available for {param}" };

        var binding = new Binding(nameof(IQueryValueCondition.CompareValue));

        var compareValueType = condition is SimpleListValueCondition ? condition.ActualFieldType.GetGenericArguments().First() : condition.CompareValueType;
        var actualFieldType = condition is SimpleListValueCondition ? condition.ActualFieldType.GetGenericArguments().First() : condition.ActualFieldType;

        Control? control;
        if (condition is IQueryListCondition listCondition) {
            var listType = actualFieldType.GetGenericArguments().FirstOrDefault() ?? actualFieldType;
            if (listCondition.SubConditions.Count == 0) {
                listCondition.SubConditions.Add(ConditionEntryFactory.Create(listType));
            }

            var queryConditionsView = new QueryConditionsView {
                ContextType = listType,
                QueryConditions = listCondition.SubConditions,
                [!QueryConditionsView.ConditionEntryFactoryProperty] = this.GetObservable(ConditionEntryFactoryProperty).ToBinding(),
                [!QueryConditionsView.LinkCacheProperty] = this.GetObservable(LinkCacheProperty).ToBinding()
            };

            control = new Button {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                [!ContentControl.ContentProperty] = listCondition.SubConditions
                    .Select(x => x.Summary)
                    .CombineLatest()
                    .Select(list => string.Join(' ', list))
                    .ToBinding(),
                Flyout = new Flyout {
                    Content = new StackPanel {
                        Children = {
                            new TextBlock { Text = condition.ActualFieldType.Name },
                            queryConditionsView
                        }
                    }
                }
            };
        } else if (compareValueType == typeof(bool)) {
            control = new CheckBox {
                [!ToggleButton.IsCheckedProperty] = binding
            };
        } else if (compareValueType == typeof(string)) {
            control = new TextBox {
                [!TextBox.TextProperty] = binding
            };
        } else if (compareValueType == typeof(float) || compareValueType == typeof(double)) {
            control = new NumericUpDown {
                Minimum = decimal.MinValue,
                Maximum = decimal.MaxValue,
                Increment = 0.1m,
                FormatString = "N4",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (compareValueType == typeof(Enum)) {
            control = new ComboBox {
                ItemsSource = Enum.GetValues(actualFieldType).Cast<Enum>().OrderBy(x => x.ToString()).ToArray(),
                [!SelectingItemsControl.SelectedItemProperty] = binding,
            };
        } else if (compareValueType == typeof(FormKey)) {
            control = GetFormKeyPicker(actualFieldType.GetGenericArguments());
        } else if (compareValueType.InheritsFrom(typeof(IFormLinkGetter))) {
            control = GetFormKeyPicker(actualFieldType.GetGenericArguments());
        } else if (compareValueType == typeof(long)) {
            control = new NumericUpDown {
                Minimum = long.MinValue,
                Maximum = long.MaxValue,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (compareValueType == typeof(ulong)) {
            control = new NumericUpDown {
                Minimum = ulong.MinValue,
                Maximum = ulong.MaxValue,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (compareValueType == typeof(int)) {
            control = new NumericUpDown {
                Minimum = int.MinValue,
                Maximum = int.MaxValue,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (compareValueType == typeof(uint)) {
            control = new NumericUpDown {
                Minimum = uint.MinValue,
                Maximum = uint.MaxValue,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (compareValueType == typeof(short)) {
            control = new NumericUpDown {
                Minimum = short.MinValue,
                Maximum = short.MaxValue,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (compareValueType == typeof(ushort)) {
            control = new NumericUpDown {
                Minimum = ushort.MinValue,
                Maximum = ushort.MaxValue,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (compareValueType == typeof(sbyte)) {
            control = new NumericUpDown {
                Minimum = sbyte.MinValue,
                Maximum = sbyte.MaxValue,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (compareValueType == typeof(byte)) {
            control = new NumericUpDown {
                Minimum = byte.MinValue,
                Maximum = byte.MaxValue,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else {
            control = new TextBlock {
                Text = $"No Condition is available for {param}"
            };
        }

        control.DataContext = param;
        control.HorizontalAlignment = HorizontalAlignment.Stretch;

        return control;

        Control GetFormKeyPicker(IEnumerable<Type> scopedTypes) {
            // todo add placed picker button game-independent
            // if (function.Type.InheritsFromAny(RecordTypeConstants.AllPlacedInterfaceTypes)) {
            //     return new PlacedPickerButton {
            //         [PlacedPickerButton.ScopedTypesProperty] = types,
            //         [!PlacedPickerButton.PlacedProperty] = new Binding($"{compareValueName}.{nameof(FormLinkOrIndex<IMajorRecordGetter>.Link)}"),
            //         [!PlacedPickerButton.LinkCacheProperty] = this[!LinkCacheProperty],
            //     };
            // }

            var formKeyPicker = new FormKeyPicker {
                [!AFormKeyPicker.LinkCacheProperty] = this[!LinkCacheProperty],
                ShowFormKeyBox = false,
                ScopedTypes = scopedTypes,
            };

            if (param is FormLinkValueCondition linkCondition) {
                if (linkCondition.CompareValue is IFormLinkGetter formLink) {
                    formKeyPicker[AFormKeyPicker.FormKeyProperty] = formLink.FormKey;
                }
                formKeyPicker[!AFormKeyPicker.FormLinkProperty] = binding;
            } else {
                formKeyPicker[!AFormKeyPicker.FormKeyProperty] = binding;

            }
            return formKeyPicker;
        }
    }
}
