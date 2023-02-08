using Avalonia.ReactiveUI;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor.MajorRecord;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Editor.MajorRecord.Faction;

public partial class FactionEditor : ReactiveUserControl<FactionEditorVM> {
    public FactionEditor() {
        InitializeComponent();
    }
    
    public FactionEditor(FactionEditorVM vm) : this() {
        DataContext = vm;
    }
}
