using Avalonia.Controls;
using Avalonia.Markup.Xaml;
namespace CreationEditor.WPF.Views.Docking; 

public partial class DocumentDockView : UserControl {
    public DocumentDockView() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
