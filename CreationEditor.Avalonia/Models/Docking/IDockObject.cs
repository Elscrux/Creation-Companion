using CreationEditor.Avalonia.ViewModels.Docking;
namespace CreationEditor.Avalonia.Models.Docking;

public interface IDockObject {
    public DockingManagerVM DockRoot {
        get {
            var root = this;
            while (root.DockParent != null) {
                root = root.DockParent;
            }

            if (root is not DockingManagerVM dockingManagerVM) {
                throw new Exception("No docking manager found as root");
            }

            return dockingManagerVM;
        }
    }

    public DockContainerVM? DockParent { get; }
}
