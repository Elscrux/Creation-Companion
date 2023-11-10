using System.Drawing;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using CreationEditor.Avalonia.Converter;
using CreationEditor.Avalonia.Views.Query;
using CreationEditor.Avalonia.Views.Record.Picker;
using CreationEditor.Services.Query.Where;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Noggog;
namespace CreationEditor.Avalonia.DataTemplates;

public sealed class QueryConditionFieldEditorTemplate : AvaloniaObject, IDataTemplate {
    public static readonly StyledProperty<ILinkCache> LinkCacheProperty
        = AvaloniaProperty.Register<QueryConditionFieldEditorTemplate, ILinkCache>(nameof(LinkCache));

    public ILinkCache LinkCache {
        get => GetValue(LinkCacheProperty);
        set => SetValue(LinkCacheProperty, value);
    }

    public static readonly StyledProperty<IQueryConditionFactory> ConditionFactoryProperty
        = AvaloniaProperty.Register<QueryConditionFieldEditorTemplate, IQueryConditionFactory>(nameof(ConditionFactory));

    public IQueryConditionFactory ConditionFactory {
        get => GetValue(ConditionFactoryProperty);
        set => SetValue(ConditionFactoryProperty, value);
    }

    public bool Match(object? data) => data is ConditionState;

    public Control Build(object? param) {
        if (param is not ConditionState state) return new TextBlock { Text = $"No Condition is available for {param}" };

        var field = state.GetField();
        if (field is null) return new TextBlock { Text = $"No Field is available for {param}" };

        // Convert fields to controls
        var control = field switch {
            ValueFieldInformation value => GetValueControl(value, state),
            CollectionFieldInformation collection => GetCollectionControl(collection, state),
            _ => throw new ArgumentOutOfRangeException()
        };

        control.DataContext = state;
        control.HorizontalAlignment = HorizontalAlignment.Stretch;
        return control;
    }

    private Control GetValueControl(ValueFieldInformation value, ConditionState? state = null) {
        var binding = new Binding(value.Name);

        Control? control;
        if (value.TypeClass == typeof(bool)) {
            control = new CheckBox {
                [!ToggleButton.IsCheckedProperty] = binding
            };
        } else if (value.TypeClass == typeof(string)) {
            control = new TextBox {
                [!TextBox.TextProperty] = binding
            };
        } else if (value.TypeClass == typeof(float) || value.TypeClass == typeof(double)) {
            control = new NumericUpDown {
                Minimum = decimal.MinValue,
                Maximum = decimal.MaxValue,
                Increment = 0.1m,
                MinWidth = 128,
                FormatString = "N4",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (value.TypeClass == typeof(Enum)) {
            control = new ComboBox {
                ItemsSource = Enum.GetValues(value.ActualType).Cast<Enum>().OrderBy(x => x.ToString()).ToArray(),
                [!SelectingItemsControl.SelectedItemProperty] = binding,
            };
        } else if (value.TypeClass == typeof(FormKey)) {
            control = GetFormKeyPicker(value.ActualType.GetGenericArguments());
        } else if (value.TypeClass.InheritsFrom(typeof(IFormLinkGetter))) {
            control = GetFormKeyPicker(value.ActualType.GetGenericArguments());
        } else if (value.TypeClass == typeof(long)) {
            control = new NumericUpDown {
                Minimum = long.MinValue,
                Maximum = long.MaxValue,
                MinWidth = 128,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (value.TypeClass == typeof(ulong)) {
            control = new NumericUpDown {
                Minimum = ulong.MinValue,
                Maximum = ulong.MaxValue,
                MinWidth = 128,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (value.TypeClass == typeof(int)) {
            control = new NumericUpDown {
                Minimum = int.MinValue,
                Maximum = int.MaxValue,
                MinWidth = 128,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (value.TypeClass == typeof(uint)) {
            control = new NumericUpDown {
                Minimum = uint.MinValue,
                Maximum = uint.MaxValue,
                MinWidth = 128,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (value.TypeClass == typeof(short)) {
            control = new NumericUpDown {
                Minimum = short.MinValue,
                Maximum = short.MaxValue,
                MinWidth = 128,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (value.TypeClass == typeof(ushort)) {
            control = new NumericUpDown {
                Minimum = ushort.MinValue,
                Maximum = ushort.MaxValue,
                MinWidth = 128,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (value.TypeClass == typeof(sbyte)) {
            control = new NumericUpDown {
                Minimum = sbyte.MinValue,
                Maximum = sbyte.MaxValue,
                MinWidth = 128,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (value.TypeClass == typeof(byte)) {
            control = new NumericUpDown {
                Minimum = byte.MinValue,
                Maximum = byte.MaxValue,
                MinWidth = 128,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (value.TypeClass == typeof(Color)) {
            control = new ColorPickerButton {
                [!ColorPickerButton.ColorProperty] = new Binding(value.Name) {
                    Converter = new ExtendedFuncValueConverter<Color, global::Avalonia.Media.Color?, object?>(
                        (color, _) => global::Avalonia.Media.Color.FromArgb(color.A, color.R, color.G, color.B),
                        (color, _) => color.HasValue
                            ? Color.FromArgb(color.Value.A, color.Value.R, color.Value.G, color.Value.B)
                            : Color.White)
                },
            };
        } else {
            control = new TextBlock {
                Text = $"No Control is available for {value.TypeClass}"
            };
        }
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

            if (value.TypeClass.InheritsFrom(typeof(IFormLinkGetter))) {
                if (state is { CompareValue: FormLinkInformation formLink }) {
                    formKeyPicker[AFormKeyPicker.FormKeyProperty] = formLink.FormKey;
                } else {
                    formKeyPicker[AFormKeyPicker.FormKeyProperty] = FormKey.Null;
                }

                formKeyPicker[!AFormKeyPicker.FormLinkProperty] = binding;
            } else {
                if (state is { CompareValue: not FormKey }) {
                    formKeyPicker[AFormKeyPicker.FormKeyProperty] = FormKey.Null;
                    state.CompareValue = FormKey.Null;
                }

                formKeyPicker[!AFormKeyPicker.FormKeyProperty] = binding;
            }
            return formKeyPicker;
        }
    }

    private Button GetCollectionControl(CollectionFieldInformation collection, ConditionState state) {
        return new Button {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            [!ContentControl.ContentProperty] = state.SubConditions
                .SelectWhenCollectionChanges(() => state.Summary)
                .ThrottleMedium()
                .Select(state.GetFullSummary)
                .ToBinding(),
            Flyout = new Flyout {
                FlyoutPresenterClasses = {
                    "Flyout750x250"
                },
                Content = new QueryConditionsView {
                    ContextType = collection.ElementType,
                    QueryConditions = state.SubConditions,
                    [!QueryConditionsView.ConditionFactoryProperty] = this.GetObservable(ConditionFactoryProperty).ToBinding(),
                    [!QueryConditionsView.LinkCacheProperty] = this.GetObservable(LinkCacheProperty).ToBinding()
                }
            }
        };
    }
}
