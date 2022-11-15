using System;
using System.Collections.Generic;
using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Environment;
using CreationEditor.WPF.Services.Record;
using CreationEditor.WPF.Skyrim.Models.Records;
using CreationEditor.WPF.Skyrim.Views.Record;
using CreationEditor.WPF.ViewModels.Record;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.WPF.Skyrim.ViewModels.Record;

public class FactionEditorVM : ReactiveObject, IRecordEditorVM<Faction, IFactionGetter> {
    private readonly IRecordEditorController _recordEditorController;
    private readonly IEditorEnvironment _editorEnvironment;
    
    IMajorRecordGetter IRecordEditorVM.Record => Record;
    public Faction Record { get; set; } = null!;
    [Reactive] public EditableFaction EditableRecord { get; set; } = null!;

    public ILinkCache LinkCache => _editorEnvironment.LinkCache;

    public ReactiveCommand<Unit, Unit> Save { get; }
    public ReactiveCommand<Unit, Unit> AddRelation { get; }
    public ReactiveCommand<Unit, Unit> RemoveSelectedRelations { get; }
    public ReactiveCommand<Unit, Unit> AddRank { get; }
    public ReactiveCommand<Unit, Unit> RemoveSelectedRank { get; }
    
    public int SelectedRelationIndex { get; set; }
    public int SelectedRankIndex { get; set; }
    
    public IEnumerable<Type> OutfitTypes { get; } = typeof(IOutfitGetter).AsEnumerable();
    public IEnumerable<Type> FormListTypes { get; } = typeof(IFormListGetter).AsEnumerable();
    
    public static IEnumerable<Type> RelationTypes { get; } = new[] { typeof(IFactionGetter), typeof(IRaceGetter) };
    public static IEnumerable<CombatReaction> CombatReactions { get; } = Enum.GetValues<CombatReaction>();
    public FactionEditorVM(
        IRecordEditorController recordEditorController,
        IEditorEnvironment editorEnvironment) {
        _recordEditorController = recordEditorController;
        _editorEnvironment = editorEnvironment;
        
        Save = ReactiveCommand.Create(() => {
            EditableRecord.SetFaction(Record);
            
            _recordEditorController.CloseEditor(Record);
        });
        
        AddRelation = ReactiveCommand.Create(() => {
            EditableRecord.Relations.Add(new Relation());
        });
        
        RemoveSelectedRelations = ReactiveCommand.Create(() => {
            if (SelectedRelationIndex < 0 || SelectedRelationIndex >= EditableRecord.Relations.Count) return;

            EditableRecord.Relations.RemoveAt(SelectedRelationIndex);
        });
        
        AddRank = ReactiveCommand.Create(() => {
            EditableRecord.Ranks.Add(new Rank { Title = new GenderedItem<TranslatedString?>(string.Empty, string.Empty)});
        });
        
        RemoveSelectedRank = ReactiveCommand.Create(() => {
            if (SelectedRankIndex < 0 || SelectedRankIndex >= EditableRecord.Ranks.Count) return;

            EditableRecord.Ranks.RemoveAt(SelectedRankIndex);
        });
    }
    
    public UserControl CreateUserControl(Faction record) {
        Record = record;
        EditableRecord = new EditableFaction(record);
        
        return new FactionEditor(this);
    }
    public void Dispose() {
        throw new NotImplementedException();
    }
}