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

public sealed class QueryCompareFunctionFactory : IQueryCompareFunctionFactory {
    private readonly List<QueryFunctionCategory> _compareFunctions = [];

    private static bool CheckSubConditions(QueryConditionState conditionState, object? item) =>
        conditionState.SubConditions.EvaluateConditions(item);

    private static bool DefaultAccepts<T>(Type type) => type.InheritsFrom(typeof(T));
    private static bool SimpleListAccepts<T>(Type type) {
        if (!DefaultAccepts<T>(type) || !type.IsGenericType || !type.InheritsFrom(typeof(IEnumerable))) return false;

        var genericArguments = type.GetGenericArguments();
        if (genericArguments.Length > 1) return false;

        var genericArgument = genericArguments[0];
        return IQueryFieldInformation.IsValueType(genericArgument);
    }
    private static bool DictionaryAccepts(Type type) {
        var genericArguments = type.GetGenericArguments();
        return genericArguments.Length == 2 && type.InheritsFrom(typeof(IEnumerable));
    }

    public QueryCompareFunctionFactory(
        ILinkCacheProvider linkCacheProvider) {

        // Simple List
        Func<Type, IQueryFieldInformation> simpleListFieldOverride = type => {
            var listType = type.GetGenericArguments()[0];
            return new ValueQueryFieldInformation(listType, listType);
        };

        RegisterCompareFunction([
                new QueryCompareFunction<IEnumerable, object>(
                    "One Matches",
                    (context, enumerable) => enumerable.Cast<object?>().Any(item => Equals(item, context.CompareValue)),
                    simpleListFieldOverride),
                new QueryCompareFunction<IEnumerable, object>(
                    "All Match",
                    (context, enumerable) => enumerable.Cast<object?>().All(item => Equals(item, context.CompareValue)),
                    simpleListFieldOverride)
            ],
            SimpleListAccepts<IEnumerable>,
            -50);

        // Dictionary
        Func<Type, IQueryFieldInformation> dictionaryFieldOverride = type => {
            var keyValueType = typeof(IKeyValue<,>).MakeGenericType(type.GetGenericArguments());
            return new CollectionQueryFieldInformation(typeof(IEnumerable), keyValueType);
        };
        RegisterCompareFunction([
                new QueryCompareFunction<IEnumerable, object>(
                    "One Matches",
                    (context, enumerable) => enumerable.Cast<object?>().Any(o => CheckSubConditions(context, o)),
                    dictionaryFieldOverride),
                new QueryCompareFunction<IEnumerable, object>(
                    "All Matches",
                    (context, enumerable) => enumerable.Any() && enumerable.Cast<object?>().All(o => CheckSubConditions(context, o)),
                    dictionaryFieldOverride),
                new QueryCompareFunction<IEnumerable, int>(
                    "Count Equals",
                    (enumerable, count) => enumerable.CountIsExactly(count)),
                new QueryCompareFunction<IEnumerable, int>(
                    "Count Less Than",
                    (enumerable, count) => enumerable.CountIsLessThan(count)),
                new QueryCompareFunction<IEnumerable, int>(
                    "Count Greater Than",
                    (enumerable, count) => enumerable.CountIsGreaterThan(count))
            ],
            DictionaryAccepts, -10);

        // List
        RegisterCompareFunction<IEnumerable>([
                new QueryCompareFunction<IEnumerable, object>(
                    "One Matches",
                    (context, enumerable) => enumerable.Cast<object?>().Any(o => CheckSubConditions(context, o)),
                    QueryFieldCategory.Collection),
                new QueryCompareFunction<IEnumerable, object>(
                    "All Matches",
                    (context, enumerable) => enumerable.Any() && enumerable.Cast<object?>().All(o => CheckSubConditions(context, o)),
                    QueryFieldCategory.Collection),
                new QueryCompareFunction<IEnumerable, int>(
                    "Count Equals",
                    (enumerable, count) => enumerable.CountIsExactly(count)),
                new QueryCompareFunction<IEnumerable, int>(
                    "Count Less Than",
                    (enumerable, count) => enumerable.CountIsLessThan(count)),
                new QueryCompareFunction<IEnumerable, int>(
                    "Count Greater Than",
                    (enumerable, count) => enumerable.CountIsGreaterThan(count))
            ],
            -100);

        // Translated String
        RegisterCompareFunction<ITranslatedStringGetter>(
            GetStringFunctions<ITranslatedStringGetter>(
                (stringFunction, translatedString, text) => {
                    if (translatedString.String is not null && stringFunction(translatedString.String, text)) return true;

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
        RegisterCompareFunction<IFormLinkGetter>([
            new QueryCompareFunction<IFormLinkGetter, IFormLinkGetter>(
                "Equals",
                (formLink, compareFormLink) => FormLinkIdentifierEqualityComparer.Instance.Equals(formLink, compareFormLink)),
            new QueryCompareFunction<IFormLinkGetter, object>(
                "Matching",
                (context, formLink) =>
                    linkCacheProvider.LinkCache.TryResolve(formLink, out var record)
                 && context.SubConditions.EvaluateConditions(record),
                QueryFieldCategory.Collection),
        ]);

        // Form Key
        RegisterCompareFunction([GetSimpleEquals<FormKey>()]);

        // Color
        RegisterCompareFunction([GetSimpleEquals<Color>()]);

        // Enum
        RegisterCompareFunction([
                new QueryCompareFunction<Enum, Enum>(
                    "Equals",
                    (e, flag) => e.HasFlag(flag))
            ],
            type => type.IsEnum && type.GetCustomAttribute<FlagsAttribute>() is not null,
            10);

        RegisterCompareFunction([
            new QueryCompareFunction<Enum, Enum>(
                "Equals",
                (e, other) => e.Equals(other))
        ]);

        // Bool
        RegisterCompareFunction([GetSimpleEquals<bool>()]);

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
        RegisterCompareFunction([new QueryCompareFunction<object, object>("Matching", CheckSubConditions, QueryFieldCategory.Collection)],
            int.MinValue);

        // Pre-sort
        _compareFunctions.Sort((a, b) => b.Priority.CompareTo(a.Priority));
    }

    public static IEnumerable<IQueryCompareFunction> GetStringFunctions<T>(
        Func<Func<string, string, bool>, T, string, bool> translationFunction) {
        yield return new QueryCompareFunction<T, string>("Equals", (a, b)
            => translationFunction(StringComparer.OrdinalIgnoreCase.Equals, a, b));
        yield return new QueryCompareFunction<T, string>("Contains", (a, b)
            => translationFunction((x, y) => x.Contains(y, StringComparison.OrdinalIgnoreCase), a, b));
        yield return new QueryCompareFunction<T, string>("Starts With", (a, b)
            => translationFunction((x, y) => x.StartsWith(y, StringComparison.OrdinalIgnoreCase), a, b));
        yield return new QueryCompareFunction<T, string>("Ends With", (a, b)
            => translationFunction((x, y) => x.EndsWith(y, StringComparison.OrdinalIgnoreCase), a, b));
        yield return new QueryCompareFunction<T, string>("Matches RegEx", (a, b)
            => translationFunction((x, y) => {
                try {
                    return Regex.IsMatch(x, y);
                } catch {
                    return false;
                }
            }, a, b));
    }

    public static IEnumerable<QueryCompareFunction<T, T>> GetNumericFunctions<T>()
        where T : IComparisonOperators<T, T, bool> {
        yield return new QueryCompareFunction<T, T>("Equals", (x, y) => x == y);
        yield return new QueryCompareFunction<T, T>("Smaller Than", (x, y) => x < y);
        yield return new QueryCompareFunction<T, T>("Smaller or Equal", (x, y) => x <= y);
        yield return new QueryCompareFunction<T, T>("Greater Than", (x, y) => x > y);
        yield return new QueryCompareFunction<T, T>("Greater or Equal", (x, y) => x >= y);
    }

    public static QueryCompareFunction<T, T> GetSimpleEquals<T>() {
        return new QueryCompareFunction<T, T>(
            "Equals",
            (a, b) => Equals(a, b));
    }

    public void RegisterCompareFunction<T>(IEnumerable<IQueryCompareFunction> compareFunctions, int priority = 0) {
        _compareFunctions.Add(new QueryFunctionCategory(compareFunctions, DefaultAccepts<T>, priority));
    }

    public void RegisterCompareFunction<T>(IEnumerable<QueryCompareFunction<T, T>> compareFunctions, int priority = 0) {
        _compareFunctions.Add(new QueryFunctionCategory(compareFunctions, DefaultAccepts<T>, priority));
    }

    public void RegisterCompareFunction(IEnumerable<IQueryCompareFunction> compareFunctions, Func<Type, bool> accepts, int priority = 0) {
        _compareFunctions.Add(new QueryFunctionCategory(compareFunctions, accepts, priority));
    }

    public IEnumerable<IQueryCompareFunction> Get(Type type) {
        if (type.InheritsFrom(typeof(ReadOnlyMemorySlice<>))) return [];

        type = type.InheritsFrom(typeof(Nullable<>))
            ? type.GetGenericArguments()[0]
            : type;

        var priorityCategory = _compareFunctions
            .OrderByDescending(x => x.Priority)
            .FirstOrDefault(x => x.Accepts(type));

        return priorityCategory?.CompareFunctions ?? [];
    }

    private sealed record QueryFunctionCategory(IEnumerable<IQueryCompareFunction> CompareFunctions, Func<Type, bool> Accepts, int Priority);
}
