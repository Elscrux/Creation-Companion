using System.ComponentModel;
using System.Runtime.CompilerServices;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;
using DynamicData.Binding;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.Models.Record.Editor.MajorRecord;

public sealed class EditableFaction : Faction, INotifyPropertyChanged {
    public new ObservableCollectionExtended<EditableRelation> Relations { get; set; }
    public new ObservableCollectionExtended<Rank> Ranks { get; set; }
    public new ObservableCollectionExtended<EditableCondition> Conditions { get; set; }
    public new CrimeValues CrimeValues { get; set; }
    public new VendorValues VendorValues { get; set; }

    public override string? EditorID { get; set; }

    public bool HiddenFromPc {
        get => (Flags & FactionFlag.HiddenFromPC) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.HiddenFromPC, value);
            OnPropertyChanged();
        }
    }

    public bool SpecialCombat {
        get => (Flags & FactionFlag.SpecialCombat) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.SpecialCombat, value);
            OnPropertyChanged();
        }
    }

    public bool CanBeOwner {
        get => (Flags & FactionFlag.CanBeOwner) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.CanBeOwner, value);
            OnPropertyChanged();
        }
    }

    public string? NameStr {
        get => Name?.String;
        set {
            Name = value;
            OnPropertyChanged();
        }
    }

    public bool IgnoreAssault {
        get => (Flags & FactionFlag.IgnoreAssault) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.IgnoreAssault, value);
            OnPropertyChanged();
        }
    }

    public bool IgnoreMurder {
        get => (Flags & FactionFlag.IgnoreMurder) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.IgnoreMurder, value);
            OnPropertyChanged();
        }
    }

    public bool IgnorePickpocket {
        get => (Flags & FactionFlag.IgnorePickpocket) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.IgnorePickpocket, value);
            OnPropertyChanged();
        }
    }

    public bool IgnoreStealing {
        get => (Flags & FactionFlag.IgnoreStealing) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.IgnoreStealing, value);
            OnPropertyChanged();
        }
    }

    public bool IgnoreTrespass {
        get => (Flags & FactionFlag.IgnoreTrespass) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.IgnoreTrespass, value);
            OnPropertyChanged();
        }
    }

    public bool IgnoreWerewolf {
        get => (Flags & FactionFlag.IgnoreWerewolf) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.IgnoreWerewolf, value);
            OnPropertyChanged();
        }
    }

    public bool DoNotReportCrimesAgainstMembers {
        get => (Flags & FactionFlag.DoNotReportCrimesAgainstMembers) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.DoNotReportCrimesAgainstMembers, value);
            OnPropertyChanged();
        }
    }

    public bool TrackCrime {
        get => (Flags & FactionFlag.TrackCrime) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.TrackCrime, value);
            OnPropertyChanged();
        }
    }

    public bool CrimeGoldUseDefaults {
        get => (Flags & FactionFlag.CrimeGoldUseDefaults) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.CrimeGoldUseDefaults, value);
            OnPropertyChanged();
        }
    }

    public bool Vendor {
        get => (Flags & FactionFlag.Vendor) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.Vendor, value);
            OnPropertyChanged();
        }
    }

    public EditableFaction(IFaction parent) {
        EditorID = parent.EditorID;
        Name = parent.Name;
        FormKey = parent.FormKey;
        Flags = parent.Flags;
        Relations = new ObservableCollectionExtended<EditableRelation>(parent.Relations.Select(r => new EditableRelation {
            Reaction = r.Reaction, TargetFormKey = r.Target.FormKey,
        }));
        Ranks = new ObservableCollectionExtended<Rank>(parent.Ranks);

        CrimeValues = parent.CrimeValues ?? GetDefaultCrimeValues();
        JailOutfit = parent.JailOutfit;
        SharedCrimeFactionList = parent.SharedCrimeFactionList;
        ExteriorJailMarker = parent.ExteriorJailMarker;
        FollowerWaitMarker = parent.FollowerWaitMarker;
        PlayerInventoryContainer = parent.PlayerInventoryContainer;
        StolenGoodsContainer = parent.StolenGoodsContainer;

        VendorValues = parent.VendorValues ?? new VendorValues { StartHour = 0, EndHour = 24 };
        VendorLocation = parent.VendorLocation ?? new LocationTargetRadius();
        VendorBuySellList = parent.VendorBuySellList;
        MerchantContainer = parent.MerchantContainer;
        Conditions = parent.Conditions is null
            ? []
            : new ObservableCollectionExtended<EditableCondition>(parent.Conditions.Select(c => new EditableCondition(c)));
    }

    public void CopyTo(Faction faction) {
        faction.EditorID = EditorID;
        faction.Name = Name;
        faction.Flags = Flags;
        faction.Relations.ReplaceWith(Relations
            .Where(r => !r.TargetFormKey.IsNull)
            .Select(r => r.ToRelation()));

        for (var i = 0; i < Ranks.Count; i++) Ranks[i].Number = (uint) i;
        faction.Ranks.ReplaceWith(Ranks);

        faction.CrimeValues = CrimeGoldUseDefaults ? GetDefaultCrimeValues() : CrimeValues;
        faction.JailOutfit = JailOutfit;
        faction.SharedCrimeFactionList = SharedCrimeFactionList;
        faction.ExteriorJailMarker = ExteriorJailMarker;
        faction.FollowerWaitMarker = FollowerWaitMarker;
        faction.PlayerInventoryContainer = PlayerInventoryContainer;
        faction.StolenGoodsContainer = StolenGoodsContainer;

        faction.VendorValues = VendorValues;
        faction.VendorLocation = VendorLocation;
        faction.VendorBuySellList = VendorBuySellList;
        faction.MerchantContainer = MerchantContainer;
        faction.Conditions = Conditions.Select(c => c.ToCondition()).ToExtendedList();
    }

    private static CrimeValues GetDefaultCrimeValues() {
        return new CrimeValues {
            Arrest = true,
            AttackOnSight = true,
            Murder = 1000,
            Assault = 40,
            Pickpocket = 25,
            Trespass = 5,
            StealMult = 0.5f,
            Escape = 100,
            Werewolf = 1000,
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
