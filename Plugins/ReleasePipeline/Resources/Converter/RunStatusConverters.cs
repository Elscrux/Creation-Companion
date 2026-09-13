using Avalonia.Data.Converters;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using ReleasePipeline.Models.Actions;
namespace ReleasePipeline.Resources.Converter;

public static class RunStatusConverters {
    /// <summary>Maps a <see cref="RunStatus"/> to a Fluent symbol icon.</summary>
    public static readonly FuncValueConverter<RunStatus, FASymbol> ToSymbol
        = new(status => status switch {
            RunStatus.Running => FASymbol.Sync,
            RunStatus.Passed => FASymbol.Checkmark,
            RunStatus.Failed => FASymbol.Cancel,
            RunStatus.Warned => FASymbol.Alert,
            RunStatus.Skipped => FASymbol.Forward,
            _ => FASymbol.Clock,
        });

    /// <summary>Maps a <see cref="RunStatus"/> to a brush color.</summary>
    public static readonly FuncValueConverter<RunStatus, IBrush> ToBrush
        = new(status => status switch {
            RunStatus.Running => Brushes.CornflowerBlue,
            RunStatus.Passed => Brushes.ForestGreen,
            RunStatus.Failed => Brushes.IndianRed,
            RunStatus.Warned => Brushes.Goldenrod,
            RunStatus.Skipped => Brushes.Gray,
            _ => Brushes.Gray,
        });

    /// <summary>Maps a <see cref="RunStatus"/> to a human-readable label.</summary>
    public static readonly FuncValueConverter<RunStatus, string> ToStatusText
        = new(status => status switch {
            RunStatus.Running => "Running",
            RunStatus.Passed => "Done",
            RunStatus.Failed => "Failed",
            RunStatus.Warned => "Warned",
            RunStatus.Skipped => "Skipped",
            _ => "Pending",
        });

    /// <summary>Maps the run's <see cref="PipelineRunVM.IsRunning"/> to a status label.</summary>
    public static readonly FuncValueConverter<bool, string> ToRunStateText
        = new(isRunning => isRunning ? "Running…" : "Idle");

    /// <summary>Maps the run's <see cref="PipelineRunVM.Succeeded"/> to a result label.</summary>
    public static readonly FuncValueConverter<bool, string> ToResultText
        = new(succeeded => succeeded ? "Succeeded" : "Failed");

    /// <summary>True when a message is present (used to show/hide the info line).</summary>
    public static readonly FuncValueConverter<string?, bool> HasMessage
        = new(message => !string.IsNullOrWhiteSpace(message));
}
