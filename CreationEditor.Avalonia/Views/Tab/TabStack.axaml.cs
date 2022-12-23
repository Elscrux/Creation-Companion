using Avalonia.Controls;
using Avalonia.Layout;

namespace CreationEditor.Avalonia.Views.Tab; 

public partial class TabStack : UserControl {
    public Orientation Orientation { get; set; } = Orientation.Horizontal;
    
    public TabStack() {
        InitializeComponent();
    }
}