using Avalonia.Data.Converters;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using ReleasePipeline.Models.Check;
namespace ReleasePipeline.Resources.Converter;

public static class CheckStatusConverters {
    /// <summary>Maps a <see cref="CheckStatus"/> to a Fluent symbol icon.</summary>
    public static readonly FuncValueConverter<CheckStatus, FASymbol> ToSymbol
        = new(status => status switch {
            CheckStatus.Passed => FASymbol.Checkmark,
            CheckStatus.Failed => FASymbol.Cancel,
            CheckStatus.Warned => FASymbol.Alert,
            CheckStatus.Running => FASymbol.Sync,
            _ => FASymbol.Clock,
        });

    /// <summary>Maps a <see cref="CheckStatus"/> to a brush color.</summary>
    public static readonly FuncValueConverter<CheckStatus, IBrush> ToBrush
        = new(status => status switch {
            CheckStatus.Passed => Brushes.ForestGreen,
            CheckStatus.Failed => Brushes.IndianRed,
            CheckStatus.Warned => Brushes.Goldenrod,
            CheckStatus.Running => Brushes.CornflowerBlue,
            _ => Brushes.Gray,
        });
}
