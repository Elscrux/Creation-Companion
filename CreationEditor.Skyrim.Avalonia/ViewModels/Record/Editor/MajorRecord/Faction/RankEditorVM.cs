using System.Collections;
using CreationEditor.Avalonia.ViewModels;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor.MajorRecord.Faction;

public sealed partial class RankEditorVM(FactionEditorVM factionEditorVM) : ViewModel {
    public FactionEditorVM FactionEditorVM { get; } = factionEditorVM;

    public int SelectedRankIndex { get; set; }

    [ReactiveCommand]
    private void AddRank() {
        FactionEditorVM.Core.EditableRecord.Ranks.Add(new Rank {
            Title = new GenderedItem<TranslatedString?>(string.Empty, string.Empty)
        });
    }

    [ReactiveCommand]
    private void RemoveRank(IList ranks) {
        foreach (var rank in ranks.OfType<Rank>().ToList()) {
            FactionEditorVM.Core.EditableRecord.Ranks.Remove(rank);
        }
    }

    public static bool CanDrop(object? o) => o is Rank;
}
