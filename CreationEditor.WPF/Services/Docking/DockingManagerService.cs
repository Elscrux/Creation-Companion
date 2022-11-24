using System.Reactive.Subjects;
using Avalonia.Controls;
using CreationEditor.WPF.ViewModels;
using CreationEditor.WPF.ViewModels.Docking;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.ReactiveUI.Controls;
using Noggog;
using ReactiveUI;
namespace CreationEditor.WPF.Services.Docking;

public interface IDockingManagerService {
    public IRootDock DockingManager { get; }

    public IDock? ActiveDocument { get; }

    public void AddControl<TControl>(TControl control,
        string title,
        Avalonia.Controls.Dock? dock = null)
        where TControl : UserControl;

    public void RemoveControl(UserControl control);

    public void SetActiveControl(UserControl control);

    public IObservable<UserControl> Closed { get; }

    public void SaveLayout();
    public void LoadLayout();
}
public class DockingManagerService : ReactiveObject, IDockingManagerService {
    public IRootDock DockingManager { get; }
    private DefaultDockFactory Factory { get; }

    private readonly Subject<UserControl> _closed = new();
    public IObservable<UserControl> Closed => _closed;

    public IDock? ActiveDocument { get; private set; }

    public DockingManagerService() {
        Factory = new DefaultDockFactory(this);
        DockingManager = Factory.CreateLayout();
        Factory.InitLayout(DockingManager);
    }

    public void AddControl<TControl>(TControl control,
        string title,
        Avalonia.Controls.Dock? dock = null)
        where TControl : UserControl {
        // Factory.Main.VisibleDockables?.Add(new ProportionalDockSplitter());
        Factory.Main.VisibleDockables?.Add(new ToolDock {
            Proportion = 1,
            VisibleDockables = Factory.CreateList<IDockable>(
                new ToolDockVM {
                    Title = title,
                    Id = Guid.NewGuid().ToString(),
                    Control = control
                }
            )
        });

        Factory.InitLayout(DockingManager);
    }

    public void RemoveControl(UserControl control) {
        DockingManager.VisibleDockables.RemoveWhere(l => ReferenceEquals(l.Context, control));
    }

    public void SetActiveControl(UserControl control) {
        var dock = GetDock(control);
        if (dock == null) return;

        if (ActiveDocument != null) ActiveDocument.IsActive = false;

        dock.IsActive = true;
        ActiveDocument = dock;
    }

    private IDock? GetDock(UserControl control) {
        return null;
        // return DockingManager.VisibleDockables?.FirstOrDefault(l => ReferenceEquals(l.Context, control));
    }

    public void SaveLayout() {}

    public void LoadLayout() {}
}
public static class ConvertDock {
    public static DockMode ToDockMode(this Avalonia.Controls.Dock? dock) {
        return dock switch {
            Avalonia.Controls.Dock.Left => DockMode.Left,
            Avalonia.Controls.Dock.Bottom => DockMode.Bottom,
            Avalonia.Controls.Dock.Right => DockMode.Right,
            Avalonia.Controls.Dock.Top => DockMode.Top,
            _ => DockMode.Center
        };
    }
}

//
// public interface IDockingManagerService {
//     public IUniDockService DockingManager { get; }
//     
//     public DockItemVM? ActiveDocument { get; }
//
//     public void AddControl<TControl>(
//         TControl control,
//         string title,
//         DockKind dockKind)
//         where TControl : UserControl;
//     
//     public void RemoveControl(UserControl control);
//     
//     public void SetActiveControl(UserControl control);
//
//     public void SaveLayout();
//     public void LoadLayout();
// }
//
// public class DockingManagerService : ReactiveObject, IDockingManagerService {
//     private const string DockSerializationFileName = @".\DockSerialization.xml";
//     private const string VMSerializationFileName = @".\DockVMSerialization.xml";
//     
//     public IUniDockService DockingManager { get; set; }
//     
//     public DockItemVM? ActiveDocument { get; private set; }
//
//     public DockingManagerService(IUniDockService dockingManager) {
//         DockingManager = dockingManager;
//         DockingManager.DockItemsViewModels ??= new ObservableCollection<DockItemViewModelBase>();
//     }
//     
//     public void AddControl<TControl>(
//         TControl control,
//         string title,
//         DockKind dockKind)
//         where TControl : UserControl {
//         DockingManager.DockItemsViewModels.Add(new DockItemVM {
//             DockId = Guid.NewGuid().ToString(),
//             Header = title,
//             Content = control,
//             DefaultDockGroupId = dockKind.ToString(),
//         });
//     }
//     
//     public void RemoveControl(UserControl control) {
//         DockingManager.DockItemsViewModels.RemoveWhere(l => ReferenceEquals(l.Content, control));
//     }
//     
//     public void SetActiveControl(UserControl control) {
//         var paneVM = GetDockItemVM(control);
//         if (paneVM == null) return;
//
//         if (ActiveDocument != null) ActiveDocument.IsActive = false;
//         
//         paneVM.IsActive = true;
//         ActiveDocument = paneVM;
//     }
//
//     private DockItemVM? GetDockItemVM(UserControl control) {
//         return DockingManager.DockItemsViewModels.FirstOrDefault(l => ReferenceEquals(l.Content, control)) as DockItemVM;
//     }
//
//     public void SaveLayout() {
//         DockingManager.SaveToFile(DockSerializationFileName);
//         DockingManager.SaveViewModelsToFile(VMSerializationFileName);
//     }
//
//     public void LoadLayout() {
//         DockingManager.DockItemsViewModels.Clear();
//         
//         DockingManager.RestoreFromFile(DockSerializationFileName);
//         DockingManager.RestoreViewModelsFromFile(VMSerializationFileName);
//     }
// }
