using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions;

public class CastTypeEnum : IConditionValueEnums {
    public IList<Enum> Enums { get; } = Enum.GetValues<CastType>().Cast<Enum>().ToList();

    public bool Match(Condition.Function function) => function is Condition.Function.GetCurrentCastingType;
}
