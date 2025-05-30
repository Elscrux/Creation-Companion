using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.Constants;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Docking;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views.Docking;

[DebuggerDisplay("Header = {Header}")]
public partial class DockedControl : ReactiveUserControl<IDockedItem>, IDockedItem, IDockPreview {
    public IDockedItem DockedItem { get; } = null!;

    public Guid Id => DockedItem.Id;

    public Control Control => DockedItem.Control;
    public DockContainerVM DockParent {
        get => DockedItem.DockParent;
        set => DockedItem.DockParent = value;
    }

    public bool IsSelected {
        get => DockedItem.IsSelected;
        set => DockedItem.IsSelected = value;
    }

    public string? Header {
        get => DockedItem.Header;
        set => DockedItem.Header = value;
    }

    public IconSource? IconSource {
        get => DockedItem.IconSource;
        set => DockedItem.IconSource = value;
    }

    public double? Size {
        get => DockedItem.Size;
        set => DockedItem.Size = value;
    }

    public bool CanClose {
        get => DockedItem.CanClose;
        set => DockedItem.CanClose = value;
    }

    public ReactiveCommand<Unit, IObservable<IDockedItem>> Close => DockedItem.Close;

    public DisposableCounterLock RemovalLock => DockedItem.RemovalLock;

    public IObservable<IDockedItem> Closed => DockedItem.Closed;

    public DockedControl() {
        InitializeComponent();
    }

    public DockedControl(IDockedItem vm) {
        DataContext = DockedItem = vm;
        Name = Header = vm.Header;

        HorizontalAlignment = HorizontalAlignment.Stretch;
        VerticalAlignment = VerticalAlignment.Stretch;

        this.WhenActivated(d => {
            EventHandler<LogicalTreeAttachmentEventArgs> checkRemoved = (_, _) => CheckRemoved();
            DetachedFromLogicalTree += checkRemoved;
            Control.DetachedFromLogicalTree += checkRemoved;

            d.Add(Disposable.Create(() => DetachedFromLogicalTree -= checkRemoved));
            d.Add(Disposable.Create(() => Control.DetachedFromLogicalTree -= checkRemoved));
        });

        InitializeComponent();
    }

    private void CheckRemoved() {
        if (RemovalLock.IsLocked()
         || GetValue(VisualParentProperty) is not null
         || Control.GetValue(VisualParentProperty) is not null
         || DockParent.TryGetDock(Control, out _)) return;

        (this as IDockObject).DockRoot.OnDockRemoved(this);
    }

    public void ShowPreview(Dock dock) {
        var grid = new Grid {
            Children = {
                new Rectangle {
                    IsHitTestVisible = false,
                    Opacity = 0.5,
                    Fill = StandardBrushes.HighlightBrush,
                    [Grid.RowProperty] = 1,
                },
            },
        };
        switch (dock) {
            case Dock.Top:
                grid.RowDefinitions = [new RowDefinition(), new RowDefinition()];
                grid.Children[0].SetValue(Grid.RowProperty, 0);
                break;
            case Dock.Bottom:
                grid.RowDefinitions = [new RowDefinition(), new RowDefinition()];
                grid.Children[0].SetValue(Grid.RowProperty, 1);
                break;
            case Dock.Left:
                grid.ColumnDefinitions = [new ColumnDefinition(), new ColumnDefinition()];
                grid.Children[0].SetValue(Grid.ColumnProperty, 0);
                break;
            case Dock.Right:
                grid.ColumnDefinitions = [new ColumnDefinition(), new ColumnDefinition()];
                grid.Children[0].SetValue(Grid.ColumnProperty, 1);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dock), dock, null);
        }

        AdornerLayer.SetAdorner(this, grid);
    }

    public bool Equals(IDockedItem? other) {
        return Id == other?.Id;
    }
}
