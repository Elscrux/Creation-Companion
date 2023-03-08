using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Docking;
namespace CreationEditor.Avalonia.Views.Docking;

public partial class SideDock : ReactiveUserControl<SideDockVM>, IDockPreview {
    public SideDock() {
        InitializeComponent();
    }
}
