using Avalonia.Controls;
using Dock.Model.ReactiveUI.Controls;
namespace CreationEditor.WPF.ViewModels.Docking; 

public class ToolDockVM : Tool {
    public required UserControl Control { get; set; }
}
