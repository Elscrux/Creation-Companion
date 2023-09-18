using System.Collections;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;
using CreationEditor.Services.Mutagen.FormLink;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Strings;
using Noggog;
namespace CreationEditor.Services.Query.Where;

public abstract class QueryValueCondition<TField> : QueryValueCondition<TField, TField> where TField : notnull {
    protected QueryValueCondition(IList<CompareFunction<TField, TField>> functions) : base(functions) {}
}

public sealed class NullValueCondition : QueryValueCondition<object> {
    private static IList<CompareFunction<object, object>> DefaultFunctions { get; } = new[] {
        new CompareFunction<object, object>(string.Empty, (_, _) => true)
    };

    public NullValueCondition() : base(DefaultFunctions) {}
}

public sealed class SimpleListValueCondition : QueryValueCondition<IEnumerable, object> {
    public override int Priority => -5;

    public SimpleListValueCondition() : base(new CompareFunction<IEnumerable, object>[] {
        new("One Matches", (list, listElement) =>
            list.Cast<object?>().Any(item => Equals(item, listElement))),
        new("All Match", (list, listElement) =>
            list.Cast<object?>().All(item => Equals(item, listElement))),
    }) {}

    public override bool Accepts(Type type) {
        if (!base.Accepts(type) || !type.IsGenericType || !type.InheritsFrom(typeof(IEnumerable))) return false;

        var genericArguments = type.GetGenericArguments();
        if (genericArguments.Length > 1) return false;

        var genericArgument = genericArguments[0];
        return IQueryCondition.IsPrimitiveType(genericArgument);
    }

    public override List<FieldType> GetFields() {
        var listType = UnderlyingType.GetGenericArguments()[0];

        const string compareValueName = nameof(CompareValue);
        return new List<FieldType> {
            new(listType, listType, compareValueName)
        };
    }
}

public sealed class ListCondition : QueryListCondition<IEnumerable, object> {
    private const string OneMatches = "One Matches";
    private const string AllMatch = "All Match";
    private static bool False(IEnumerable enumerable, object o) => false;

    public override int Priority => -10; // Ensure that this condition is not used for something like strings, as they would also match this condition

    public ListCondition(IQueryConditionEntryFactory queryConditionEntryFactory)
        : base(queryConditionEntryFactory, new CompareFunction<IEnumerable, object>[] {
            new(OneMatches, False),
            new(AllMatch, False),
        }) {}

    public override bool Evaluate(object? fieldValue) {
        if (fieldValue is not IList list) return false;

        bool CheckSubConditions(object? item) => SubConditions.EvaluateConditions(item);
        return SelectedFunction.Operator switch {
            OneMatches => list.Cast<object?>().Any(CheckSubConditions),
            AllMatch => list.Any() && list.Cast<object?>().All(CheckSubConditions),
            _ => false
        };
    }
}

public sealed class DictionaryCondition : QueryListCondition<IEnumerable, object> {
    private const string OneMatches = "One Matches";
    private const string AllMatch = "All Match";
    private static bool False(IEnumerable enumerable, object o) => false;

    public DictionaryCondition(IQueryConditionEntryFactory queryConditionEntryFactory)
        : base(queryConditionEntryFactory, new CompareFunction<IEnumerable, object>[] {
            new(OneMatches, False),
            new(AllMatch, False),
        }) {}

    public override bool Accepts(Type type) {
        var genericArguments = type.GetGenericArguments();
        if (genericArguments.Length != 2) return false;

        return type.InheritsFrom(typeof(IEnumerable));
    }

    public override List<FieldType> GetFields() {
        var keyValueType = typeof(IKeyValue<,>).MakeGenericType(UnderlyingType.GetGenericArguments());
        const string subConditionsName = nameof(SubConditions);
        return new List<FieldType> { new(typeof(IEnumerable), keyValueType, subConditionsName) };
    }

    public override bool Evaluate(object? fieldValue) {
        if (fieldValue is not IEnumerable enumerable) return false;

        bool CheckSubConditions(object? item) => SubConditions.EvaluateConditions(item);
        return SelectedFunction.Operator switch {
            OneMatches => enumerable.Cast<object?>().Any(CheckSubConditions),
            AllMatch => enumerable.Any() && enumerable.Cast<object?>().All(CheckSubConditions),
            _ => false
        };
    }
}

public sealed class ObjectCondition : QueryListCondition<object, object> {
    public override int Priority => int.MinValue;

    private static bool False(object o1, object o2) => false;
    public ObjectCondition(IQueryConditionEntryFactory queryConditionEntryFactory)
        : base(queryConditionEntryFactory, new CompareFunction<object, object>[] {
            new("Matching", False),
        }) {}

    public override bool Evaluate(object? fieldValue) {
        return SubConditions.EvaluateConditions(fieldValue);
    }
}

public sealed class TranslatedStringValueCondition : QueryValueCondition<ITranslatedStringGetter, string> {
    public override int Priority => 10;

    public TranslatedStringValueCondition() : base(QueryConditionHelper.GetStringFunctions<ITranslatedStringGetter>(TranslationFunction)) {}

    private static bool TranslationFunction(
        Func<string, string, bool> stringFunction,
        ITranslatedStringGetter translatedString,
        string text) {
        if (translatedString.String != null && stringFunction(translatedString.String, text)) return true;

        foreach (var (_, translation) in translatedString) {
            if (stringFunction(translation, text)) {
                return true;
            }
        }
        return false;
    }
}

public sealed class AssetLinkValueCondition : QueryValueCondition<IAssetLinkGetter, string> {
    public override int Priority => 10;

