using System.Collections;
using System.Reactive;
using CreationEditor.Avalonia.Models.Selectables;
using ReactiveUI;
namespace CreationEditor.Avalonia.Command;

public static class SelectableCommand {
    public static readonly ReactiveCommand<IEnumerable?, Unit> SelectAll =
        ReactiveCommand.Create<IEnumerable?>(selectables => {
            if (selectables is null) return;

            foreach (var selectable in selectables.Cast<IReactiveSelectable>()) {
                selectable.IsSelected = true;
            }
        });

    public static readonly ReactiveCommand<IEnumerable?, Unit> UnselectAll =
        ReactiveCommand.Create<IEnumerable?>(selectables => {
            if (selectables is null) return;

            foreach (var selectable in selectables.Cast<IReactiveSelectable>()) {
                selectable.IsSelected = false;
            }
        });

    public static readonly ReactiveCommand<IEnumerable?, Unit> ToggleAll =
        ReactiveCommand.Create<IEnumerable?>(selectables => {
            if (selectables is null) return;

            foreach (var selectable in selectables.Cast<IReactiveSelectable>()) {
                selectable.IsSelected = !selectable.IsSelected;
            }
        });
}
