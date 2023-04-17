using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions;

public interface IConditionValueEnums {
    public IList<Enum> Enums { get; }
    public bool Match(Condition.Function function);
}
