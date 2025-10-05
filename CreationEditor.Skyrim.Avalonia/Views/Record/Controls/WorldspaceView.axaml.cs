using Avalonia.ReactiveUI;
using WorldspaceViewVM = CreationEditor.Skyrim.Avalonia.ViewModels.Record.Controls.WorldspaceViewVM;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Controls;

public partial class WorldspaceView : ReactiveUserControl<WorldspaceViewVM> {
    public WorldspaceView() {
        InitializeComponent();
    }

    public WorldspaceView(WorldspaceViewVM vm) : this() {
        DataContext = vm;
    }
}
