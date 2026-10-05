using CreationEditor.Avalonia.Models.Mod.Editor;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Skyrim.Avalonia.Models.Record.Editor.MajorRecord;

[IReactiveObject]
public sealed partial class EditableFaction : Faction, IEditableRecord<Faction> {
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
            this.RaisePropertyChanged();
        }
    }

    public bool SpecialCombat {
        get => (Flags & FactionFlag.SpecialCombat) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.SpecialCombat, value);
            this.RaisePropertyChanged();
        }
    }

    public bool CanBeOwner {
        get => (Flags & FactionFlag.CanBeOwner) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.CanBeOwner, value);
            this.RaisePropertyChanged();
        }
    }

    public string? NameStr {
        get => Name?.String;
        set {
            Name = value;
            this.RaisePropertyChanged();
        }
    }

    public bool IgnoreAssault {
        get => (Flags & FactionFlag.IgnoreAssault) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.IgnoreAssault, value);
            this.RaisePropertyChanged();
        }
    }

    public bool IgnoreMurder {
        get => (Flags & FactionFlag.IgnoreMurder) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.IgnoreMurder, value);
            this.RaisePropertyChanged();
        }
    }

    public bool IgnorePickpocket {
        get => (Flags & FactionFlag.IgnorePickpocket) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.IgnorePickpocket, value);
            this.RaisePropertyChanged();
        }
    }

    public bool IgnoreStealing {
        get => (Flags & FactionFlag.IgnoreStealing) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.IgnoreStealing, value);
            this.RaisePropertyChanged();
        }
    }

    public bool IgnoreTrespass {
        get => (Flags & FactionFlag.IgnoreTrespass) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.IgnoreTrespass, value);
            this.RaisePropertyChanged();
        }
    }

    public bool IgnoreWerewolf {
        get => (Flags & FactionFlag.IgnoreWerewolf) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.IgnoreWerewolf, value);
            this.RaisePropertyChanged();
        }
    }

    public bool DoNotReportCrimesAgainstMembers {
        get => (Flags & FactionFlag.DoNotReportCrimesAgainstMembers) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.DoNotReportCrimesAgainstMembers, value);
            this.RaisePropertyChanged();
        }
    }

    public bool TrackCrime {
        get => (Flags & FactionFlag.TrackCrime) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.TrackCrime, value);
            this.RaisePropertyChanged();
        }
    }

    public bool CrimeGoldUseDefaults {
        get => (Flags & FactionFlag.CrimeGoldUseDefaults) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.CrimeGoldUseDefaults, value);
            CrimeValues = GetDefaultCrimeValues();
            this.RaisePropertyChanged();
        }
    }

    public bool Vendor {
        get => (Flags & FactionFlag.Vendor) != 0;
        set {
            Flags = Flags.SetFlag(FactionFlag.Vendor, value);
            this.RaisePropertyChanged();
        }
    }

    public EditableFaction(IFactionGetter parent) {
        FormKey = parent.FormKey;
        this.DeepCopyIn(parent);
        Relations = new ObservableCollectionExtended<EditableRelation>(parent.Relations.Select(r => new EditableRelation {
            Reaction = r.Reaction, TargetFormKey = r.Target.FormKey,
        }));

        Ranks = new ObservableCollectionExtended<Rank>(parent.Ranks.Select(r => r.DeepCopy()));

        CrimeValues = parent.CrimeValues?.DeepCopy() ?? GetDefaultCrimeValues();

        VendorValues = parent.VendorValues?.DeepCopy() ?? new VendorValues();
        VendorLocation = parent.VendorLocation?.DeepCopy() ?? new LocationTargetRadius();
        Conditions = parent.Conditions is null
            ? []
            : new ObservableCollectionExtended<EditableCondition>(parent.Conditions.Select(c => new EditableCondition(c)));
    }

    public void CopyTo(Faction faction) {
        faction.DeepCopyIn(this);
        faction.Relations.ReplaceWith(Relations
            .Where(r => !r.TargetFormKey.IsNull)
            .Select(r => r.ToRelation()));

        for (var i = 0; i < Ranks.Count; i++) {
            Ranks[i].Number = (uint) i;
        }
        faction.Ranks.ReplaceWith(Ranks);

        faction.CrimeValues = CrimeGoldUseDefaults ? GetDefaultCrimeValues() : CrimeValues;

        faction.VendorValues = VendorValues;
        faction.Conditions = Conditions.Select(c => c.ToCondition()).ToExtendedList();
    }

    private CrimeValues GetDefaultCrimeValues() => new() {
        Arrest = CrimeValues.Arrest,
        AttackOnSight = CrimeValues.AttackOnSight,
        Murder = 1000,
        Assault = 40,
        Pickpocket = 25,
        Trespass = 5,
        StealMult = 0.5f,
        Escape = 100,
        Werewolf = 1000,
    };
}
