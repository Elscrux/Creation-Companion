using System;
using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.Editor;
using CreationEditor.Services.Environment;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.MajorRecord;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using FactionEditor = CreationEditor.Skyrim.Avalonia.Views.Record.Editor.MajorRecord.Faction.FactionEditor;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor.MajorRecord;

public sealed class FactionEditorVM : ViewModel, IRecordEditorVM<Faction, IFactionGetter> {
    IMajorRecordGetter IRecordEditorVM.Record => Record;
    public Faction Record { get; set; } = null!;
    [Reactive] public EditableFaction EditableRecord { get; set; } = null!;

    [Reactive] public ILinkCache LinkCache { get; set; }

    public ReactiveCommand<Unit, Unit> Save { get; }
    public ReactiveCommand<Unit, Unit> AddRelation { get; }
    public ReactiveCommand<Unit, Unit> RemoveSelectedRelations { get; }
    public ReactiveCommand<Unit, Unit> AddRank { get; }
    public ReactiveCommand<Unit, Unit> RemoveSelectedRank { get; }
    
    public int SelectedRelationIndex { get; set; }
    public int SelectedRankIndex { get; set; }
    
    public FactionEditorVM(
        IRecordEditorController recordEditorController,
        IEditorEnvironment editorEnvironment) {

        LinkCache = editorEnvironment.LinkCache;

        editorEnvironment.LinkCacheChanged.Subscribe(newLinkCache => LinkCache = newLinkCache);
        
        Save = ReactiveCommand.Create(() => {
            EditableRecord.SetFaction(Record);
            
            recordEditorController.CloseEditor(Record);
        });
        
        AddRelation = ReactiveCommand.Create(() => {
            var relation = new EditableRelation { Reaction = CombatReaction.Neutral };
            EditableRecord.Relations.Add(relation);
            // todo adding like that makes is that when you fill the relations, they all get the same relation because every new selection overrides the old ones
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
    
    public Control CreateControl(Faction record) {
        Record = record;
        EditableRecord = new EditableFaction(record);
        
        return new FactionEditor(this);
    }
}