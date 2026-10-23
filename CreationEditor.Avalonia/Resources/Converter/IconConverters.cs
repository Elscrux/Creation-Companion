using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Converter;

public static class IconConverters {
    public static Control? ToIcon(object? icon) => icon switch {
        null => null,
        FASymbol symbol => new FASymbolIcon { Symbol = symbol },
        FAIconSource source => new FAIconSourceElement { IconSource = source },
        FAIconElement element => element,
        Control control => control,
        _ => new FAFontIcon { Glyph = icon.ToString() },
    };

    public static FAIconSource? ToIconSource(object? icon) => icon switch {
        null => null,
        FASymbol symbol => new FASymbolIconSource { Symbol = symbol },
        FAIconSource source => source,
        _ => new FAFontIconSource { Glyph = icon.ToString() },
    };
}
