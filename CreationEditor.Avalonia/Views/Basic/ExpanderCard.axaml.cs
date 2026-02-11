using Avalonia;
using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Views.Basic;

public class ExpanderCard : Expander {
    public static readonly StyledProperty<Symbol> IconProperty
        = AvaloniaProperty.Register<ExpanderCard, Symbol>(nameof(Icon));

    public Symbol Icon {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly StyledProperty<Control?> IconControlProperty
        = AvaloniaProperty.Register<ExpanderCard, Control?>(nameof(IconControl));

    public Control? IconControl {
        get => GetValue(IconControlProperty);
        set => SetValue(IconControlProperty, value);
    }

    public static readonly StyledProperty<string?> DescriptionProperty
        = AvaloniaProperty.Register<ExpanderCard, string?>(nameof(Description));

    public string? Description {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
}
