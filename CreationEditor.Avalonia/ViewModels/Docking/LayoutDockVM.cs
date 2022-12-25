using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Views.Docking;
namespace CreationEditor.Avalonia.ViewModels.Docking;

public class LayoutDockVM : ViewModel, IDockContainerVM {
    protected const int MinLayoutSize = 150;

    private static readonly DockConfig DefaultDockConfig = new();
    
    public Grid LayoutGrid { get; } = new() {
        RowDefinitions = new RowDefinitions { new(GridLength.Star) { MinHeight = MinLayoutSize } },
        ColumnDefinitions = new ColumnDefinitions { new(GridLength.Star) { MinWidth = MinLayoutSize } },
    };

    protected LayoutDockVM() {}
    public LayoutDockVM(IDockedItem dockedControlVM) : this() {
        LayoutGrid.Children.Add(new DockedControl(dockedControlVM));
    }

    public static Control CreateControl(IDockedItem dockedItem) => new LayoutDock(new LayoutDockVM(dockedItem));

    public int ChildrenCount => LayoutGrid.Children.Count;
    
    public DockingManagerVM GetRoot() {
        var dockedControl = LayoutGrid.Children.OfType<DockedControl>().FirstOrDefault();
        return dockedControl == null
            ? LayoutGrid.FindLogicalAncestorOfType<DockingManager>()?.ViewModel!
            : dockedControl.Root;

    }

    public bool TryGetDock(Control control, out IDockedItem outDock) {
        outDock = null!;
        
        if (ChildrenCount > 1) {
            foreach (var layoutChild in LayoutGrid.Children) {
                if (layoutChild.DataContext is IDockContainerVM dockContainerVM && dockContainerVM.TryGetDock(control, out var dock)) {
                    outDock = dock;
                    return true;
                }
            }
        } else if (LayoutGrid.Children[0] is DockedControl dockedControl && ReferenceEquals(dockedControl.Control, control)) {
            outDock = dockedControl;
            return true;
        }
        
        return false;
    }
    
    public void Focus(IDockedItem dockedItem) {
        foreach (var child in LayoutGrid.Children.OfType<Control>()) {
            if (child is GridSplitter || child.DataContext is not IDockContainerVM dockableVM) continue;
            
            if (ReferenceEquals(dockedItem, child) && child is DockedControl dockedControl) {
                dockedControl.Focus();
            } else {
                dockableVM.Focus(dockedItem);
            }
        }
    }

