using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions;

public class WeaponAnimationTypeEnum : IConditionValueEnums {
    public IList<Enum> Enums { get; } = Enum.GetValues<WeaponAnimationType>().Cast<Enum>().ToList();

    public bool Match(Condition.Function function) => function
        is Condition.Function.GetEquippedItemType
        or Condition.Function.GetReplacedItemType
        or Condition.Function.GetIsUsedItemType
        or Condition.Function.GetWeaponAnimType;
}
