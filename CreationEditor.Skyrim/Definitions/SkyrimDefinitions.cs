using CreationEditor.Skyrim.Definitions.Enums;
using CreationEditor.Skyrim.Definitions.StoryManagerEvents;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Skyrim.Definitions;

public static class SkyrimDefinitions {
    public static readonly ModKey[] SkyrimModKeys = {
        Mutagen.Bethesda.FormKeys.SkyrimSE.Skyrim.ModKey,
        Update.ModKey,
        Dawnguard.ModKey,
        HearthFires.ModKey,
        Dragonborn.ModKey,
    };

    public static readonly IList<AStoryManagerEvent> StoryManagerEvents
        = typeof(AStoryManagerEvent)
            .GetAllSubClass<AStoryManagerEvent>()
            .ToList();

    public static readonly IList<IConditionValueEnums> ConditionValueEnums
        = typeof(IConditionValueEnums)
            .GetAllSubClass<IConditionValueEnums>()
            .ToList();
}
