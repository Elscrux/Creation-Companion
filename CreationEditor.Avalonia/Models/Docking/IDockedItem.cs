using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia.ViewModels.Docking;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Docking; 

public interface IDockedItem {
    public Control Control { get; set; }
    public DockingManagerVM Root { get; set; }
    
    [Reactive] public string? Header { get; set; }
    [Reactive] public IconSource? IconSource { get; set; }
    
    [Reactive] public bool CanClose { get; set; }
    [Reactive] public bool CanPin { get; set; }
    
    [Reactive] public bool IsPinned { get; set; }
    [Reactive] public bool IsSelected { get; set; }

    public ReactiveCommand<Unit, Unit> Close { get; }
    public ReactiveCommand<Unit, Unit> TogglePin { get; }
}
