using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Docking;

public sealed class SideDockVM : DockContainerVM {
    public ObservableCollection<IDockedItem> Tabs { get; set; } = new();

    public override IEnumerable<IDockObject> Children => Tabs;
    public override int ChildrenCount => Tabs.Count;

    [Reactive] public IDockedItem? ActiveTab { get; set; }
    public ReactiveCommand<IDockedItem, Unit> Activate { get; }

    public SideDockVM(DockContainerVM dockParent) {
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

    private void Unfocus() {
        if (ActiveTab != null) ActiveTab.IsSelected = false;

        ActiveTab = null;
    }

    public override void Add(IDockedItem dockedItem, DockConfig config) {
        if (config.DockMode is not null and not DockMode.Side) return;

        dockedItem.DockParent = this;
        Tabs.Add(dockedItem);
        Focus(dockedItem);

        (this as IDockObject).DockRoot.OnDockAdded(dockedItem);
    }

    public override bool Remove(IDockedItem dockedItem) {
        if (!Tabs.Contains(dockedItem)) return false;

        using ((this as IDockObject).DockRoot.CleanUpLock.Lock()) {
            using (dockedItem.RemovalLock.Lock()) {
                // Hide if active
                if (dockedItem.Equals(ActiveTab)) Unfocus();

                // Remove dock item
                Tabs.Remove(dockedItem);

                return false;
            }
        }
    }

    public override bool CleanUp() => false;
}
