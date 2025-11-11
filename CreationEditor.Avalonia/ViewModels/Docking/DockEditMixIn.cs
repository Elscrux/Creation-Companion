namespace CreationEditor.Avalonia.ViewModels.Docking;

public static class DockEditMixIn {
    extension(DockingManagerVM dockingManagerVM) {
        public DockEdit StartEdit() {
            return new DockEdit(dockingManagerVM);
        }
    }
}
