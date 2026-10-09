using Avalonia.Data.Converters;
using Avalonia.Media;
using CreationEditor.Avalonia.Constants;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
namespace CreationEditor.Avalonia.Converter;

public static class SeverityConverters {
    public static readonly IValueConverter ToSolidBrush = new FuncValueConverter<Severity, IBrush>(SeverityBrushes.Solid);
    public static readonly IValueConverter ToRowFillBrush = new FuncValueConverter<Severity, IBrush>(SeverityBrushes.RowFill);
    public static readonly IValueConverter ToRowHoverStrokeBrush = new FuncValueConverter<Severity, IBrush>(SeverityBrushes.RowHoverStroke);
    public static readonly IValueConverter ToRowHoverFillBrush = new FuncValueConverter<Severity, IBrush>(SeverityBrushes.RowHoverFill);
    public static readonly IValueConverter ToRowLipBrush = new FuncValueConverter<Severity, IBrush>(SeverityBrushes.RowLip);
}
