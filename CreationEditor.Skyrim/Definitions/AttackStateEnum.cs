using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions;

public class AttackStateEnum : IConditionValueEnums {
    private enum AttackState {
        None,
        DrawingWeapon,
        SwingingWeapon,
        Hit,
        NextAttack,
        FollowThrough,
        Bash
    }

    public IList<Enum> Enums { get; } = Enum.GetValues<AttackState>().Cast<Enum>().ToList();

    public bool Match(Condition.Function function) => function is Condition.Function.GetAttackState;
}
