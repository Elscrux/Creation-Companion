using Avalonia.ReactiveUI;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Editor;

public partial class FactionEditor : ReactiveUserControl<FactionEditorVM> {
    public FactionEditor() {
        InitializeComponent();
    }
    public FactionEditor(FactionEditorVM vm) {
        InitializeComponent();

        DataContext = vm;
    }
}
