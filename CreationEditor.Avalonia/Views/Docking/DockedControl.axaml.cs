using System.Reactive;
using Avalonia.Controls;
using Avalonia.Layout;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Docking;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Views.Docking; 

public partial class DockedControl : UserControl, IDockedItem {
    public Control Control { get; set; } = null!;
    public DockingManagerVM Root { get; set; } = null!;
    
    [Reactive] public string? Header { get; set; }
    [Reactive] public IconSource? IconSource { get; set; }

    [Reactive] public bool CanPin { get; set; } = true;
    [Reactive] public bool CanClose { get; set; } = true;

    [Reactive] public bool IsPinned { get; set; } = true;
    [Reactive] public bool IsSelected { get; set; }

    public ReactiveCommand<Unit, Unit> Close { get; }
    public ReactiveCommand<Unit, Unit> TogglePin { get; }

    public DockedControl() {
        InitializeComponent();
        
        Close = ReactiveCommand.Create(
            canExecute: this.WhenAnyValue(x => x.CanClose),
            execute: () => {
                Root?.Remove(this);
            });

        TogglePin = ReactiveCommand.Create(
            canExecute: this.WhenAnyValue(x => x.CanPin),
            execute: () => {
                IsPinned = !IsPinned;
            });
    }
    
    public DockedControl(IDockedItem vm) : this() {
        HorizontalAlignment = HorizontalAlignment.Stretch;
        VerticalAlignment = VerticalAlignment.Stretch;

        Control = vm.Control;
        Root = vm.Root;
        DataContext = vm;
    }
}
