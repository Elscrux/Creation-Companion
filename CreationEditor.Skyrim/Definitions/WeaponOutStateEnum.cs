using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions;

public class WeaponOutStateEnum : IConditionValueEnums {
    private enum WeaponOutState {
        NoWeaponDrawn,
        FistsOnly,
        AnyWeaponNotSpell
    }

    public IList<Enum> Enums { get; set; } = Enum.GetValues<WeaponOutState>().Cast<Enum>().ToList();

    public bool Match(Condition.Function function) => function is Condition.Function.IsWeaponOut;
}
