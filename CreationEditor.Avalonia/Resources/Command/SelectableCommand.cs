using System.Reactive;
using CreationEditor.Avalonia.Models;
using ReactiveUI;
namespace CreationEditor.Avalonia.Command; 

public static class SelectableCommand {
    public static readonly ReactiveCommand<IEnumerable<IReactiveSelectable>, Unit> SelectAll =
        ReactiveCommand.Create<IEnumerable<IReactiveSelectable>>(selectables => {
            foreach (var selectable in selectables) {
                selectable.IsSelected = true;
            }
        });
    
    public static readonly ReactiveCommand<IEnumerable<IReactiveSelectable>, Unit> UnselectAll =
        ReactiveCommand.Create<IEnumerable<IReactiveSelectable>>(selectables => {
            foreach (var selectable in selectables) {
                selectable.IsSelected = false;
            }
        });
    
    public static readonly ReactiveCommand<IEnumerable<IReactiveSelectable>, Unit> ToggleAll =
        ReactiveCommand.Create<IEnumerable<IReactiveSelectable>>(selectables => {
            foreach (var selectable in selectables) {
                selectable.IsSelected = !selectable.IsSelected;
            }
        });
}
