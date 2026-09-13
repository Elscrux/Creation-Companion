using Avalonia.Data.Converters;
using Avalonia.Media;
using CreationEditor.Avalonia.Constants;
namespace CreationEditor.Avalonia.Converter;

public static class BrushConverters {
    public static readonly FuncValueConverter<bool, IBrush?> ToAccentBrush = new(d => d ? StandardBrushes.HighlightBrush : null);
    public static readonly FuncValueConverter<bool, IBrush?> ToValidBrush = new(d => d ? StandardBrushes.ValidBrush : null);
    public static readonly FuncValueConverter<bool, IBrush?> ToInvalidBrush = new(d => d ? StandardBrushes.InvalidBrush : null);
    public static readonly FuncValueConverter<bool, IBrush?> ToValidInvalidBrush = new(d => d ? StandardBrushes.ValidBrush : StandardBrushes.InvalidBrush);
    public static readonly FuncValueConverter<bool, IBrush?> ToBackgroundBrush = new(d => d ? StandardBrushes.BackgroundBrush : null);
}
