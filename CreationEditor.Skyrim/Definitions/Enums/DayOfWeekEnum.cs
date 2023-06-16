using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions.Enums;

public class DayOfWeekEnum : IConditionValueEnums {
    public IList<Enum> Enums { get; } = Enum.GetValues<DayOfWeek>().Cast<Enum>().ToList();

    public bool Match(Condition.Function function) => function is Condition.Function.GetDayOfWeek;
}
