using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using ReleasePipeline.ViewModels;
namespace ReleasePipeline.Resources.Converter;

/// <summary>
/// Builds the editor control for a <see cref="ConfigFieldVM"/> based on its property type, so
/// only the matching control (CheckBox / TextBox / ComboBox / NumericUpDown) is instantiated and
/// bound to the value.
/// </summary>
public sealed class ConfigFieldDataTemplate : IDataTemplate {
    private static readonly Binding ValueBinding = new("Value");

    public Control Build(object? param) {
        if (param is not ConfigFieldVM field) {
            return new TextBlock { Text = "Unknown config field" };
        }

        if (field.IsBool) {
            return new CheckBox {
                [!CheckBox.IsCheckedProperty] = ValueBinding,
            };
        }

        if (field.IsEnum) {
            return new ComboBox {
                ItemsSource = field.EnumValues,
                MinWidth = 200,
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(12, 8),
                [!ComboBox.SelectedItemProperty] = ValueBinding,
            };
        }

        if (field.IsString) {
            return new TextBox {
                MinWidth = 200,
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(12, 8),
                [!TextBox.TextProperty] = ValueBinding,
            };
        }

        if (field.IsNumeric) {
            return new NumericUpDown {
                MinWidth = 200,
                [!NumericUpDown.ValueProperty] = ValueBinding,
            };
        }

        return new TextBlock { Text = $"Unsupported field type: {field.PropertyType.Name}" };
    }

    public bool Match(object? data) => data is ConfigFieldVM;
}
