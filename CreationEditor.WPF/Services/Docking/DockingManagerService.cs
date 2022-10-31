using System.Collections.ObjectModel;
using System.Windows.Controls;
using AvalonDock;
using AvalonDock.Layout.Serialization;
using CreationEditor.WPF.ViewModels.Docking;
using Noggog;
using Noggog.WPF;
namespace CreationEditor.WPF.Services.Docking;

public interface IDockingManagerService {
    public DockingManager DockingManager { get; }
    
    public PaneVM? ActiveDocument { get; }
    public ObservableCollection<PaneVM> Anchorables { get; }
    public ObservableCollection<PaneVM> Documents { get; }

    public void AddAnchoredControl<TControl>(
        TControl control,
        string title,
        DockingStatus dockingStatus)
        where TControl : UserControl;
    
    public void AddDocumentControl<TControl>(
        TControl control,
        string title)
        where TControl : UserControl;
    
    public void RemoveControl(UserControl control);
    
    public void SetActiveControl(UserControl control);

    public void SaveLayout();
    public void LoadLayout();
}

public class DockingManagerService : ViewModel, IDockingManagerService {
    public const string LayoutPath = @".\AvalonDock.Layout.config";
    
    public DockingManager DockingManager { get; set; }
    
    public PaneVM? ActiveDocument { get; private set; }
    public ObservableCollection<PaneVM> Anchorables { get; } = new();
    public ObservableCollection<PaneVM> Documents { get; } = new();

    public DockingManagerService(DockingManager dockingManager) {
        DockingManager = dockingManager;
    }
    
    public void AddAnchoredControl<TControl>(
        TControl control,
        string title,
        DockingStatus dockingStatus)
        where TControl : UserControl {
        Anchorables.Add(new PaneVM {
            Control = control,
            DockingStatus = dockingStatus,
            Title = title,
            ContentId = title,
            IsActive = true,
            IsSelected = true,
        });
    }
    
    public void AddDocumentControl<TControl>(
        TControl control,
        string title)
        where TControl : UserControl {
        Documents.Add(new PaneVM {
            Control = control,
            Title = title,
            ContentId = title,
            IsActive = true,
            IsSelected = true,
        });
    }
    
    public void RemoveControl(UserControl control) {
        Documents.RemoveWhere(l => ReferenceEquals(l.Control, control));
        Anchorables.RemoveWhere(l => ReferenceEquals(l.Control, control));
    }
    
    public void SetActiveControl(UserControl control) {
        var paneVM = GetPane(control);
        if (paneVM == null) return;

        ActiveDocument = paneVM;
    }

    public PaneVM? GetPane(UserControl control) {
        return Anchorables.FirstOrDefault(l => ReferenceEquals(l.Control, control));
    }

    public void SaveLayout() {
        var layoutSerializer = new XmlLayoutSerializer(DockingManager);
        layoutSerializer.Serialize(LayoutPath);
    }

    public void LoadLayout() {
        var layoutSerializer = new XmlLayoutSerializer(DockingManager);
        layoutSerializer.Deserialize(LayoutPath);
    }
}
