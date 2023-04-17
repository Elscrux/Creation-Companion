namespace CreationEditor.Skyrim.Definitions;

public abstract class AStoryManagerEvent {
    public abstract IList<Enum> Enums { get; }
    public IList<Enum> ReferenceEnums { get; }
    public IList<Enum> NonReferenceEnums { get; }

    public abstract int Type { get; }

    private static readonly IList<int> ReferenceValues = new List<int> { 0x3152, 0x3252 };

    protected AStoryManagerEvent() {
        ReferenceEnums = Enums.Where(x => {
            var int32 = Convert.ToInt32(x);
            return ReferenceValues.Any(y => y == int32);
        }).ToList();
        NonReferenceEnums = Enums.Where(x => {
            var int32 = Convert.ToInt32(x);
            return ReferenceValues.All(y => y != int32);
        }).ToList();
    } 
}
