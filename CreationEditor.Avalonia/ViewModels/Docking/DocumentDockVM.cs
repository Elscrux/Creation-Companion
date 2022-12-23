using System.Collections.ObjectModel;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Views.Docking;
namespace CreationEditor.Avalonia.ViewModels.Docking;

/// <summary>
/// Documents that are organized in a tab view
/// </summary>
public class DocumentDockVM : ViewModel, IDockableVM {//todo this is more like a document dock (with tabs)
    public ObservableCollection<IDockedItem> Tabs { get; } = new();

    public IDockedItem? ActiveTab { get; set; }

    protected DocumentDockVM(IDockedItem dockedItem) {
        Tabs.Add(dockedItem);
        ActiveTab = dockedItem;
    }
    
    public static Control Create(IDockedItem dockedItem) => new DocumentDock(new DocumentDockVM(dockedItem));

    public int ChildrenCount => Tabs.Count;

    public bool TryGetDock(Control control, out IDockedItem outDock) {
        outDock = null!;
        
        foreach (var dockedItem in Tabs) {
            if (ReferenceEquals(dockedItem.Control, control)) {
                outDock = dockedItem;
                return true;
            }
        }

        return false;
    }
    
    public void AddDockedControl(IDockedItem dockedItem) {
        Tabs.Add(dockedItem);
        Focus(dockedItem.Control);
    }
    
    public bool RemoveDockedControl(Control control) {
        var oldCount = Tabs.Count;
        
        // Remove control
        for (var i = Tabs.Count - 1; i >= 0; i--) {
            var dockedItem = Tabs[i];
            if (ReferenceEquals(dockedItem.Control, control)) {
                dockedItem.Root.OnControlRemoved(control);
                Tabs.RemoveAt(i);
            }
        }
        
        // Change active tab if necessary
        if (Tabs.Count > 0 && ReferenceEquals(ActiveTab?.Control, control)) ActiveTab = Tabs[0];
        
        return oldCount != Tabs.Count;
    }

    public void Focus(Control control) {
        if (TryGetDock(control, out var dock)) {
            foreach (var tab in Tabs) {
                tab.IsSelected = false;
            }
            dock.IsSelected = true;
        }
    }
}