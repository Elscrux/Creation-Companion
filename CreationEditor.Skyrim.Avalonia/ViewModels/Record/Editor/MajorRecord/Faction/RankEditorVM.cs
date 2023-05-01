using System.Collections;
using System.Linq;
using System.Reactive;
using CreationEditor.Avalonia.ViewModels;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor.MajorRecord.Faction;

public sealed class RankEditorVM : ViewModel {
    public FactionEditorVM FactionEditorVM { get; }

    public ReactiveCommand<Unit, Unit> AddRank { get; }
    public ReactiveCommand<IList, Unit> RemoveRank { get; }

    public int SelectedRankIndex { get; set; }

    public RankEditorVM(FactionEditorVM factionEditorVM) {
        FactionEditorVM = factionEditorVM;

        AddRank = ReactiveCommand.Create(() => {
            FactionEditorVM.EditableRecord.Ranks.Add(new Rank { Title = new GenderedItem<TranslatedString?>(string.Empty, string.Empty) });
        });

        RemoveRank = ReactiveCommand.Create<IList>(ranks => {
            foreach (var rank in ranks.OfType<Rank>().ToList()) {
                FactionEditorVM.EditableRecord.Ranks.Remove(rank);
            }
        });
    }

    public bool CanDrop(object? o) {
        return o is Rank;
    }
}
