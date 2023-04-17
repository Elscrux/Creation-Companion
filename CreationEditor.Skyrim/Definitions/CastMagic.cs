﻿using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions;

public class CastMagic : AStoryManagerEvent {
    public enum CastMagicEvent : ushort {
        CastingActor = 0x3152,
        SpellTarget = 0x3252,
        Location = 0x314C,
        SpellForm = 0x3146,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<CastMagicEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.CAST;
}
