using Avalonia.Controls;
using Avalonia.Markup.Xaml;
namespace CreationEditor.WPF.Views.Mod; 

public partial class ModDetails : UserControl {
    public ModDetails() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}
