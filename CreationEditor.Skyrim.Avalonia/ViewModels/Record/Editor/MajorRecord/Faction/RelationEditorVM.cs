using System.Collections.ObjectModel;
using System.Reactive;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Extension;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor.MajorRecord.Faction;

public class RelationEditorVM : ViewModel {
    public FactionEditorVM FactionEditorVM { get; }
    
    public ReadOnlyObservableCollection<FormKey> BlacklistedFormKeys { get; }
    
    public ReactiveCommand<Unit, Unit> AddRelation { get; }
    public ReactiveCommand<Unit, Unit> RemoveSelectedRelations { get; }

    public int SelectedRelationIndex { get; set; }

    public RelationEditorVM(FactionEditorVM factionEditorVM) {
        FactionEditorVM = factionEditorVM;
        
        BlacklistedFormKeys = FactionEditorVM.EditableRecord.Relations
            .SelectObservableCollection(x => x.TargetFormKey, this);
        
        AddRelation = ReactiveCommand.Create(() => {
            var relation = new EditableRelation { Reaction = CombatReaction.Neutral };
            FactionEditorVM.EditableRecord.Relations.Add(relation);
        });
        
        RemoveSelectedRelations = ReactiveCommand.Create(() => {
            if (SelectedRelationIndex < 0 || SelectedRelationIndex >= FactionEditorVM.EditableRecord.Relations.Count) return;

            FactionEditorVM.EditableRecord.Relations.RemoveAt(SelectedRelationIndex);
        });
    }
    
    public bool CanDrop(object o) {
        return o is EditableRelation relation
         && !BlacklistedFormKeys.Contains(relation.TargetFormKey);
    }
}
