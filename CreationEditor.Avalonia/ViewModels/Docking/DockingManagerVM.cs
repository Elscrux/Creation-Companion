using System.Reactive.Subjects;
using Avalonia.Controls;
using Avalonia.Media;
using CreationEditor.Avalonia.Models.Docking;
namespace CreationEditor.Avalonia.ViewModels.Docking;

/// <summary>
/// this is a dock where you also have side bars/trays and you can add any docks to it
/// </summary>
public class DockingManagerVM : LayoutDockVM {
    public void OnRemoved(IDockedItem dockedItem) {
        _closed.OnNext(dockedItem);
    }
    
    private readonly Subject<IDockedItem> _closed = new();
    public IObservable<IDockedItem> Closed => _closed;
    
    public new void Add(IDockedItem dockedItem, DockConfig config) {
        switch (config.DockType) {
            case DockType.Layout:
                AddDockedControl(CreateControl(dockedItem), config);
                break;
            case DockType.Document:
                AddDocumentControl(dockedItem, config);
                break;
            case DockType.Side:
                AddSideControl(dockedItem, config);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void AddSideControl(IDockedItem dockedItem, DockConfig config) {
        var tabGroup = config.Dock switch {
            Dock.Top => TopSide,
            Dock.Bottom => BottomSide,
            Dock.Left => LeftSide,
            Dock.Right => RightSide,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        tabGroup.Add(dockedItem, config);
    }

    public Grid Top { get; } = new() {
        Background = Brushes.Purple,
        RowDefinitions = new RowDefinitions { new(GridLength.Star) { MinHeight = MinLayoutSize } },
        ColumnDefinitions = new ColumnDefinitions { new(GridLength.Star) { MinWidth = MinLayoutSize } },
    };
    public Grid Top2 { get; } = new() {
        Background = Brushes.Red,
        RowDefinitions = new RowDefinitions { new(GridLength.Star) { MinHeight = MinLayoutSize } },
        ColumnDefinitions = new ColumnDefinitions { new(GridLength.Star) { MinWidth = MinLayoutSize } },
    };
    public Grid Bottom { get; } = new() {
        Background = Brushes.Green,
        RowDefinitions = new RowDefinitions { new(GridLength.Star) { MinHeight = MinLayoutSize } },
        ColumnDefinitions = new ColumnDefinitions { new(GridLength.Star) { MinWidth = MinLayoutSize } },
    };
    public Grid Left { get; } = new() {
        Background = Brushes.Yellow,
        RowDefinitions = new RowDefinitions { new(GridLength.Star) { MinHeight = MinLayoutSize } },
        ColumnDefinitions = new ColumnDefinitions { new(GridLength.Star) { MinWidth = MinLayoutSize } },
    };
    public Grid Right { get; } = new() {
        Background = Brushes.Orange,
        RowDefinitions = new RowDefinitions { new(GridLength.Star) { MinHeight = MinLayoutSize } },
        ColumnDefinitions = new ColumnDefinitions { new(GridLength.Star) { MinWidth = MinLayoutSize } },
    };

    public SidePanelVM TopSide { get; } = new();
    public SidePanelVM LeftSide { get; } = new();
    public SidePanelVM RightSide { get; } = new();
    public SidePanelVM BottomSide { get; } = new();

    public DockingManagerVM() {
        TopSide.Tabs.Add(new DockedItem(Top, this, new DockConfig()) {
                Header = "test1",
            });
        
        TopSide.Tabs.Add(new DockedItem(Bottom, this, new DockConfig()) {
                Header = "test2",
            });
        
        
        LeftSide.Tabs.Add(new DockedItem(Left, this, new DockConfig()) {
                Header = "AAAAAA",
            });
        
        LeftSide.Tabs.Add(new DockedItem(Top, this, new DockConfig()) {
                Header = "BBBBBB",
            });
        
        
        RightSide.Tabs.Add(new DockedItem(Top, this, new DockConfig()) {
                Header = "CCCCCC",
            });
        
        RightSide.Tabs.Add(new DockedItem(Top, this, new DockConfig()) {
                Header = "DDDD",
            });
        
        
        BottomSide.Tabs.Add(new DockedItem(Bottom, this, new DockConfig()) {
                Header = "EEEEEEE",
            });
        
        BottomSide.Tabs.Add(new DockedItem(Left, this, new DockConfig()) {
                Header = "FFFF",
            });
    }
}
