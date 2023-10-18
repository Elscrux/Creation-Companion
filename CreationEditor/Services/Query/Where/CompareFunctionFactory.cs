using System.Collections;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.FormLink;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Strings;
using Noggog;
namespace CreationEditor.Services.Query.Where;

public sealed class CompareFunctionFactory : ICompareFunctionFactory {
    private readonly List<FunctionCategory> _compareFunctions = new();

    private static bool CheckSubConditions(ConditionState conditionState, object? item) =>
        conditionState.SubConditions.EvaluateConditions(item);

    private static bool DefaultAccepts<T>(Type type) => type.InheritsFrom(typeof(T));
    private static bool SimpleListAccepts<T>(Type type) {
        if (!DefaultAccepts<T>(type) || !type.IsGenericType || !type.InheritsFrom(typeof(IEnumerable))) return false;

        var genericArguments = type.GetGenericArguments();
        if (genericArguments.Length > 1) return false;

        var genericArgument = genericArguments[0];
        return ConditionState.IsPrimitiveType(genericArgument);
    }
    private static bool DictionaryAccepts(Type type) {
        var genericArguments = type.GetGenericArguments();
        return genericArguments.Length == 2 && type.InheritsFrom(typeof(IEnumerable));
    }

    public CompareFunctionFactory(
        ILinkCacheProvider linkCacheProvider) {

        // Simple List
        Func<Type, IEnumerable<FieldType>> simpleListFieldOverride = type => {
            var listType = type.GetGenericArguments()[0];
            return new FieldType[] {
                new(listType, listType, FieldCategory.Value)
            };
        };

        RegisterCompareFunction(
            new ICompareFunction[] {
                new CompareFunction<IEnumerable, object>(
                    "One Matches",
                    (context, enumerable) => enumerable.Cast<object?>().Any(item => Equals(item, context.CompareValue)),
                    simpleListFieldOverride),
                new CompareFunction<IEnumerable, object>(
                    "All Match",
                    (context, enumerable) => enumerable.Cast<object?>().All(item => Equals(item, context.CompareValue)),
                    simpleListFieldOverride)
            },
            SimpleListAccepts<IEnumerable>,
            -50);

        // Dictionary
        Func<Type, IEnumerable<FieldType>> dictionaryFieldOverride = type => {
            var keyValueType = typeof(IKeyValue<,>).MakeGenericType(type.GetGenericArguments());
            return new FieldType[] { new(typeof(IEnumerable), keyValueType, FieldCategory.Collection) };
        };
        RegisterCompareFunction(new ICompareFunction[] {
                new CompareFunction<IEnumerable, object>(
                    "One Matches",
                    (context, enumerable) => enumerable.Cast<object?>().Any(o => CheckSubConditions(context, o)),
                    dictionaryFieldOverride),
                new CompareFunction<IEnumerable, object>(
                    "All Matches",
                    (context, enumerable) => enumerable.Any() && enumerable.Cast<object?>().All(o => CheckSubConditions(context, o)),
                    dictionaryFieldOverride),
            },
            DictionaryAccepts, -10);

        // List
        RegisterCompareFunction<IEnumerable>(new ICompareFunction[] {
                new CompareFunction<IEnumerable, object>(
                    "One Matches",
                    (context, enumerable) => enumerable.Cast<object?>().Any(o => CheckSubConditions(context, o)),
                    FieldCategory.Collection),
                new CompareFunction<IEnumerable, object>(
                    "All Matches",
                    (context, enumerable) => enumerable.Any() && enumerable.Cast<object?>().All(o => CheckSubConditions(context, o)),
                    FieldCategory.Collection)
            },
            -100);

        // Translated String
        RegisterCompareFunction<ITranslatedStringGetter>(
            GetStringFunctions<ITranslatedStringGetter>(
                (stringFunction, translatedString, text) => {
                    if (translatedString.String != null && stringFunction(translatedString.String, text)) return true;

                    foreach (var (_, translation) in translatedString) {
                        if (stringFunction(translation, text)) {
                            return true;
                        }
                    }
                    return false;
                }),
            10);

        // Asset Link
        RegisterCompareFunction<IAssetLinkGetter>(
            GetStringFunctions<IAssetLinkGetter>(
                (stringFunction, translatedString, text) =>
                    stringFunction(translatedString.DataRelativePath, text)),
            10);

        // String
        RegisterCompareFunction<string>(GetStringFunctions<string>(
            (stringFunction, a, b) => stringFunction(a, b)));

        // Form Link
        RegisterCompareFunction<IFormLinkGetter>(new ICompareFunction[] {
            new CompareFunction<IFormLinkGetter, IFormLinkGetter>(
                "Equals",
                (context, formLink) =>
                    context.CompareValue is IFormLinkGetter compareFormLink
                 && FormLinkIdentifierEqualityComparer.Instance.Equals(formLink, compareFormLink)),
            new CompareFunction<IFormLinkGetter, object>(
                "Matching",
                (context, formLink) =>
                    linkCacheProvider.LinkCache.TryResolve(formLink, out var record)
                 && context.SubConditions.EvaluateConditions(record),
                FieldCategory.Collection),
        });

        // Form Key
        RegisterCompareFunction(new[] { GetSimpleEquals<FormKey>() });

        // Color
        RegisterCompareFunction(new[] { GetSimpleEquals<Color>() });

        // Enum
        RegisterCompareFunction(new[] {
                new CompareFunction<Enum, Enum>(
                    "Equals",
                    (context, e) => context.CompareValue is Enum flag && e.HasFlag(flag))
            },
            type => type.IsEnum && type.GetCustomAttribute<FlagsAttribute>() != null,
            10);

        RegisterCompareFunction(new[] {
            new CompareFunction<Enum, Enum>(
                "Equals",
                (context, e) => context.CompareValue is Enum other && e.Equals(other))
        });

        // Bool
        RegisterCompareFunction(new[] { GetSimpleEquals<bool>() });

        // Numeric
        RegisterCompareFunction(GetNumericFunctions<double>());
        RegisterCompareFunction(GetNumericFunctions<float>());
        RegisterCompareFunction(GetNumericFunctions<long>());
        RegisterCompareFunction(GetNumericFunctions<ulong>());
        RegisterCompareFunction(GetNumericFunctions<int>());
        RegisterCompareFunction(GetNumericFunctions<uint>());
        RegisterCompareFunction(GetNumericFunctions<short>());
        RegisterCompareFunction(GetNumericFunctions<ushort>());
        RegisterCompareFunction(GetNumericFunctions<sbyte>());
        RegisterCompareFunction(GetNumericFunctions<byte>());

        // Object
        RegisterCompareFunction(new[] {
                new CompareFunction<object, object>("Matching", CheckSubConditions, FieldCategory.Collection)
            },
            int.MinValue);

        // Pre-sort
        _compareFunctions.Sort((a, b) => b.Priority.CompareTo(a.Priority));
    }

