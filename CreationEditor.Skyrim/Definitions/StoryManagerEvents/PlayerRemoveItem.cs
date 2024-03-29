﻿using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions.StoryManagerEvents;

public class PlayerRemoveItem : AStoryManagerEvent {
    private enum PlayerRemoveItemEvent : ushort {
        OwnerRef = 0x3152,
        ItemRef = 0x3252,
        Location = 0x314C,
        ObjectForm = 0x3146,
        RemoveType = 0x3156,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<PlayerRemoveItemEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.REMP;
}