    public void Add(IDockedItem dockedItem, DockConfig config) {
        switch (config.DockType) {
            case DockType.Layout:
                AddDockedControl(CreateControl(dockedItem), config);
                break;
            case DockType.Document:
                AddDocumentControl(dockedItem, config);
                break;
            case DockType.Side:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void AddDocumentControl(IDockedItem dockedItem, DockConfig config) {
        var documentDockVM = LayoutGrid.Children
            .Select(x => x.DataContext)
            .OfType<DocumentDockVM>()
            .FirstOrDefault();
        
        //Try to add to existing document dock, otherwise add new document dock
        if (documentDockVM != null) {
            documentDockVM.AddDocumentControl(dockedItem);
        } else {
            AddDockedControl(DocumentDockVM.CreateControl(dockedItem), config);
        }
    }
    
    public void AddDockedControl(Control control, DockConfig? config = null) {
        config ??= DefaultDockConfig;
        
        if (LayoutGrid.Children.Count == 0) {
            LayoutGrid.ColumnDefinitions[0].Width = config.Size;
            LayoutGrid.RowDefinitions[0].Height = config.Size;
            LayoutGrid.Children.Add(control);
            return;
        }

        var rowSpan = LayoutGrid.RowDefinitions.Count;
        var columnSpan = LayoutGrid.ColumnDefinitions.Count;
        var gridSplitter = new GridSplitter { Background = Brushes.Transparent };
        var gridLength = new GridLength(5);
        
        switch (config.Dock) {
            case Dock.Top:
                // Increment row index
                foreach (var child in LayoutGrid.Children.Cast<Control>()) {
                    Grid.SetRow(child, Grid.GetRow(child) + 2);
                }

                // Add grid splitter
                LayoutGrid.RowDefinitions.Insert(0, new RowDefinition(gridLength));
                LayoutGrid.Children.Add(gridSplitter);
                Grid.SetRow(gridSplitter, 1);
                Grid.SetColumnSpan(gridSplitter, columnSpan);

                // Add new docking manager for control
                LayoutGrid.RowDefinitions.Insert(0, new RowDefinition(GridLength.Star) { MinHeight = MinLayoutSize, Height = config.Size });
                LayoutGrid.Children.Add(control);
                Grid.SetRow(control, 0);
                Grid.SetColumnSpan(control, columnSpan);

                break;
            case Dock.Bottom:
                // Increment row index
                foreach (var child in LayoutGrid.Children.Cast<Control>()) {
                    Grid.SetRow(child, Grid.GetRow(child) + 2);
                }

                // Add grid splitter
                LayoutGrid.RowDefinitions.Add(new RowDefinition(gridLength));
                LayoutGrid.Children.Add(gridSplitter);
                Grid.SetRow(gridSplitter, 1);
                Grid.SetColumnSpan(gridSplitter, columnSpan);

                // Add new docking manager for control
                LayoutGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star) { MinHeight = MinLayoutSize, Height = config.Size });
                LayoutGrid.Children.Add(control);
                Grid.SetRow(control, 0);
                Grid.SetColumnSpan(control, columnSpan);

                break;
            case Dock.Left:
                // Increment column index
                foreach (var child in LayoutGrid.Children.Cast<Control>()) {
                    Grid.SetColumn(child, Grid.GetColumn(child) + 2);
                }

                // Add grid splitter
                LayoutGrid.ColumnDefinitions.Insert(0, new ColumnDefinition(gridLength));
                LayoutGrid.Children.Add(gridSplitter);
                Grid.SetColumn(gridSplitter, 1);
                Grid.SetRowSpan(gridSplitter, rowSpan);

                // Add new docking manager for control
                LayoutGrid.ColumnDefinitions.Insert(0, new ColumnDefinition(GridLength.Star) { MinWidth = MinLayoutSize, Width = config.Size });
                LayoutGrid.Children.Add(control);
                Grid.SetColumn(control, 0);
                Grid.SetRowSpan(control, rowSpan);

                break;
            case Dock.Right:
                // Increment column index
                foreach (var child in LayoutGrid.Children.Cast<Control>()) {
                    Grid.SetColumn(child, Grid.GetColumn(child) + 2);
                }

                // Add grid splitter
                LayoutGrid.ColumnDefinitions.Add(new ColumnDefinition(gridLength));
                LayoutGrid.Children.Add(gridSplitter);
                Grid.SetColumn(gridSplitter, 1);
                Grid.SetRowSpan(gridSplitter, rowSpan);

                // Add new docking manager for control
                LayoutGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star) { MinWidth = MinLayoutSize, Width = config.Size });
                LayoutGrid.Children.Add(control);
                Grid.SetColumn(control, 0);
                Grid.SetRowSpan(control, rowSpan);

