using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Docking; 

public class SidePanelVM : ViewModel, IDockContainerVM {
    private const double InitialTabSize = 20;
    private const double InitialDefaultTabMinSize = 50;
    private const double TabSizeExpanded = 150;
    
    public ObservableCollection<IDockedItem> Tabs { get; set; } = new();
    
    [Reactive] public IDockedItem? ActiveTab { get; set; }
    public ReactiveCommand<IDockedItem, Unit> Activate { get; }
    
    public double DefaultTabSize { get; set; } = InitialTabSize;
    [Reactive] public double TabSize { get; set; } = InitialTabSize;

    public double DefaultTabMinSize { get; set; } = InitialDefaultTabMinSize;
    [Reactive] public double TabMinSize { get; set; } = InitialTabSize;
    public ReactiveCommand<IDockedItem, Unit> Close { get; }

    public SidePanelVM() {
        Activate = ReactiveCommand.Create<IDockedItem>(tab => {
            if (ReferenceEquals(tab, ActiveTab)) {
                Unfocus();
            } else {
                Focus(tab);
            }
        });
        
        Close = ReactiveCommand.Create<IDockedItem>(tab => Remove(tab));
    }

    public int ChildrenCount => Tabs.Count;
    
    public bool TryGetDock(Control control, [MaybeNullWhen(false)] out IDockedItem outDock) {
        outDock = Tabs.FirstOrDefault(tab => ReferenceEquals(tab.Control, control));
        return outDock != null;
    }

    public void Focus(IDockedItem dockedItem) {
        if (ActiveTab != null) ActiveTab.IsSelected = false;
        
        ActiveTab = dockedItem;
        dockedItem.IsSelected = true;
        TabMinSize = DefaultTabMinSize;
        TabSize = TabSizeExpanded;
    }

    private void Unfocus() {
        if (ActiveTab != null) ActiveTab.IsSelected = false;
        
        ActiveTab = null;
        TabMinSize = TabSize = DefaultTabSize;
    }
    
    public void Add(IDockedItem dockedItem, DockConfig config) {
        Tabs.Add(dockedItem);
        Focus(dockedItem);
    }
    
    public bool Remove(IDockedItem dockedItem) {
        if (!Tabs.Contains(dockedItem)) return false;

        if (ReferenceEquals(dockedItem, ActiveTab)) Unfocus();
        
        Tabs.Remove(dockedItem);
        dockedItem.Root.OnRemoved(dockedItem);

        if (Tabs.Count == 0) TabMinSize = TabSize = 0;
        return true;
    }
}
