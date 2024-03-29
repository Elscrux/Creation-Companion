﻿using Mutagen.Bethesda.Skyrim.Internals;
namespace CreationEditor.Skyrim.Definitions.StoryManagerEvents;

public class DeadBody : AStoryManagerEvent {
    private enum DeadBodyEvent : ushort {
        Actor = 0x3152,
        DeadActor = 0x3252,
        Location = 0x314C,
    }

    public override IList<Enum> Enums { get; } = Enum.GetValues<DeadBodyEvent>().Cast<Enum>().ToList();
    public override int Type => RecordTypeInts.DEAD;
}
