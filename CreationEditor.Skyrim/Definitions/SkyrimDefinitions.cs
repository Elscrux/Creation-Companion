namespace CreationEditor.Skyrim.Definitions;

public static class SkyrimDefinitions {
    public static readonly IList<AStoryManagerEvent> StoryManagerEvents
        = typeof(AStoryManagerEvent)
            .GetAllSubClass<AStoryManagerEvent>()
            .ToList();

    public static readonly IList<IConditionValueEnums> ConditionValueEnums
        = typeof(IConditionValueEnums)
            .GetAllSubClass<IConditionValueEnums>()
            .ToList();
}
