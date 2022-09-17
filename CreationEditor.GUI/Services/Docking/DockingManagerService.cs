using Syncfusion.Windows.Tools.Controls;
namespace CreationEditor.GUI.Services.Docking;

public interface IDockingManagerService {
    public DockingManager GetDockingManager();
}

public class DockingManagerService : IDockingManagerService {
    private readonly DockingManager _dockingManager;
    
    public DockingManagerService(DockingManager dockingManager) {
        _dockingManager = dockingManager;

    }
    
    public DockingManager GetDockingManager() {
        return _dockingManager;
    }
}
