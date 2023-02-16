using System.Reactive;
using CreationEditor.Avalonia.ViewModels;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor.MajorRecord.Faction; 

public class RankEditorVM : ViewModel {
    public FactionEditorVM FactionEditorVM { get; }
    
    public ReactiveCommand<Unit, Unit> AddRank { get; }
    public ReactiveCommand<Unit, Unit> RemoveSelectedRank { get; }
    
    public int SelectedRankIndex { get; set; }

    public RankEditorVM(FactionEditorVM factionEditorVM) {
        FactionEditorVM = factionEditorVM;
        
        AddRank = ReactiveCommand.Create(() => {
            FactionEditorVM.EditableRecord.Ranks.Add(new Rank { Title = new GenderedItem<TranslatedString?>(string.Empty, string.Empty)});
        });
        
        RemoveSelectedRank = ReactiveCommand.Create(() => {
            if (SelectedRankIndex < 0 || SelectedRankIndex >= FactionEditorVM.EditableRecord.Ranks.Count) return;

            FactionEditorVM.EditableRecord.Ranks.RemoveAt(SelectedRankIndex);
        });
        
        RemoveSelectedRank = ReactiveCommand.Create(() => {
            if (SelectedRankIndex < 0 || SelectedRankIndex >= FactionEditorVM.EditableRecord.Ranks.Count) return;
            
            FactionEditorVM.EditableRecord.Ranks.RemoveAt(SelectedRankIndex);
        });
    }
}
