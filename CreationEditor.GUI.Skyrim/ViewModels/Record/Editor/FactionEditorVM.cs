using System.Reactive;
using System.Windows.Controls;
using CreationEditor.Environment;
using CreationEditor.GUI.Services.Record;
using CreationEditor.GUI.Skyrim.Views.Record;
using CreationEditor.GUI.ViewModels.Record;
using CreationEditor.Skyrim.Models.Records;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.WPF;
using ReactiveUI;
namespace CreationEditor.GUI.Skyrim.ViewModels.Record;

public class FactionEditorVM : ViewModel, IRecordEditorVM<Faction, IFactionGetter> {
    private readonly IRecordEditorController _recordEditorController;
    private readonly IEditorEnvironment _editorEnvironment;

    public Faction Record { get; set; } = null!;
    public EditableFaction EditableRecord { get; set; } = null!;

    public ILinkCache LinkCache => _editorEnvironment.LinkCache;

    public ReactiveCommand<Unit, Unit> Save { get; }
    public ReactiveCommand<Unit, Unit> AddRelation { get; }
    public ReactiveCommand<Unit, Unit> RemoveSelectedRelations { get; }
    
    public Relation? SelectedRelation { get; set; }

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
            EditableRecord.Relations.RemoveWhere(relation => SelectedRelation.Equals(relation));
        });
    }
    
    public UserControl CreateUserControl(Faction record) {
        Record = record;
        EditableRecord = new EditableFaction(record);
        
        return new FactionEditor(this);
    }
}