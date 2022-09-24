using System.Collections.ObjectModel;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Models.Records; 

public sealed class EditableFaction : Faction {
    public new ObservableCollection<Relation> Relations { get; set; }
    public new ObservableCollection<Rank> Ranks { get; set; }

    public bool HiddenFromPc {
        get => (Flags & FactionFlag.HiddenFromPC) != 0;
        set => Flags = Flags.SetFlag(FactionFlag.HiddenFromPC, value);
    }

    public bool SpecialCombat {
        get => (Flags & FactionFlag.SpecialCombat) != 0;
        set => Flags = Flags.SetFlag(FactionFlag.SpecialCombat, value);
    }

    public bool CanBeOwner {
        get => (Flags & FactionFlag.CanBeOwner) != 0;
        set => Flags = Flags.SetFlag(FactionFlag.CanBeOwner, value);
    }

    public string? NameStr {
        get => Name?.String;
        set => Name = value;
    }
    
    public EditableFaction(Faction parent) {
        EditorID = parent.EditorID;
        Name = parent.Name;
        FormKey = parent.FormKey;
        Flags = parent.Flags;
        Relations = new ObservableCollection<Relation>(parent.Relations);
        Ranks = new ObservableCollection<Rank>(parent.Ranks);
    }

    public void SetFaction(Faction faction) {
        faction.EditorID = EditorID;
        faction.Name = Name;
        faction.Flags = Flags;
        faction.Relations.SetTo(Relations);
        
        for (var i = 0; i < Ranks.Count; i++) Ranks[i].Number = (uint) i;
        faction.Ranks.SetTo(Ranks);
    }
}
