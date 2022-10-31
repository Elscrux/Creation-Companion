using CreationEditor.WPF.Skyrim.ViewModels.Record;
namespace CreationEditor.WPF.Skyrim.Views.Record;

public partial class FactionEditor {
    public FactionEditor(FactionEditorVM vm) {
        InitializeComponent();

        DataContext = vm;
    }
}
