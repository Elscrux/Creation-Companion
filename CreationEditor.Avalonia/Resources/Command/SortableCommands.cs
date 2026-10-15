using System.Reactive;
using ReactiveUI;
using Sortable.Avalonia;
namespace CreationEditor.Avalonia.Command;

public static class SortableCommands {
    public static readonly ReactiveCommand<SortableUpdateEventArgs, Unit> UpdateCommand
        = ReactiveCommand.Create<SortableUpdateEventArgs>(e => e.ApplyUpdateMutation());
}
