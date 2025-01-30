using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Record.Editor;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor.MajorRecord.Faction;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Editor.MajorRecord.Faction;

public partial class FactionEditor : ReactiveUserControl<FactionEditorVM> {
    public FactionEditor() {
        InitializeComponent();
    }

    public FactionEditor(IRecordEditorVM<Mutagen.Bethesda.Skyrim.Faction, IFactionGetter> vm) : this() {
        DataContext = vm;
    }
}
