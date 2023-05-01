using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Docking;

public abstract class TabbedDockVM : DockContainerVM {
    public ObservableCollection<IDockedItem> Tabs { get; set; } = new();

    public override IEnumerable<IDockObject> Children => Tabs;
    public override int ChildrenCount => Tabs.Count;

    [Reactive] public IDockedItem? ActiveTab { get; set; }
    public ReactiveCommand<IDockedItem, Unit> Activate { get; }

    public abstract DockMode DockMode { get; }

    protected TabbedDockVM(DockContainerVM dockParent) {
        DockParent = dockParent;

        Activate = ReactiveCommand.Create<IDockedItem>(tab => {
            if (tab.Equals(ActiveTab)) {
                Unfocus();
            } else {
                Focus(tab);
            }
        });
    }

    public override bool TryGetDock(Control control, [MaybeNullWhen(false)] out IDockedItem outDock) {
        outDock = Tabs.FirstOrDefault(tab => ReferenceEquals(tab.Control, control));
        return outDock != null;
    }

    public override bool Focus(IDockedItem dockedItem) {
        if (!Tabs.Contains(dockedItem)) return false;

        if (ActiveTab != null) ActiveTab.IsSelected = false;

        ActiveTab = dockedItem;
        dockedItem.IsSelected = true;

        return true;
    }

    protected virtual void Unfocus() {}

    public override void Add(IDockedItem dockedItem, DockConfig config) {
        if (config.DockMode != null && config.DockMode != DockMode) return;

        using ((this as IDockObject).DockRoot.CleanUpLock.Lock()) {
            dockedItem.DockParent = this;
            Tabs.Add(dockedItem);
            Focus(dockedItem);

            (this as IDockObject).DockRoot.OnDockAdded(dockedItem);
        }
    }

    public override bool Remove(IDockedItem dockedItem) {
        if (!Tabs.Contains(dockedItem)) return false;

        using ((this as IDockObject).DockRoot.CleanUpLock.Lock()) {
            using (dockedItem.RemovalLock.Lock()) {
                var oldCount = Tabs.Count;

                // Hide if active
                if (dockedItem.Equals(ActiveTab)) {
                    Unfocus();
                    var newActiveTab = Tabs.FirstOrDefault(tab => !tab.Equals(dockedItem));
                    if (newActiveTab != null) newActiveTab.IsSelected = true;
                    ActiveTab = newActiveTab;
                }

                // Remove dock item
                Tabs.Remove(dockedItem);

                return oldCount != Tabs.Count;
            }
        }
    }

    public void Move(int oldIndex, int newIndex) {
        if (oldIndex == newIndex) return;

        var item = Tabs[oldIndex];
        Tabs.RemoveAt(oldIndex);
        Tabs.Insert(newIndex, item);
    }

    public override bool CleanUp() => false;
}
