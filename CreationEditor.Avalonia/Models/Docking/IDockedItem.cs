using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia.ViewModels.Docking;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Docking;

public interface IDockedItem : IDockObject, IEquatable<IDockedItem> {
    public Guid Id { get; }

    public Control Control { get; }
    
    public new DockContainerVM DockParent { get; set; }
    
    [Reactive] public bool IsSelected { get; set; }
    
    [Reactive] public string? Header { get; set; }
    [Reactive] public IconSource? IconSource { get; set; }
    
    public double? Size { get; set; }
    
    [Reactive] public bool CanClose { get; set; }
    public ReactiveCommand<Unit, IObservable<IDockedItem>> Close { get; }

    public DisposableCounterLock RemovalLock { get; }
    
    public IObservable<IDockedItem> Closed { get; }
}