    public AssetLinkValueCondition() : base(QueryConditionHelper.GetStringFunctions<IAssetLinkGetter>(TranslationFunction)) {}

    private static bool TranslationFunction(
        Func<string, string, bool> stringFunction,
        IAssetLinkGetter translatedString,
        string text) {
        return stringFunction(translatedString.DataRelativePath, text);
    }
}

public sealed class FormLinkValueCondition : QueryValueCondition<IFormLinkGetter> {
    public FormLinkValueCondition() : base(new CompareFunction<IFormLinkGetter, IFormLinkGetter>[] {
        new("Equals", FormLinkIdentifierEqualityComparer.Instance.Equals),
    }) {}
}

public sealed class FormKeyValueCondition : QueryValueCondition<FormKey> {
    public FormKeyValueCondition() : base(new[] {
        new CompareFunction<FormKey, FormKey>("Equals", (x, y) => x == y),
    }) {}
}

public sealed class ColorValueCondition : QueryValueCondition<Color> {
    public ColorValueCondition() : base(new[] {
        new CompareFunction<Color, Color>("Equals", (x, y) => x == y),
    }) {}
}

public sealed class StringValueCondition : QueryValueCondition<string> {
    public StringValueCondition() : base(QueryConditionHelper.GetStringFunctions<string>((func, x, y) => func(x, y))) {}
}

public sealed class EnumFlagValueCondition : QueryValueCondition<Enum> {
    public override int Priority => 10;

    public override bool Accepts(Type type) => type.IsEnum && type.GetCustomAttribute<FlagsAttribute>() != null;

    public EnumFlagValueCondition() : base(new CompareFunction<Enum, Enum>[] {
        new("Equals", (x, y) => x.HasFlag(y)),
    }) {}
}

public sealed class EnumValueCondition : QueryValueCondition<Enum> {
    public EnumValueCondition() : base(new CompareFunction<Enum, Enum>[] {
        new("Equals", Equals),
    }) {}
}

public sealed class BoolValueCondition : QueryValueCondition<bool> {
    public BoolValueCondition() : base(new CompareFunction<bool, bool>[] {
        new("Equals", (x, y) => x == y),
    }) {}
}

public sealed class DoubleValueCondition : QueryValueCondition<double> {
    public DoubleValueCondition() : base(QueryConditionHelper.GetNumericFunctions<double>()) {}
}

public sealed class FloatValueCondition : QueryValueCondition<float> {
    public FloatValueCondition() : base(QueryConditionHelper.GetNumericFunctions<float>()) {}
}

public sealed class LongValueCondition : QueryValueCondition<long> {
    public LongValueCondition() : base(QueryConditionHelper.GetNumericFunctions<long>()) {}
}

public sealed class ULongValueCondition : QueryValueCondition<ulong> {
    public ULongValueCondition() : base(QueryConditionHelper.GetNumericFunctions<ulong>()) {}
}

public sealed class IntValueCondition : QueryValueCondition<int> {
    public IntValueCondition() : base(QueryConditionHelper.GetNumericFunctions<int>()) {}
}

public sealed class UIntValueCondition : QueryValueCondition<uint> {
    public UIntValueCondition() : base(QueryConditionHelper.GetNumericFunctions<uint>()) {}
}

public sealed class ShortValueCondition : QueryValueCondition<short> {
    public ShortValueCondition() : base(QueryConditionHelper.GetNumericFunctions<short>()) {}
}

public sealed class UShortValueCondition : QueryValueCondition<ushort> {
    public UShortValueCondition() : base(QueryConditionHelper.GetNumericFunctions<ushort>()) {}
}

public sealed class SByteValueCondition : QueryValueCondition<sbyte> {
    public SByteValueCondition() : base(QueryConditionHelper.GetNumericFunctions<sbyte>()) {}
}

public sealed class ByteValueCondition : QueryValueCondition<byte> {
    public ByteValueCondition() : base(QueryConditionHelper.GetNumericFunctions<byte>()) {}
}

public static class QueryConditionHelper {
    public static IList<CompareFunction<T, T>> GetNumericFunctions<T>() where T : IComparisonOperators<T, T, bool> {
        return new List<CompareFunction<T, T>> {
            new("Equals", (x, y) => x == y),
            new("Smaller Than", (x, y) => x < y),
            new("Smaller or Equal", (x, y) => x <= y),
            new("Greater Than", (x, y) => x > y),
            new("Greater or Equal", (x, y) => x >= y),
        };
    }

    public static IList<CompareFunction<T, string>> GetStringFunctions<T>(
        Func<Func<string, string, bool>, T, string, bool> translationFunction) {
        return new CompareFunction<T, string>[] {
            new("Equals", (a, b)
                => translationFunction(StringComparer.OrdinalIgnoreCase.Equals, a, b)),
            new("Contains", (a, b)
                => translationFunction((x, y) => x.Contains(y, StringComparison.OrdinalIgnoreCase), a, b)),
            new("Starts With", (a, b)
                => translationFunction((x, y) => x.StartsWith(y, StringComparison.OrdinalIgnoreCase), a, b)),
            new("Ends With", (a, b)
                => translationFunction((x, y) => x.EndsWith(y, StringComparison.OrdinalIgnoreCase), a, b)),
            new("Matches RegEx", (a, b)
                => translationFunction((x, y) => {
                    try {
                        return Regex.IsMatch(x, y);
                    } catch {
                        return false;
                    }
                }, a, b)),
        };
    }
}
