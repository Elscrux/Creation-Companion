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

public sealed class ConditionValueEditorTemplate : AvaloniaObject, IDataTemplate {
    public static readonly StyledProperty<ILinkCache> LinkCacheProperty
        = AvaloniaProperty.Register<ConditionValueEditorTemplate, ILinkCache>(nameof(LinkCache));

    public ILinkCache LinkCache {
        get => GetValue(LinkCacheProperty);
        set => SetValue(LinkCacheProperty, value);
    }

    public static readonly StyledProperty<IQueryConditionFactory> ConditionFactoryProperty
        = AvaloniaProperty.Register<ConditionValueEditorTemplate, IQueryConditionFactory>(nameof(ConditionFactory));

    public IQueryConditionFactory ConditionFactory {
        get => GetValue(ConditionFactoryProperty);
        set => SetValue(ConditionFactoryProperty, value);
    }

    public bool Match(object? data) => data is ConditionState;

    public Control Build(object? param) {
        if (param is not ConditionState state) return new TextBlock { Text = $"No Condition is available for {param}" };

        // Convert fields to controls
        var fields = state.GetFields().ToArray();
        var controls = fields
            .Select<FieldType, Control?>(field => {
                Control? primitiveControl = null;
                Control? complexControl = null;
                if (field.FieldCategory == FieldCategory.Value) {
                    primitiveControl = GetPrimitiveControl(field, new Binding(ConditionState.FieldCategoryToName(field.FieldCategory)), state);
                }

                if (field.FieldCategory == FieldCategory.Collection) {
                    if (state.SubConditions.Count == 0) {
                        state.SubConditions.Add(ConditionFactory.Create(field.ActualType));
                    }

                    var queryConditionsView = new QueryConditionsView {
                        ContextType = field.ActualType,
                        QueryConditions = state.SubConditions,
                        [!QueryConditionsView.ConditionFactoryProperty] = this.GetObservable(ConditionFactoryProperty).ToBinding(),
                        [!QueryConditionsView.LinkCacheProperty] = this.GetObservable(LinkCacheProperty).ToBinding()
                    };

                    complexControl = new Button {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        [!ContentControl.ContentProperty] = state.SubConditions
                            .Select(x => x.Summary)
                            .CombineLatest()
                            .Select(list => string.Join(' ', list))
                            .ToBinding(),
                        Flyout = new Flyout {
                            FlyoutPresenterClasses = { "Flyout750x250" },
                            Content = WrapWithName(queryConditionsView)
                        }
                    };
                }

                if (complexControl is null) {
                    return primitiveControl is not null
                        ? fields.Length > 1
                            ? WrapWithName(primitiveControl)
                            : primitiveControl
                        : new TextBlock { Text = $"No Condition is available for {state}" };
                }

                if (primitiveControl is null) return complexControl;

                // If both primitive and complex controls are available
                // return a stack panel with both controls
                var checkBox = new CheckBox { [ToolTip.TipProperty] = "Advanced" };
                var checkedObservable = checkBox.GetObservable(ToggleButton.IsCheckedProperty);
                primitiveControl[!Visual.IsVisibleProperty] = checkedObservable.Negate().ToBinding();
                complexControl[!Visual.IsVisibleProperty] = checkedObservable.ToBinding();
                var stackPanel = new StackPanel {
                    Orientation = Orientation.Horizontal,
                    Children = {
                        checkBox,
                        new StackPanel {
                            Orientation = Orientation.Horizontal,
                            Children = { primitiveControl, complexControl }
                        }
                    }
                };
                return fields.Length > 1
                    ? WrapWithName(stackPanel)
                    : stackPanel;

                Control WrapWithName(Control c) {
                    return fields.Length > 1
                        ? new StackPanel {
                            Children = {
                                new TextBlock { Text = field.ActualType.Name },
                                c
                            }
                        }
                        : c;
                }

            })
            .NotNull()
            .Select(c => {
                c.DataContext = state;
                c.HorizontalAlignment = HorizontalAlignment.Stretch;
                return c;
            })
            .ToList();

        // For single controls, return the control
        if (controls is [var control]) return control;

        // For multiple controls, return a stack panel
        var stackPanel = new StackPanel();
        foreach (var c in controls) {
            stackPanel.Children.Add(c);
        }
        return stackPanel;
    }

