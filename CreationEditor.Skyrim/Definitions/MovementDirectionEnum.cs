using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions;

public class MovementDirectionEnum : IConditionValueEnums {
    private enum MovementDirection {
        NotMoving,
        Forward,
        Right,
        Back,
        Left
    }

    public IList<Enum> Enums { get; set; } = Enum.GetValues<MovementDirection>().Cast<Enum>().ToList();

    public bool Match(Condition.Function function) => function
        is Condition.Function.GetMovementDirection
        or Condition.Function.IsMoving;
}
