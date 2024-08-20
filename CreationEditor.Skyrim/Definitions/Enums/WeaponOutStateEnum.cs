using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions.Enums;

public class WeaponOutStateEnum : IConditionValueEnums {
    private enum WeaponOutState {
        NoWeaponDrawn,
        FistsOnly,
        AnyWeaponNotSpell,
    }

    public IList<Enum> Enums { get; } = Enum.GetValues<WeaponOutState>().Cast<Enum>().ToList();

    public bool Match(Condition.Function function) => function is Condition.Function.IsWeaponOut;
}
