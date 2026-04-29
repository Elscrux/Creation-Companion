using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using CreationEditor.Avalonia.Models.Docking;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Docking;

public sealed partial class DockedItemVM : ViewModel, IDockedItem {
    public Guid Id { get; }

    public Control Control { get; }

    public DockContainerVM DockParent { get; set; } = null!;

    [Reactive] public partial string? Header { get; set; }
    [Reactive] public partial IconSource? IconSource { get; set; }
    public double? Size { get; set; }

    [Reactive] public partial bool IsSelected { get; set; }

    [Reactive] public partial bool CanClose { get; set; }
    private IObservable<bool> CanCloseObservable => this.WhenAnyValue(x => x.CanClose);
    public ReactiveCommand<Unit, IObservable<IDockedItem>> CloseCommand { get; }

    public DisposableCounterLock RemovalLock { get; }


    private readonly Subject<IDockedItem> _closed = new();
    public IObservable<IDockedItem> Closed => _closed;

    public DockedItemVM(Control control, DockInfo info) {
        Id = Guid.NewGuid();
        Control = control;
        Header = info.Header;
        IconSource = info.IconSource;
        Size = info.Size;
        CanClose = info.CanClose;

        RemovalLock = new DisposableCounterLock(CheckRemoved);
        
        CloseCommand = ReactiveCommand.Create(Close, CanCloseObservable);

        Control.DetachedFromLogicalTree += CheckRemoved;
    }

    private IObservable<IDockedItem> Close() {
        var oneTimeSubscription = Closed.Take(1);

        DockParent.Remove(this);

        return oneTimeSubscription;
    }

    private void CheckRemoved(object? o, LogicalTreeAttachmentEventArgs logicalTreeAttachmentEventArgs) => CheckRemoved();
    private void CheckRemoved() {
        if (RemovalLock.IsLocked()
         || Control.GetValue(Visual.VisualParentProperty) is not null
         || DockParent.TryGetDock(Control, out _)) return;

        _closed.OnNext(this);
        (this as IDockObject).DockRoot.OnDockRemoved(this);
        Control.DetachedFromLogicalTree -= CheckRemoved;
    }

    public bool Equals(IDockedItem? other) {
        return Id == other?.Id;
    }
}
