using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;
namespace CreationEditor.Avalonia.Command;

public class WindowCommands {
    public static readonly ReactiveCommand<Window?, Unit> Close =
        ReactiveCommand.Create<Window?>(window => window?.Close());
}
