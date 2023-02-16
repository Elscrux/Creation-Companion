using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.Attached;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor.MajorRecord.Faction;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Editor.MajorRecord.Faction; 

public partial class RelationsEditor : ReactiveUserControl<RelationEditorVM> {
    public RelationsEditor() {
        InitializeComponent();

        RelationsGrid.SetValue(DragDropExtended.CanDropProperty, o => ViewModel != null && ViewModel.CanDrop(o));
        RelationsGrid.SetValue(DragDropExtended.DropSelectorProperty, EditableRelation.Factory);
    }
}

