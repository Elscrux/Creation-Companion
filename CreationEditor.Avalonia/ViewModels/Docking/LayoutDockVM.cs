using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Views.Docking;
namespace CreationEditor.Avalonia.ViewModels.Docking;

public sealed class LayoutDockVM : DockContainerVM {
    public DockGrid LayoutGrid { get; }

    public LayoutDockVM(DockContainerVM? dockParent) {
        DockParent = dockParent;
        LayoutGrid = new DockGrid(this);
    }

    public LayoutDockVM(IDockedItem dockedItem, DockContainerVM? dockParent) : this(dockParent) {
        AddDockedControl(dockedItem, DockConfig.Default);
    }
    
    public override IEnumerable<IDockObject> Children {
        get {
            foreach (var layoutChild in LayoutGrid.Children) {
                switch (layoutChild) {
                    // Found dock container
                    case { DataContext: DockContainerVM dockableVM } when dockableVM != this:
                        yield return dockableVM;
                        break;
                    // Found docked item
                    case DockedControl dockedControl:
                        yield return dockedControl;
                        break;
                }
            }
        }
    }

    public override bool TryGetDock(Control control, [MaybeNullWhen(false)] out IDockedItem outDock) {
        outDock = null;
        switch (ChildrenCount) {
            case 0:
                return false;
            case 1:
                switch (LayoutGrid.Children[0]) {
                    // Found matching docked item
                    case DockedControl dockedControl when ReferenceEquals(dockedControl.Control, control):
                        dockedControl.Focus();
                        outDock = dockedControl;
                        return true;
                    // Found dock container
                    case { DataContext: DockContainerVM dockableVM }:
                        if (dockableVM.TryGetDock(control, out var output)) {
                            outDock = output;
                            return true;
                        }
                        return false;
                }
                break;
            case > 1:
                foreach (var layoutChild in LayoutGrid.Children) {
                    if (layoutChild.DataContext is not DockContainerVM dockContainerVM
                     || layoutChild.DataContext == this
                     || !dockContainerVM.TryGetDock(control, out var dock)) continue;

                    outDock = dock;
                    return true;
                }
                break;
        }

        return false;
    }

    public override bool Focus(IDockedItem dockedItem) {
        foreach (var layoutChild in LayoutGrid.Children.OfType<Control>()) {
            switch (layoutChild) {
                // Found matching docked item
                case DockedControl dockedControl when dockedItem.Equals(dockedControl):
                    dockedControl.Focus();
                    return true;
                // Found dock container
                case { DataContext: DockContainerVM dockableVM }:
                    return dockableVM.Focus(dockedItem);
            }
        }

        return false;
    }

    public override void Add(IDockedItem dockedItem, DockConfig config) {
        switch (config.DockMode) {
            case DockMode.Document:
                AddDocumentControl(dockedItem, config);
                break;
            case DockMode.Side:
                break;
            case null:
            case DockMode.Layout:
                AddDockedControl(dockedItem, config);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(config));
        }
    }

    private void AddDocumentControl(IDockedItem dockedItem, DockConfig config) {
        var documentDockVM = LayoutGrid.Children
            .Select(x => x.DataContext)
            .OfType<DocumentDockVM>()
            .FirstOrDefault();

        //Try to add to existing document dock, otherwise add new document dock
        using ((this as IDockObject).DockRoot.CleanUpLock.Lock()) {
            if (documentDockVM != null) {
                documentDockVM.Add(dockedItem, DockConfig.Default);
            } else {
                var documentControl = DocumentDockVM.CreateControl(dockedItem, this);
                LayoutGrid.Add(documentControl, config);
            }
        }
    }
    
    private void AddDockedControl(IDockedItem dockedItem, DockConfig config) {
        using ((this as IDockObject).DockRoot.CleanUpLock.Lock()) {
            switch (LayoutGrid.Children) {
                // If the layout grid is empty, add the element directly and return early
                case []: {
                    dockedItem.DockParent = this;
                    var dockedControl = new DockedControl(dockedItem);
                    LayoutGrid.Add(dockedControl, config);
                    return;
                }
                // If the layout grid has just one element, wrap it in a layout itself and continue
                case [DockedControl dockedControl]: {
                    // Remove docked control
                    if (LayoutGrid.Remove(dockedControl)) {
                        // Add layout grid instead
                        var convertedLayoutDock = new LayoutDockVM(dockedControl, this);
                        LayoutGrid.Add(convertedLayoutDock.LayoutGrid, DockConfig.Default);
                    }
                    break;
                }
            }

            var layoutDockVM = new LayoutDockVM(dockedItem, this);
            LayoutGrid.Add(layoutDockVM.LayoutGrid, config);
        }
    }

    public override bool Remove(IDockedItem dockedItem) {
        using ((this as IDockObject).DockRoot.CleanUpLock.Lock()) {
            using (dockedItem.RemovalLock.Lock()) {
                foreach (var control in LayoutGrid.Children.OfType<Control>()) {
                    switch (control) {
                        // Found dock grid
                        case DockGrid { DataContext: DockContainerVM childDockVM } when childDockVM != this:
                            if (childDockVM.Remove(dockedItem)) return true;

                            break;
                        // Found docked item
                        case DockedControl dockedControl when dockedControl.Equals(dockedItem):
                            if (LayoutGrid.Remove(dockedControl)) return true;

                            break;
                    }
                }
            }

            return false;
        }
    }

    public override bool CleanUp() {
        var anyChanges = false;
        foreach (var child in LayoutGrid.Children.OfType<Control>()) {
            if (child.DataContext is not DockContainerVM childVM || childVM == this) continue;

            // Recursively clean all children
            while (childVM.CleanUp()) anyChanges = true;

            // Flatten dock grids with only one child
            if (child is DockGrid { Children: [DockGrid { DataContext: DockContainerVM grandchildVM } grandchildGrid] } childGrid) {
                var index = LayoutGrid.Children.IndexOf(childGrid);
                childGrid.Children.Clear();

                LayoutGrid.Children.RemoveAt(index);
                LayoutGrid.Children.Insert(index, grandchildGrid);
                
                Grid.SetRow(grandchildGrid, Grid.GetRow(childGrid));
                Grid.SetRowSpan(grandchildGrid, Grid.GetRowSpan(childGrid));
                Grid.SetColumn(grandchildGrid, Grid.GetColumn(childGrid));
                Grid.SetColumnSpan(grandchildGrid, Grid.GetColumnSpan(childGrid));
            
                grandchildVM.DockParent = this;
                return true;
            }

            // Remove empty containers
            if (childVM.ChildrenCount == 0 && LayoutGrid.Remove(child)) {
                childVM.DockParent = null;
                return true;
            }
        }

        return anyChanges;
    }
}
