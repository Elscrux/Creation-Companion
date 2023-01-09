using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Visual = Avalonia.Visual;
namespace CreationEditor.Avalonia.ViewModels.Docking;

public sealed class DockedItemVM : ViewModel, IDockedItem {
    public Guid Id { get; }

    public Control Control { get; }

    public DockContainerVM DockParent { get; set; } = null!;

    [Reactive] public string? Header { get; set; }
    [Reactive] public IconSource? IconSource { get; set; }
    
    [Reactive] public bool IsSelected { get; set; }

    [Reactive] public bool CanClose { get; set; }
    public ReactiveCommand<Unit, IObservable<IDockedItem>> Close { get; }

    public DisposableCounterLock RemovalLock { get; }
    
    
    private readonly Subject<IDockedItem> _closed = new();
    public IObservable<IDockedItem> Closed => _closed;

    public DockedItemVM(Control control, DockInfo info) {
        Id = Guid.NewGuid();
        Control = control;
        Header = info.Header;
        IconSource = info.IconSource;
        CanClose = info.CanClose;

        RemovalLock = new DisposableCounterLock(CheckRemoved);

        Control.DetachedFromLogicalTree += (_, _) => CheckRemoved();

        Close = ReactiveCommand.Create(
            canExecute: this.WhenAnyValue(x => x.CanClose),
            execute: () => {
                var oneTimeSubscription = Closed.Take(1);
                
                DockParent.Remove(this);

                return oneTimeSubscription;
            });
    }

    private void CheckRemoved() {
        if (RemovalLock.IsLocked()
         || Control.GetValue(Visual.VisualParentProperty) != null
         || DockParent.TryGetDock(Control, out _)) return;

        _closed.OnNext(this);
        (this as IDockObject).DockRoot.OnDockRemoved(this);
    }

    public bool Equals(IDockedItem? other) {
        return Id == other?.Id;
    }
}
