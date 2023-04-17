using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions;

public class TargetTypeEnum : IConditionValueEnums {
    public IList<Enum> Enums { get; set; } = Enum.GetValues<TargetType>().Cast<Enum>().ToList();

    public bool Match(Condition.Function function) => function is Condition.Function.GetCurrentDeliveryType;
}