                break;
        }
    }
    
    public bool Remove(IDockedItem dockedItem) {
        var root = GetRoot();
        
        var controlIndex = -1;
        foreach (var layoutControl in LayoutGrid.Children.OfType<Control>()) {
            controlIndex++;
            if (layoutControl is GridSplitter || layoutControl.DataContext is not IDockContainerVM dockContainer) continue;
            
            if (dockContainer.ChildrenCount > 1) {
                // Multi dock can resolve dock itself
                dockContainer.Remove(dockedItem);
            } else {
                // If single dock contains the control
                if (!dockContainer.TryGetDock(dockedItem.Control, out _)) continue;
                
                // We need to remove the dock from the layout
                // If this was the only entry, we're done
                if (LayoutGrid.Children.Count == 1) {
                    LayoutGrid.Children.RemoveAt(0);
                    root.OnRemoved(dockedItem);
                    return true;
                }
                
                // Otherwise adjust remaining controls accordingly
                var controlRow = Grid.GetRow(layoutControl);
                var controlColumn = Grid.GetColumn(layoutControl);

                // Doesn't matter where it was docked otherwise
                var originalDock = Dock.Left;
                if (controlIndex > 1) {
                    if (LayoutGrid.Children[controlIndex - 2] is Control previous) {
                        var row = Grid.GetRow(previous);
                        var column = Grid.GetColumn(previous);
                        
                        if (controlRow < row) {
                            originalDock = Dock.Top;
                        } else if (controlRow > row) {
                            originalDock = Dock.Bottom;
                        } else if (controlColumn < column) {
                            originalDock = Dock.Left;
                        } else if (controlColumn > column) {
                            originalDock = Dock.Right;
                        }
                    }
                }

                // Remove control
                LayoutGrid.Children.RemoveAt(controlIndex);
                if (controlIndex == 0) {
                    // The grid splitter will always be here for the first control
                    if (LayoutGrid.Children.Count > 0) LayoutGrid.Children.RemoveAt(0);
                } else {
                    // The grid splitter will always be here for other controls
                    LayoutGrid.Children.RemoveAt(controlIndex - 1);
                }
                root.OnRemoved(dockedItem);
                
                switch (originalDock) {
                    case Dock.Top:
                    case Dock.Bottom:
                        foreach (var c in LayoutGrid.Children) {
                            if (c is not Control child) continue;
                            
                            var row = Grid.GetRow(child);
                            var rowSpan = Grid.GetRowSpan(child);
                        
                            if (row + rowSpan - 1 >= controlRow) { 
                                // We'll be affected by the row changes
                                if (row <= controlRow) {
                                    // We're inside the row that is removed - decrease size
                                    if (rowSpan > 2) {
                                        Grid.SetRowSpan(child, rowSpan - 2);
                                    } else {
                                        Console.WriteLine($"WarningXXXXXX: To small to decrease span {child.GetType().Name}");
                                    }
                                } else {
                                    // We're outside the row, but the row removal will affect us - change row
                                    Grid.SetRow(child, row - 2);
                                }
                            }
                        }
                    
                        if (LayoutGrid.RowDefinitions.Count > 1) {
                            LayoutGrid.RowDefinitions.RemoveAt(controlRow);
                            LayoutGrid.RowDefinitions.RemoveAt(LayoutGrid.RowDefinitions.Count > controlRow ? controlRow : LayoutGrid.RowDefinitions.Count - 1);
                        }
                        break;
                    case Dock.Left:
                    case Dock.Right:
                        // Only iterate after the child to be removed as the ones ones can stay
                        foreach (var c in LayoutGrid.Children) {
                            if (c is not Control child) continue;
                            
                            var column = Grid.GetColumn(child);
                            var columnSpan = Grid.GetColumnSpan(child);
                        
                            if (column + columnSpan - 1 >= controlColumn) { 
                                // We'll be affected by the column changes
                                if (column <= controlColumn) {
                                    // We're inside the column that is removed - decrease size
                                    if (columnSpan > 2) {
                                        Grid.SetColumnSpan(child, columnSpan - 2);
                                    } else {
                                        Console.WriteLine($"WarningXXXXXX: To small to decrease span {child.GetType().Name}");
                                    }
                                } else {
                                    // We're outside the column, but the column removal will affect us - change column
                                    Grid.SetColumn(child, column - 2);
                                }
                            }
                        }
                    
                        if (LayoutGrid.ColumnDefinitions.Count > 1) {
                            LayoutGrid.ColumnDefinitions.RemoveAt(controlColumn);
                            LayoutGrid.ColumnDefinitions.RemoveAt(LayoutGrid.ColumnDefinitions.Count > controlColumn ? controlColumn : LayoutGrid.ColumnDefinitions.Count - 1);
                        }
                        break;
                }

                if (ChildrenCount <= 1) {
                    if (LayoutGrid.ColumnDefinitions.Count > 1) {
                        LayoutGrid.ColumnDefinitions.RemoveAt(controlColumn);
                        LayoutGrid.ColumnDefinitions.RemoveAt(LayoutGrid.ColumnDefinitions.Count > controlColumn ? controlColumn : LayoutGrid.ColumnDefinitions.Count - 1);
                    } else if (LayoutGrid.RowDefinitions.Count > 1) {
                        LayoutGrid.RowDefinitions.RemoveAt(controlRow);
                        LayoutGrid.RowDefinitions.RemoveAt(LayoutGrid.RowDefinitions.Count > controlRow ? controlRow : LayoutGrid.RowDefinitions.Count - 1);
                    }
                }

                return true;
            }
        }

        return false;
    }
}
