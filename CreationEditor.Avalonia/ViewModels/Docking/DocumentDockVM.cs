using System.Collections.ObjectModel;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Views.Docking;
namespace CreationEditor.Avalonia.ViewModels.Docking;

/// <summary>
/// Documents that are organized in a tab view
/// </summary>
public sealed class DocumentDockVM : DockContainerVM {
    public ObservableCollection<IDockedItem> Tabs { get; } = new();
    
    public override IEnumerable<IDockObject> Children => Tabs;
    public override int ChildrenCount => Tabs.Count;
    
    public IDockedItem? ActiveTab { get; set; }

    private DocumentDockVM(IDockedItem dockedItem, DockContainerVM? dockParent) {
        DockParent = dockParent;
        Add(dockedItem, DockConfig.Default);
    }
    
    public static Control CreateControl(IDockedItem dockedItem, DockContainerVM? parent) => new DocumentDock(new DocumentDockVM(dockedItem, parent));


    public override bool TryGetDock(Control control, out IDockedItem outDock) {
        outDock = null!;
        
        foreach (var dockedItem in Tabs) {
            if (ReferenceEquals(dockedItem.Control, control)) {
                outDock = dockedItem;
                return true;
            }
        }

        return false;
    }

    public override void Add(IDockedItem dockedItem, DockConfig config) {
        if (config.DockMode is not DockMode.Default or DockMode.Document) return;
        
        dockedItem.DockParent = this;
        Tabs.Add(dockedItem);
        Focus(dockedItem);
        
        (this as IDockObject).DockRoot.OnDockAdded(dockedItem);
    }
    
    public override bool Remove(IDockedItem dockedItem) {
        if (!Tabs.Contains(dockedItem)) return false;
        
        using ((this as IDockObject).DockRoot.ModificationLock.Lock()) {
            using (dockedItem.RemovalLock.Lock()) {
                var oldCount = Tabs.Count;
        
                // Change active tab if necessary
                if (dockedItem.Equals(ActiveTab)) ActiveTab = Tabs.FirstOrDefault(tab => !tab.Equals(dockedItem));

                // Remove tab
                Tabs.Remove(dockedItem);

                return oldCount != Tabs.Count;
            }
        }
    }

    public override bool CleanUp() => false;

    public override bool Focus(IDockedItem dockedItem) {
        if (!Tabs.Contains(dockedItem)) return false;
        
        foreach (var tab in Tabs) tab.IsSelected = false;

        dockedItem.IsSelected = true;
        ActiveTab = dockedItem;

        return true;
    }
}