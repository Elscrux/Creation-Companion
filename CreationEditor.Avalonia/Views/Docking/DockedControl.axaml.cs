using System.Diagnostics;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using Avalonia.ReactiveUI;
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
        
        DetachedFromLogicalTree += (_, _) => CheckRemoved();
        Control.DetachedFromLogicalTree += (_, _) => CheckRemoved();
        
        InitializeComponent();
    }

    private void CheckRemoved() {
        if (RemovalLock.IsLocked()
         || GetValue(VisualParentProperty) != null
         || Control.GetValue(VisualParentProperty) != null
         || DockParent.TryGetDock(Control, out _)) return;

        (this as IDockObject).DockRoot.OnDockRemoved(this);
    }
    
    public void ShowPreview(Dock dock) {
        var grid = new Grid {
            Children = {
                new Rectangle {
                    IsHitTestVisible = false,
                    Opacity = 0.5,
                    Fill = (this as IDockPreview).Brush,
                    [Grid.RowProperty] = 1,
                }
            }
        };
        switch (dock) {
            case Dock.Top:
                grid.RowDefinitions = new RowDefinitions { new(), new() };
                grid.Children[0].SetValue(Grid.RowProperty, 0);
                break;
            case Dock.Bottom:
                grid.RowDefinitions = new RowDefinitions { new(), new() };
                grid.Children[0].SetValue(Grid.RowProperty, 1);
                break;
            case Dock.Left:
                grid.ColumnDefinitions = new ColumnDefinitions { new(), new() };
                grid.Children[0].SetValue(Grid.ColumnProperty, 0);
                break;
            case Dock.Right:
                grid.ColumnDefinitions = new ColumnDefinitions { new(), new() };
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

