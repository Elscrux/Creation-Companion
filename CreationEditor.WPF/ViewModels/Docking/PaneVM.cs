using Avalonia.Controls;
using ReactiveUI;
namespace CreationEditor.WPF.ViewModels.Docking;

public class PaneVM : ReactiveObject {
    public UserControl Control { get; set; } = null!;
    
    // public DockingStatus DockingStatus { get; init; }
    
    public string Title { get; set; } = string.Empty;
    public string ContentId { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
    public bool IsActive { get; set; }

    public Image? IconSource { get; protected set; } = null;
}