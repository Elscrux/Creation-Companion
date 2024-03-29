﻿using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Definitions.Enums;

public class LockLevelEnum : IConditionValueEnums {
    public IList<Enum> Enums { get; } = Enum.GetValues<LockLevel>().Cast<Enum>().ToList();

    public bool Match(Condition.Function function) => function is Condition.Function.GetLockLevel;
}
