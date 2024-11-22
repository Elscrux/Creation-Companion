using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions.Enums;

public interface IConditionValueEnums {
    IList<Enum> Enums { get; }
    bool Match(Condition.Function function);
}
