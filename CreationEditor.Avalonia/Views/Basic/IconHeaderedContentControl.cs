using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Views.Basic;

public class IconHeaderedContentControl : HeaderedContentControl {
    public static readonly StyledProperty<Symbol> IconProperty
        = AvaloniaProperty.Register<IconHeaderedContentControl, Symbol>(nameof(Icon));

    public Symbol Icon {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly StyledProperty<Control?> IconControlProperty
        = AvaloniaProperty.Register<IconHeaderedContentControl, Control?>(nameof(IconControl));

    public Control? IconControl {
        get => GetValue(IconControlProperty);
        set => SetValue(IconControlProperty, value);
    }

    public static readonly StyledProperty<string?> DescriptionProperty
        = AvaloniaProperty.Register<IconHeaderedContentControl, string?>(nameof(Description));

    public string? Description {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
}
