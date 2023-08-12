using System.Collections;
using System.Numerics;
using System.Reactive.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CreationEditor.Services.Mutagen.FormLink;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Strings;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Services.Query;

public interface ICompareFunction {
    string Operator { get; }

    bool Evaluate(object field, object value);
}

public sealed record CompareFunction<TField, TValue>(string Operator, Func<TField, TValue, bool> Evaluate) : ICompareFunction {
    bool ICompareFunction.Evaluate(object field, object value) {
        if (field is TField fieldT && value is TValue valueT) {
            return Evaluate(fieldT, valueT);
        }
        return false;
    }
}

public interface IQueryCondition {
    int Priority { get; }

    Type FieldTypeClass { get; }
    Type CompareValueType { get; }
    Type ActualFieldType { get; set; }
    IList<ICompareFunction> Functions { get; }
    ICompareFunction SelectedFunction { get; set; }

    IObservable<string> Summary { get; }

    bool Accepts(Type type);
    bool Evaluate(object? fieldValue);
}

public interface IQueryValueCondition : IQueryCondition {
    object? CompareValue { get; set; }
}

public interface IQueryListCondition : IQueryCondition {
    IObservableCollection<IQueryConditionEntry> SubConditions { get; }
}

public abstract class QueryCondition<TField, TCompareValue> : ReactiveObject, IQueryValueCondition
    where TField : notnull {
    public int Priority { get; protected init; }
    public virtual Type FieldTypeClass => typeof(TField);
    public Type ActualFieldType { get; set; } = typeof(TField);
    public virtual Type CompareValueType => typeof(TCompareValue);

    public ICompareFunction SelectedFunction {
        get => _selectedFunction;
        set {
            if (value is CompareFunction<TField, TCompareValue> function) {
                this.RaiseAndSetIfChanged(ref _selectedFunction, function);
            }
        }
    }
    IList<ICompareFunction> IQueryCondition.Functions => _functions.Cast<ICompareFunction>().ToArray();
    public object? CompareValue {
        get => _compareValue;
        set {
            if (value is TCompareValue field) {
                this.RaiseAndSetIfChanged(ref _compareValue, field);
            } else if (value is IConvertible) {
                if (Convert.ChangeType(value, typeof(TCompareValue)) is TCompareValue converted) {
                    this.RaiseAndSetIfChanged(ref _compareValue, converted);
                }
            }
        }
    }

    private readonly IList<CompareFunction<TField, TCompareValue>> _functions;
    private CompareFunction<TField, TCompareValue> _selectedFunction;
    private TCompareValue? _compareValue;
    public IObservable<string> Summary { get; }

    protected QueryCondition(IList<CompareFunction<TField, TCompareValue>> functions) {
        _functions = functions;
        SelectedFunction = _selectedFunction = _functions.First();

        Summary = this.WhenAnyValue(
            x => x.SelectedFunction,
            x => x.CompareValue,
            (function, value) => function.Operator + " " + value);
    }

    public override string ToString() {
        return SelectedFunction.Operator + " " + CompareValue;
    }

    public virtual bool Accepts(Type type) => type.InheritsFrom(FieldTypeClass);
    public bool Evaluate(object? fieldValue) {
        if (CompareValue is null || fieldValue is not TField field) return false;

        return SelectedFunction.Evaluate(field, CompareValue);
    }
}

public abstract class QueryCondition<TField> : QueryCondition<TField, TField> where TField : notnull {
    protected QueryCondition(IList<CompareFunction<TField, TField>> functions) : base(functions) {}
}

public abstract class QueryListCondition<TField, TValue> : ReactiveObject, IQueryListCondition
    where TField : notnull {
    public int Priority { get; protected init; }
    public Type FieldTypeClass => typeof(TField);
    public Type ActualFieldType { get; set; } = typeof(TField);
    public Type CompareValueType => typeof(TValue);

    public ICompareFunction SelectedFunction {
        get => _selectedFunction;
        set {
            if (value is CompareFunction<TField, TValue> function) {
                this.RaiseAndSetIfChanged(ref _selectedFunction, function);
            }
        }
    }
    IList<ICompareFunction> IQueryCondition.Functions => _functions.Cast<ICompareFunction>().ToArray();

    private readonly IList<CompareFunction<TField, TValue>> _functions;
    private CompareFunction<TField, TValue> _selectedFunction;
    public IObservableCollection<IQueryConditionEntry> SubConditions { get; } = new ObservableCollectionExtended<IQueryConditionEntry>();
    public IObservable<string> Summary { get; }

    protected QueryListCondition(IList<CompareFunction<TField, TValue>> functions) {
        _functions = functions;
        SelectedFunction = _selectedFunction = _functions.First();

        Summary = SubConditions.Select(x => x.Summary).CombineLatest()
            .CombineLatest(this.WhenAnyValue(x => x.SelectedFunction),
                (list, function) => function.Operator + " " + string.Join(' ', list));
    }

    public override string ToString() {
        return SelectedFunction.Operator + " "
          + string.Join(' ', SubConditions.Select(x => x.ToString()));
    }

    public virtual bool Accepts(Type type) => type.InheritsFrom(FieldTypeClass);
    public abstract bool Evaluate(object? fieldValue);
}

