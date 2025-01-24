namespace CreationEditor.Skyrim.Definitions.StoryManagerEvents;

public abstract class AStoryManagerEvent {
    private static readonly IList<int> ReferenceValues = new List<int> { 0x3152, 0x3252 };

    public abstract int Type { get; }
    public abstract IList<Enum> Enums { get; }
    public IEnumerable<Enum> ReferenceEnums =>  Enums.Where(x => {
        var int32 = Convert.ToInt32(x);
        return ReferenceValues.Any(y => y == int32);
    });
    public IEnumerable<Enum> NonReferenceEnums => Enums.Where(x => {
        var int32 = Convert.ToInt32(x);
        return ReferenceValues.All(y => y != int32);
    });
}
