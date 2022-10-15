using System.Collections.ObjectModel;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Models.Records; 

public sealed class EditableFaction : Faction {
    public new ObservableCollection<Relation> Relations { get; set; }
    public new ObservableCollection<Rank> Ranks { get; set; }
    public new CrimeValues CrimeValues { get; set; }
    public new VendorValues VendorValues { get; set; }

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

    public bool IgnoreAssault {
        get => (Flags & FactionFlag.IgnoreAssault) != 0;
        set => Flags = Flags.SetFlag(FactionFlag.IgnoreAssault, value);
    }

    public bool IgnoreMurder {
        get => (Flags & FactionFlag.IgnoreMurder) != 0;
        set => Flags = Flags.SetFlag(FactionFlag.IgnoreMurder, value);
    }

    public bool IgnorePickpocket {
        get => (Flags & FactionFlag.IgnorePickpocket) != 0;
        set => Flags = Flags.SetFlag(FactionFlag.IgnorePickpocket, value);
    }

    public bool IgnoreStealing {
        get => (Flags & FactionFlag.IgnoreStealing) != 0;
        set => Flags = Flags.SetFlag(FactionFlag.IgnoreStealing, value);
    }

    public bool IgnoreTrespass {
        get => (Flags & FactionFlag.IgnoreTrespass) != 0;
        set => Flags = Flags.SetFlag(FactionFlag.IgnoreTrespass, value);
    }

    public bool IgnoreWerewolf {
        get => (Flags & FactionFlag.IgnoreWerewolf) != 0;
        set => Flags = Flags.SetFlag(FactionFlag.IgnoreWerewolf, value);
    }

    public bool DoNotReportCrimesAgainstMembers {
        get => (Flags & FactionFlag.DoNotReportCrimesAgainstMembers) != 0;
        set => Flags = Flags.SetFlag(FactionFlag.DoNotReportCrimesAgainstMembers, value);
    }

    public bool TrackCrime {
        get => (Flags & FactionFlag.TrackCrime) != 0;
        set => Flags = Flags.SetFlag(FactionFlag.TrackCrime, value);
    }

    public bool CrimeGoldUseDefaults {
        get => (Flags & FactionFlag.CrimeGoldUseDefaults) != 0;
        set => Flags = Flags.SetFlag(FactionFlag.CrimeGoldUseDefaults, value);
    }

    public bool Vendor {
        get => (Flags & FactionFlag.Vendor) != 0;
        set => Flags = Flags.SetFlag(FactionFlag.Vendor, value);
    }
    
    public EditableFaction(IFaction parent) {
        EditorID = parent.EditorID;
        Name = parent.Name;
        FormKey = parent.FormKey;
        Flags = parent.Flags;
        Relations = new ObservableCollection<Relation>(parent.Relations);
        Ranks = new ObservableCollection<Rank>(parent.Ranks);
        CrimeValues = parent.CrimeValues ?? new CrimeValues {
            Arrest = true,
            AttackOnSight = true,
            Murder = 1000,
            Assault = 40,
            Pickpocket = 25,
            Trespass = 5,
            StealMult = 0.5f,
            Escape = 100,
            Werewolf = 1000
        };
        VendorValues = parent.VendorValues ?? new VendorValues();
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