public sealed class NullCondition : QueryCondition<object> {
    private static IList<CompareFunction<object, object>> DefaultFunctions { get; } = new[] {
        new CompareFunction<object, object>(string.Empty, (_, _) => true)
    };

    public NullCondition() : base(DefaultFunctions) {}
}

public sealed class SimpleListCondition : QueryCondition<IEnumerable, object> {
    public SimpleListCondition() : base(new CompareFunction<IEnumerable, object>[] {
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

    public ListCondition() : base(new CompareFunction<IEnumerable, object>[] {
        new(OneMatches, False),
        new(AllMatch, False),
    }) {
        Priority = -10; // Don't use this condition for something like strings, although they also match this condition
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
    public ObjectCondition() : base(new CompareFunction<object, object>[] {
        new("Matching", False),
    }) {
        Priority = int.MinValue;
    }

    public override bool Evaluate(object? fieldValue) {
        return SubConditions.EvaluateConditions(fieldValue);
    }
}

public sealed class TranslatedStringCondition : QueryCondition<ITranslatedStringGetter, string> {
    public TranslatedStringCondition() : base(QueryConditionHelper.GetStringFunctions<ITranslatedStringGetter>(TranslationFunction)) {
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

public sealed class AssetLinkCondition : QueryCondition<IAssetLinkGetter, string> {
    public AssetLinkCondition() : base(QueryConditionHelper.GetStringFunctions<IAssetLinkGetter>(TranslationFunction)) {
        Priority = 10;
    }

    private static bool TranslationFunction(
        Func<string, string, bool> stringFunction,
        IAssetLinkGetter translatedString,
        string text) {
        return stringFunction(translatedString.DataRelativePath, text);
    }
}

public sealed class FormLinkCondition : QueryCondition<IFormLinkGetter> {
    public FormLinkCondition() : base(new CompareFunction<IFormLinkGetter, IFormLinkGetter>[] {
        new("Equals", FormLinkIdentifierEqualityComparer.Instance.Equals),
    }) {}
}

public sealed class FormKeyCondition : QueryCondition<FormKey> {
    public FormKeyCondition() : base(new[] {
        new CompareFunction<FormKey, FormKey>("Equals", (x, y) => x == y),
    }) {}
}

public sealed class StringCondition : QueryCondition<string> {
    public StringCondition() : base(QueryConditionHelper.GetStringFunctions<string>((func, x, y) => func(x, y))) {}
}

public sealed class EnumFlagCondition : QueryCondition<Enum> {
    public override bool Accepts(Type type) => type.IsEnum && type.GetCustomAttribute<FlagsAttribute>() != null;

    public EnumFlagCondition() : base(new CompareFunction<Enum, Enum>[] {
        new("Equals", (x, y) => x.HasFlag(y)),
    }) {
        Priority = 10;
    }
}

public sealed class EnumCondition : QueryCondition<Enum> {
    public EnumCondition() : base(new CompareFunction<Enum, Enum>[] {
        new("Equals", Equals),
    }) {}
}

public sealed class BoolCondition : QueryCondition<bool> {
    public BoolCondition() : base(new CompareFunction<bool, bool>[] {
        new("Equals", (x, y) => x == y),
    }) {}
}

public sealed class DoubleCondition : QueryCondition<double> {
    public DoubleCondition() : base(QueryConditionHelper.GetNumericFunctions<double>()) {}
}

public sealed class FloatCondition : QueryCondition<float> {
    public FloatCondition() : base(QueryConditionHelper.GetNumericFunctions<float>()) {}
}

public sealed class LongCondition : QueryCondition<long> {
    public LongCondition() : base(QueryConditionHelper.GetNumericFunctions<long>()) {}
}

public sealed class ULongCondition : QueryCondition<ulong> {
    public ULongCondition() : base(QueryConditionHelper.GetNumericFunctions<ulong>()) {}
}

public sealed class IntCondition : QueryCondition<int> {
    public IntCondition() : base(QueryConditionHelper.GetNumericFunctions<int>()) {}
}

public sealed class UIntCondition : QueryCondition<uint> {
    public UIntCondition() : base(QueryConditionHelper.GetNumericFunctions<uint>()) {}
}

public sealed class ShortCondition : QueryCondition<short> {
    public ShortCondition() : base(QueryConditionHelper.GetNumericFunctions<short>()) {}
}

public sealed class UShortCondition : QueryCondition<ushort> {
    public UShortCondition() : base(QueryConditionHelper.GetNumericFunctions<ushort>()) {}
}

public sealed class SByteCondition : QueryCondition<sbyte> {
    public SByteCondition() : base(QueryConditionHelper.GetNumericFunctions<sbyte>()) {}
}

public sealed class ByteCondition : QueryCondition<byte> {
    public ByteCondition() : base(QueryConditionHelper.GetNumericFunctions<byte>()) {}
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
