using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels.Record.Editor;
using CreationEditor.Environment;
using CreationEditor.Skyrim.Avalonia.Models.Records.Editor;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using FactionEditor = CreationEditor.Skyrim.Avalonia.Views.Record.Editor.FactionEditor;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor;

public sealed class FactionEditorVM : ViewModel, IRecordEditorVM<Faction, IFactionGetter> {
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
    
    public FactionEditorVM(
        IRecordEditorController recordEditorController,
        IEditorEnvironment editorEnvironment) {
        _editorEnvironment = editorEnvironment;
        
        Save = ReactiveCommand.Create(() => {
            EditableRecord.SetFaction(Record);
            
            recordEditorController.CloseEditor(Record);
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
    
    public Control CreateControl(Faction record) {
        Record = record;
        EditableRecord = new EditableFaction(record);
        
        return new FactionEditor(this);
    }
}