using System.Reactive;
using ReactiveUI;
using Sortable.Avalonia;
namespace CreationEditor.Avalonia.Command;

public static class SortableCommands {
    public static readonly ReactiveCommand<SortableUpdateEventArgs, Unit> UpdateCommand
        = ReactiveCommand.Create<SortableUpdateEventArgs>(e => e.ApplyUpdateMutation());
    public static readonly ReactiveCommand<SortableDropEventArgs, Unit> DropCommand
        = ReactiveCommand.Create<SortableDropEventArgs>(e => e.ApplyDropMutation());
}
