using System.Reactive.Subjects;
using Avalonia.Controls;
using Avalonia.Media;
using CreationEditor.Avalonia.Views.Tab;
namespace CreationEditor.Avalonia.ViewModels.Docking;

/// <summary>
/// this is a dock where you also have side bars/trays and you can add any docks to it
/// </summary>
public class DockingManagerVM : LayoutDockVM {
    public void OnControlRemoved(Control control) {
        _closed.OnNext(control);
    }
    
    private readonly Subject<Control> _closed = new();
    public IObservable<Control> Closed => _closed;

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
        Background = Brushes.Blue,
        RowDefinitions = new RowDefinitions { new(GridLength.Star) { MinHeight = MinLayoutSize } },
        ColumnDefinitions = new ColumnDefinitions { new(GridLength.Star) { MinWidth = MinLayoutSize } },
    };

    public TabStackVM Tabs { get; } = new();
    public TabStackVM Tabs2 { get; } = new();
    public TabStackVM Tabs3 { get; } = new();
    public TabStackVM Tabs4 { get; } = new();

    public DockingManagerVM() {
        Tabs.Tabs.Add(new TabStackTab {
                Header = "test1",
                Control = Top
            });
        
        Tabs.Tabs.Add(new TabStackTab {
                Header = "test2",
                Control = Top2
            });
        
        
        Tabs2.Tabs.Add(new TabStackTab {
                Header = "AAAAAA",
                Control = Right
            });
        
        Tabs2.Tabs.Add(new TabStackTab {
                Header = "BBBBBB",
                Control = Bottom
            });
        
        
        Tabs3.Tabs.Add(new TabStackTab {
                Header = "CCCCCC",
                Control = Right
            });
        
        Tabs3.Tabs.Add(new TabStackTab {
                Header = "DDDD",
                Control = Bottom
            });
        
        
        Tabs4.Tabs.Add(new TabStackTab {
                Header = "EEEEEEE",
                Control = Right
            });
        
        Tabs4.Tabs.Add(new TabStackTab {
                Header = "FFFF",
                Control = Bottom
            });
    }
}
