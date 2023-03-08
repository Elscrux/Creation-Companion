using CreationEditor.Avalonia.Models.Docking;
namespace CreationEditor.Avalonia.ViewModels.Docking;

public sealed class DockEdit {
    private readonly DockingManagerVM _dockingManagerVM;

    private readonly object _editLock;
    public DockEdit(DockingManagerVM dockingManagerVM) {
        _dockingManagerVM = dockingManagerVM;

        _editLock = _dockingManagerVM.EditLock;
        Monitor.Enter(_editLock);
    }

    public DockEdit Add(DockContainerVM containerVM, IDockedItem dockedItem, DockConfig config) {
        containerVM.Add(dockedItem, config);

        return this;
    }

    public DockEdit Remove(DockContainerVM containerVM, IDockedItem dockedItem) {
        using (_dockingManagerVM.CleanUpLock.Lock()) {
            using (dockedItem.RemovalLock.Lock()) {
                containerVM.Remove(dockedItem);
            }
        }

        return this;
    }

    public DockEdit Do(Action action) {
        action();

        return this;
    }

    public DockEdit CleanUp() {
        _dockingManagerVM.CleanUp();

        return this;
    }

    public DockEdit StartReporting() {
        _dockingManagerVM.IsReporting = true;

        return this;
    }

    public DockEdit StopReporting() {
        _dockingManagerVM.IsReporting = false;

        return this;
    }

    public void FinishEdit() {
        Monitor.Exit(_editLock);
    }
}
public static class DockEditMixIn {
    public static DockEdit StartEdit(this DockingManagerVM dockingManagerVM) {
        return new DockEdit(dockingManagerVM);
    }
}
