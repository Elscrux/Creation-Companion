using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions.Enums;

public class TargetTypeEnum : IConditionValueEnums {
    public IList<Enum> Enums { get; } = Enum.GetValues<TargetType>().Cast<Enum>().ToList();

    public bool Match(Condition.Function function) => function is Condition.Function.GetCurrentDeliveryType;
}
