using System.Collections;
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
    public SimpleListValueCondition() : base(new CompareFunction<IEnumerable, object>[] {
        new("One Matches", (list, listElement) =>
            list.Cast<object?>().Any(item => Equals(item, listElement))),
        new("All Match", (list, listElement) =>
            list.Cast<object?>().All(item => Equals(item, listElement))),
    }) {
        Priority = -5;
    }

    public override bool Accepts(Type type) {
        if (!base.Accepts(type) || !type.IsGenericType) return false;

        var genericArgument = type.GetGenericArguments()[0];
        return genericArgument == typeof(FormKey)
         || genericArgument.InheritsFrom(typeof(IFormLinkGetter))
         || genericArgument == typeof(int)
         || genericArgument == typeof(uint)
         || genericArgument == typeof(long)
         || genericArgument == typeof(ulong)
         || genericArgument == typeof(short)
         || genericArgument == typeof(ushort)
         || genericArgument == typeof(float)
         || genericArgument == typeof(double)
         || genericArgument == typeof(sbyte)
         || genericArgument == typeof(byte);
    }
}

public sealed class ListCondition : QueryListCondition<IEnumerable, object> {
    private const string OneMatches = "One Matches";
    private const string AllMatch = "All Match";
    private static bool False(IEnumerable enumerable, object o) => false;

    public ListCondition(IQueryConditionEntryFactory queryConditionEntryFactory)
        : base(queryConditionEntryFactory, new CompareFunction<IEnumerable, object>[] {
            new(OneMatches, False),
            new(AllMatch, False),
        }) {
        Priority = -10; // Ensure that this condition is not used for something like strings, as they would also match this condition
    }


    public override bool Evaluate(object? fieldValue) {
        if (fieldValue is not IEnumerable list) return false;

        bool CheckSubConditions(object? item) => SubConditions.EvaluateConditions(item);
        return SelectedFunction.Operator switch {
            OneMatches => list.Cast<object?>().Any(CheckSubConditions),
            AllMatch => list.Any() && list.Cast<object?>().All(CheckSubConditions),
            _ => false
        };
    }
}

public sealed class ObjectCondition : QueryListCondition<object, object> {
    private static bool False(object o1, object o2) => false;
    public ObjectCondition(IQueryConditionEntryFactory queryConditionEntryFactory)
        : base(queryConditionEntryFactory, new CompareFunction<object, object>[] {
            new("Matching", False),
        }) {
        Priority = int.MinValue;
    }

    public override bool Evaluate(object? fieldValue) {
        return SubConditions.EvaluateConditions(fieldValue);
    }
}

public sealed class TranslatedStringValueCondition : QueryValueCondition<ITranslatedStringGetter, string> {
    public TranslatedStringValueCondition() : base(QueryConditionHelper.GetStringFunctions<ITranslatedStringGetter>(TranslationFunction)) {
        Priority = 10;
    }

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
    public AssetLinkValueCondition() : base(QueryConditionHelper.GetStringFunctions<IAssetLinkGetter>(TranslationFunction)) {
        Priority = 10;
    }

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

public sealed class StringValueCondition : QueryValueCondition<string> {
    public StringValueCondition() : base(QueryConditionHelper.GetStringFunctions<string>((func, x, y) => func(x, y))) {}
}

public sealed class EnumFlagValueCondition : QueryValueCondition<Enum> {
    public override bool Accepts(Type type) => type.IsEnum && type.GetCustomAttribute<FlagsAttribute>() != null;

    public EnumFlagValueCondition() : base(new CompareFunction<Enum, Enum>[] {
        new("Equals", (x, y) => x.HasFlag(y)),
    }) {
        Priority = 10;
    }
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
