using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor.MajorRecord.Faction;

public sealed class RelationEditorVM : ViewModel {
    public FactionEditorVM FactionEditorVM { get; }

    public ReadOnlyObservableCollection<FormKey> BlacklistedFormKeys { get; }

    public ReactiveCommand<Unit, Unit> AddRelation { get; }
    public ReactiveCommand<IList, Unit> RemoveRelation { get; }

    public int SelectedRelationIndex { get; set; }

    public RelationEditorVM(FactionEditorVM factionEditorVM) {
        FactionEditorVM = factionEditorVM;

        BlacklistedFormKeys = FactionEditorVM.Core.EditableRecord.Relations
            .SelectObservableCollection(x => x.TargetFormKey, this);

        AddRelation = ReactiveCommand.Create(() => {
            var relation = new EditableRelation { Reaction = CombatReaction.Neutral };
            FactionEditorVM.Core.EditableRecord.Relations.Add(relation);
        });

        RemoveRelation = ReactiveCommand.Create<IList>(relations => {
            foreach (var relation in relations.OfType<EditableRelation>().ToList()) {
                FactionEditorVM.Core.EditableRecord.Relations.Remove(relation);
            }
        });
    }

    public bool CanDrop(object o) {
        return o is EditableRelation relation
         && !BlacklistedFormKeys.Contains(relation.TargetFormKey);
    }
}
