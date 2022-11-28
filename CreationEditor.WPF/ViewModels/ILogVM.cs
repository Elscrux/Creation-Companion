using System.Reactive;
using Avalonia.Media;
using Elscrux.Logging.Sinks;
using ReactiveUI;
namespace CreationEditor.WPF.ViewModels;

public interface ILogVM : ILogListSink {
    public ReactiveCommand<Unit, Unit> ClearCommand { get; }

    public static ISolidColorBrush VerboseBrush { get; } = Brushes.CornflowerBlue;
    public static ISolidColorBrush DebugBrush { get; } = Brushes.ForestGreen;
    public static ISolidColorBrush MessageBrush { get; } = Brushes.White;
    public static ISolidColorBrush WarningBrush { get; } = Brushes.Yellow;
    public static ISolidColorBrush ErrorBrush { get; } =   Brushes.Red;
    public static ISolidColorBrush FatalBrush { get; } =   Brushes.DarkRed;
}