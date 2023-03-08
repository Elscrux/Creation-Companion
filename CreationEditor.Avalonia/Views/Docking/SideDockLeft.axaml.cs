using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Docking;
namespace CreationEditor.Avalonia.Views.Docking;

public partial class SideDockLeft : ReactiveUserControl<SideDockVM>, IDockPreview {
    public SideDockLeft() {
        InitializeComponent();
    }
}
