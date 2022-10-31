using System.Windows.Controls;
using System.Windows.Media;
using Noggog.WPF;
namespace CreationEditor.WPF.ViewModels.Docking;

public class PaneVM : ViewModel {
    public UserControl Control { get; set; } = null!;
    
    public DockingStatus DockingStatus { get; init; }
    
    public string Title { get; set; } = string.Empty;
    public string ContentId { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
    public bool IsActive { get; set; }

    public ImageSource? IconSource { get; protected set; } = null;
}