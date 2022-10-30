using CreationEditor.GUI.Skyrim.ViewModels.Record;
namespace CreationEditor.GUI.Skyrim.Views.Record;

public partial class FactionEditor {
    public FactionEditor(FactionEditorVM vm) {
        InitializeComponent();

        DataContext = vm;
    }
}
