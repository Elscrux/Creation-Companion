using Avalonia.Data.Converters;
using Avalonia.Media;
using CreationEditor.Avalonia.Constants;
namespace ReleasePipeline.Resources.Converter;

public static class SelectionConverters {
    /// <summary>
    /// Maps a selection flag to a subtle gray highlight brush (used for the currently visible
    /// pipeline card in the right pane, whether editing or running).
    /// </summary>
    public static readonly FuncValueConverter<bool, IBrush?> ToActiveBrush
        = new(active => active ? StandardBrushes.BackgroundBrush : null);

    /// <summary>
    /// Maps a "has run" flag to a wider card width, so the card grows when the progress bar shows.
    /// </summary>
    public static readonly FuncValueConverter<bool, double> ToCardWidth
        = new(hasRun => hasRun ? 220 : 180);
}
