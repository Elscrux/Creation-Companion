using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Docking;
namespace CreationEditor.Avalonia.Views.Docking;

public partial class SideDockRight : ReactiveUserControl<SideDockVM>, IDockPreview {
    public SideDockRight() {
        InitializeComponent();
    }
}