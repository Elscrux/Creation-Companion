using System.Collections.ObjectModel;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Views.Docking;
namespace CreationEditor.Avalonia.ViewModels.Docking;

/// <summary>
/// Documents that are organized in a tab view
/// </summary>
public class DocumentDockVM : ViewModel, IDockContainerVM {
    public ObservableCollection<IDockedItem> Tabs { get; } = new();

    public IDockedItem? ActiveTab { get; set; }

    protected DocumentDockVM(IDockedItem dockedItem) {
        Tabs.Add(dockedItem);
        ActiveTab = dockedItem;
    }
    
    public static Control CreateControl(IDockedItem dockedItem) => new DocumentDock(new DocumentDockVM(dockedItem));

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

    public void Add(IDockedItem dockedItem, DockConfig config) {
        switch (config.DockType) {
            case DockType.Layout:
                break;
            case DockType.Document:
                AddDocumentControl(dockedItem);
                break;
            case DockType.Side:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public void AddDocumentControl(IDockedItem dockedItem) {
        Tabs.Add(dockedItem);
        Focus(dockedItem);
    }
    
    public bool Remove(IDockedItem dockedItem) {
        var oldCount = Tabs.Count;

        // Remove tab
        Tabs.Remove(dockedItem);
        
        // Change active tab if necessary
        if (Tabs.Count > 0 && ReferenceEquals(ActiveTab?.Control, dockedItem)) ActiveTab = Tabs[0];

        if (oldCount == Tabs.Count) return false;

        dockedItem.Root.OnRemoved(dockedItem);
        return true;
    }

    public void Focus(IDockedItem dockedItem) {
        foreach (var tab in Tabs) tab.IsSelected = false;
        
        dockedItem.IsSelected = true;
    }
}