using Avalonia.Controls;
using Avalonia.Markup.Xaml;
namespace CreationEditor.WPF.Views.Docking; 

public partial class ToolDockView : UserControl {
    public ToolDockView() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
