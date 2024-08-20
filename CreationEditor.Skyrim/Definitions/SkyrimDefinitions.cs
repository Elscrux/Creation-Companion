using System.Collections.Immutable;
using CreationEditor.Skyrim.Definitions.Enums;
using CreationEditor.Skyrim.Definitions.StoryManagerEvents;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Skyrim.Definitions;

public static class SkyrimDefinitions {
    public static readonly ModKey[] SkyrimModKeys = [
        Mutagen.Bethesda.FormKeys.SkyrimSE.Skyrim.ModKey,
        Update.ModKey,
        Dawnguard.ModKey,
        HearthFires.ModKey,
        Dragonborn.ModKey,
    ];

    public static readonly ImmutableArray<AStoryManagerEvent> StoryManagerEvents
        = [
            ..typeof(AStoryManagerEvent).GetAllSubClasses<AStoryManagerEvent>(),
        ];

    public static readonly ImmutableArray<IConditionValueEnums> ConditionValueEnums
        = [
            ..typeof(IConditionValueEnums).GetAllSubClasses<IConditionValueEnums>(),
        ];
}