    private Control? GetPrimitiveControl(FieldType fieldType, IBinding binding, ConditionState? state = null) {
        Control? control = null;
        if (fieldType.TypeClass == typeof(bool)) {
            control = new CheckBox {
                [!ToggleButton.IsCheckedProperty] = binding
            };
        } else if (fieldType.TypeClass == typeof(string)) {
            control = new TextBox {
                [!TextBox.TextProperty] = binding
            };
        } else if (fieldType.TypeClass == typeof(float) || fieldType.TypeClass == typeof(double)) {
            control = new NumericUpDown {
                Minimum = decimal.MinValue,
                Maximum = decimal.MaxValue,
                Increment = 0.1m,
                MinWidth = 128,
                FormatString = "N4",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (fieldType.TypeClass == typeof(Enum)) {
            control = new ComboBox {
                ItemsSource = Enum.GetValues(fieldType.ActualType).Cast<Enum>().OrderBy(x => x.ToString()).ToArray(),
                [!SelectingItemsControl.SelectedItemProperty] = binding,
            };
        } else if (fieldType.TypeClass == typeof(FormKey)) {
            control = GetFormKeyPicker(fieldType.ActualType.GetGenericArguments());
        } else if (fieldType.TypeClass.InheritsFrom(typeof(IFormLinkGetter))) {
            control = GetFormKeyPicker(fieldType.ActualType.GetGenericArguments());
        } else if (fieldType.TypeClass == typeof(long)) {
            control = new NumericUpDown {
                Minimum = long.MinValue,
                Maximum = long.MaxValue,
                MinWidth = 128,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (fieldType.TypeClass == typeof(ulong)) {
            control = new NumericUpDown {
                Minimum = ulong.MinValue,
                Maximum = ulong.MaxValue,
                MinWidth = 128,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (fieldType.TypeClass == typeof(int)) {
            control = new NumericUpDown {
                Minimum = int.MinValue,
                Maximum = int.MaxValue,
                MinWidth = 128,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (fieldType.TypeClass == typeof(uint)) {
            control = new NumericUpDown {
                Minimum = uint.MinValue,
                Maximum = uint.MaxValue,
                MinWidth = 128,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (fieldType.TypeClass == typeof(short)) {
            control = new NumericUpDown {
                Minimum = short.MinValue,
                Maximum = short.MaxValue,
                MinWidth = 128,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (fieldType.TypeClass == typeof(ushort)) {
            control = new NumericUpDown {
                Minimum = ushort.MinValue,
                Maximum = ushort.MaxValue,
                MinWidth = 128,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (fieldType.TypeClass == typeof(sbyte)) {
            control = new NumericUpDown {
                Minimum = sbyte.MinValue,
                Maximum = sbyte.MaxValue,
                MinWidth = 128,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (fieldType.TypeClass == typeof(byte)) {
            control = new NumericUpDown {
                Minimum = byte.MinValue,
                Maximum = byte.MaxValue,
                MinWidth = 128,
                FormatString = "N0",
                [!NumericUpDown.ValueProperty] = binding
            };
        } else if (fieldType.TypeClass == typeof(Color)) {
            control = new ColorPickerButton {
                [!ColorPickerButton.ColorProperty] = new Binding(ConditionState.FieldCategoryToName(fieldType.FieldCategory)) {
                    Converter = new ExtendedFuncValueConverter<Color, global::Avalonia.Media.Color?, object?>(
                        (color, _) => global::Avalonia.Media.Color.FromArgb(color.A, color.R, color.G, color.B),
                        (color, _) => color.HasValue
                            ? Color.FromArgb(color.Value.A, color.Value.R, color.Value.G, color.Value.B)
                            : Color.White)
                },
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

            if (fieldType.TypeClass.InheritsFrom(typeof(IFormLinkGetter))) {
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
}