    public static IEnumerable<ICompareFunction> GetStringFunctions<T>(
        Func<Func<string, string, bool>, T, string, bool> translationFunction) {
        yield return new CompareFunction<T, string>("Equals", (a, b)
            => translationFunction(StringComparer.OrdinalIgnoreCase.Equals, a, b));
        yield return new CompareFunction<T, string>("Contains", (a, b)
            => translationFunction((x, y) => x.Contains(y, StringComparison.OrdinalIgnoreCase), a, b));
        yield return new CompareFunction<T, string>("Starts With", (a, b)
            => translationFunction((x, y) => x.StartsWith(y, StringComparison.OrdinalIgnoreCase), a, b));
        yield return new CompareFunction<T, string>("Ends With", (a, b)
            => translationFunction((x, y) => x.EndsWith(y, StringComparison.OrdinalIgnoreCase), a, b));
        yield return new CompareFunction<T, string>("Matches RegEx", (a, b)
            => translationFunction((x, y) => {
                try {
                    return Regex.IsMatch(x, y);
                } catch {
                    return false;
                }
            }, a, b));
    }

    public static IEnumerable<CompareFunction<T, T>> GetNumericFunctions<T>()
        where T : IComparisonOperators<T, T, bool> {
        yield return new CompareFunction<T, T>("Equals", (x, y) => x == y);
        yield return new CompareFunction<T, T>("Smaller Than", (x, y) => x < y);
        yield return new CompareFunction<T, T>("Smaller or Equal", (x, y) => x <= y);
        yield return new CompareFunction<T, T>("Greater Than", (x, y) => x > y);
        yield return new CompareFunction<T, T>("Greater or Equal", (x, y) => x >= y);
    }

    public static CompareFunction<T, T> GetSimpleEquals<T>() {
        return new CompareFunction<T, T>(
            "Equals",
            (a, b) => Equals(a, b));
    }

    public void RegisterCompareFunction<T>(IEnumerable<ICompareFunction> compareFunctions, int priority = 0) {
        _compareFunctions.Add(new FunctionCategory(compareFunctions, DefaultAccepts<T>, priority));
    }

    public void RegisterCompareFunction<T>(IEnumerable<CompareFunction<T, T>> compareFunctions, int priority = 0) {
        _compareFunctions.Add(new FunctionCategory(compareFunctions, DefaultAccepts<T>, priority));
    }

    public void RegisterCompareFunction(IEnumerable<ICompareFunction> compareFunctions, Func<Type, bool> accepts, int priority = 0) {
        _compareFunctions.Add(new FunctionCategory(compareFunctions, accepts, priority));
    }

    public IEnumerable<ICompareFunction> Get(Type type) {
        if (type.InheritsFrom(typeof(ReadOnlyMemorySlice<>))) return Array.Empty<ICompareFunction>();

        type = type.InheritsFrom(typeof(Nullable<>))
            ? type.GetGenericArguments()[0]
            : type;

        var priorityCategory = _compareFunctions
            .OrderByDescending(x => x.Priority)
            .FirstOrDefault(x => x.Accepts(type));

        return priorityCategory?.CompareFunctions ?? Array.Empty<ICompareFunction>();
    }

    private sealed record FunctionCategory(IEnumerable<ICompareFunction> CompareFunctions, Func<Type, bool> Accepts, int Priority) {}
}
