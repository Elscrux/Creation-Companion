using CreationEditor.Services.Environment;
using CreationEditor.Services.Query.Where;
using CreationEditor.Skyrim.Tests.AutoData;
using Mutagen.Bethesda.Plugins;
using Noggog;
namespace CreationEditor.Skyrim.Tests.Query;

public sealed class QueryCompareFunctionFactoryTests {
    [Theory] [CreationEditorAutoData]
    public void Get_StringType_ReturnsStringFunctions(ILinkCacheProvider linkCacheProvider) {
        var factory = new QueryCompareFunctionFactory(linkCacheProvider);

        var operators = factory.Get(typeof(string)).Select(f => f.Operator).ToList();

        Assert.Equal(["Equals", "Contains", "Starts With", "Ends With", "Matches RegEx"], operators);
    }

    [Theory] [CreationEditorAutoData]
    public void Get_NumericType_ReturnsNumericFunctions(ILinkCacheProvider linkCacheProvider) {
        var factory = new QueryCompareFunctionFactory(linkCacheProvider);

        var operators = factory.Get(typeof(int)).Select(f => f.Operator).ToList();

        Assert.Equal(["Equals", "Smaller Than", "Smaller or Equal", "Greater Than", "Greater or Equal"], operators);
    }

    [Theory] [CreationEditorAutoData]
    public void Get_NullableType_UnwrapsToUnderlyingTypeFunctions(ILinkCacheProvider linkCacheProvider) {
        var factory = new QueryCompareFunctionFactory(linkCacheProvider);

        var operators = factory.Get(typeof(int?)).Select(f => f.Operator).ToList();

        Assert.Equal(["Equals", "Smaller Than", "Smaller or Equal", "Greater Than", "Greater or Equal"], operators);
    }

    [Theory] [CreationEditorAutoData]
    public void Get_BoolType_ReturnsEqualsOnly(ILinkCacheProvider linkCacheProvider) {
        var factory = new QueryCompareFunctionFactory(linkCacheProvider);

        var operators = factory.Get(typeof(bool)).Select(f => f.Operator).ToList();

        Assert.Equal(["Equals"], operators);
    }

    [Theory] [CreationEditorAutoData]
    public void Get_FormKeyType_ReturnsEqualsOnly(ILinkCacheProvider linkCacheProvider) {
        var factory = new QueryCompareFunctionFactory(linkCacheProvider);

        var operators = factory.Get(typeof(FormKey)).Select(f => f.Operator).ToList();

        Assert.Equal(["Equals"], operators);
    }

    [Theory] [CreationEditorAutoData]
    public void Get_SimpleListOfValueType_ReturnsListFunctions(ILinkCacheProvider linkCacheProvider) {
        var factory = new QueryCompareFunctionFactory(linkCacheProvider);

        var operators = factory.Get(typeof(List<int>)).Select(f => f.Operator).ToList();

        Assert.Equal(["One Matches", "All Match", "Count Equals", "Count Less Than", "Count Greater Than"], operators);
    }

    [Theory] [CreationEditorAutoData]
    public void Get_DictionaryType_ReturnsDictionaryFunctions(ILinkCacheProvider linkCacheProvider) {
        var factory = new QueryCompareFunctionFactory(linkCacheProvider);

        var operators = factory.Get(typeof(Dictionary<string, int>)).Select(f => f.Operator).ToList();

        Assert.Equal(["One Matches", "All Matches", "Count Equals", "Count Less Than", "Count Greater Than"], operators);
    }

    [Theory] [CreationEditorAutoData]
    public void Get_ObjectType_ReturnsMatchingFunction(ILinkCacheProvider linkCacheProvider) {
        var factory = new QueryCompareFunctionFactory(linkCacheProvider);

        var operators = factory.Get(typeof(object)).Select(f => f.Operator).ToList();

        Assert.Equal(["Matching"], operators);
    }

    [Theory] [CreationEditorAutoData]
    public void Get_FlagsEnum_ReturnsHasFlagFirst(ILinkCacheProvider linkCacheProvider) {
        var factory = new QueryCompareFunctionFactory(linkCacheProvider);

        var operators = factory.Get(typeof(Tests)).Select(f => f.Operator).ToList();

        Assert.Equal("Has Flag", operators[0]);
    }

    [Theory] [CreationEditorAutoData]
    public void Get_RegularEnum_ReturnsEquals(ILinkCacheProvider linkCacheProvider) {
        var factory = new QueryCompareFunctionFactory(linkCacheProvider);

        var operators = factory.Get(typeof(Test)).Select(f => f.Operator).ToList();

        Assert.Equal(["Equals"], operators);
    }

    [Theory] [CreationEditorAutoData]
    public void Get_MemorySliceType_ReturnsNoFunctions(ILinkCacheProvider linkCacheProvider) {
        var factory = new QueryCompareFunctionFactory(linkCacheProvider);

        var functions = factory.Get(typeof(ReadOnlyMemorySlice<byte>)).ToList();

        Assert.Empty(functions);
    }

    private enum Test {
        A,
        B,
    }

    [Flags]
    private enum Tests {
        None = 0,
        A = 1,
        B = 2,
    }
}
