namespace CreationEditor.Avalonia.ViewModels.Docking;

public static class DockEditMixIn {
    public static DockEdit StartEdit(this DockingManagerVM dockingManagerVM) {
        return new DockEdit(dockingManagerVM);
    }
}