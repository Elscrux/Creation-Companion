using System.Collections;
using CreationEditor.Services.Query.Where;
namespace CreationEditor.Skyrim.Tests.Query;

public sealed class QueryCompareFunctionTests {
    private static QueryConditionState CreateState(object? compareValue) => new(null, null) { CompareValue = compareValue };

    [Fact]
    public void Operator_IsStored() {
        var function = new QueryCompareFunction<int, int>("Equals", (a, b) => a == b);
        Assert.Equal("Equals", function.Operator);
    }

    [Fact]
    public void Evaluate_ExactTypeMatch() {
        var function = new QueryCompareFunction<int, int>("Equals", (a, b) => a == b);

        Assert.True(function.Evaluate(CreateState(5), 5));
        Assert.False(function.Evaluate(CreateState(5), 3));
    }

    [Fact]
    public void Evaluate_FieldValueOfWrongType_ReturnsFalse() {
        var function = new QueryCompareFunction<int, int>("Equals", (a, b) => a == b);

        Assert.False(function.Evaluate(CreateState(5), "5"));
        Assert.False(function.Evaluate(CreateState(5), null!));
    }

    [Fact]
    public void Evaluate_ConvertibleCompareValue_IsConverted() {
        var function = new QueryCompareFunction<int, int>("Equals", (a, b) => a == b);

        // "5" is not an int, so it goes through Convert.ChangeType
        Assert.True(function.Evaluate(CreateState("5"), 5));
        Assert.False(function.Evaluate(CreateState("5"), 3));
    }

    [Fact]
    public void Evaluate_NullCompareValue_MatchesNullFieldOnly() {
        var function = new QueryCompareFunction<string, string>("Equals", (a, b) => a == b);

        Assert.False(function.Evaluate(CreateState(null), "value"));
    }

    [Fact]
    public void Evaluate_NumericWidening_IsConverted() {
        var function = new QueryCompareFunction<double, double>("Greater Than", (a, b) => a > b);

        // int compare value is converted to double
        Assert.True(function.Evaluate(CreateState(5), 5.5));
        Assert.False(function.Evaluate(CreateState(5), 4.5));
    }

    [Fact]
    public void Evaluate_StateAwareFunction_ReceivesConditionState() {
        var function = new QueryCompareFunction<string, object>(
            "Custom",
            (state, fieldValue) => ReferenceEquals(state.CompareValue, fieldValue),
            QueryFieldCategory.Value);

        var state = CreateState("value");
        Assert.True(function.Evaluate(state, "value"));
        Assert.False(function.Evaluate(state, "other"));
    }

    [Fact]
    public void GetField_ValueCategory_UsesCompareValueType() {
        var function = new QueryCompareFunction<int, int>("Equals", (a, b) => a == b);

        var field = function.GetField(typeof(double));

        var valueField = Assert.IsType<ValueQueryFieldInformation>(field);
        Assert.Equal(typeof(int), valueField.TypeClass);
        Assert.Equal(typeof(double), valueField.ActualType);
    }

    [Fact]
    public void GetField_CollectionCategory_UsesElementType() {
        var function = new QueryCompareFunction<IEnumerable, object>(
            "One Matches",
            (_, _) => true,
            QueryFieldCategory.Collection);

        var field = function.GetField(typeof(List<int>));

        var collectionField = Assert.IsType<CollectionQueryFieldInformation>(field);
        Assert.Equal(typeof(int), collectionField.TypeClass);
        Assert.Equal(typeof(int), collectionField.ElementType);
    }

    [Fact]
    public void GetField_CollectionCategory_NonGenericType_UsesTypeItself() {
        var function = new QueryCompareFunction<IEnumerable, object>(
            "One Matches",
            (_, _) => true,
            QueryFieldCategory.Collection);

        var field = function.GetField(typeof(int[]));

        var collectionField = Assert.IsType<CollectionQueryFieldInformation>(field);
        Assert.Equal(typeof(int[]), collectionField.TypeClass);
        Assert.Equal(typeof(int[]), collectionField.ElementType);
    }

    [Fact]
    public void GetField_CustomGetField_IsUsed() {
        static IQueryFieldInformation CustomField(Type _) => new ValueQueryFieldInformation(typeof(string), typeof(string));

        var function = new QueryCompareFunction<int, int>(
            "Equals",
            (state, _) => true,
            CustomField);

        var field = function.GetField(typeof(double));
        Assert.IsType<ValueQueryFieldInformation>(field);
        Assert.Equal(typeof(string), field.TypeClass);
    }

    [Fact]
    public void Constructor_InvalidCategory_Throws() {
        Assert.Throws<ArgumentOutOfRangeException>(() => new QueryCompareFunction<int, int>(
            "Equals",
            (state, _) => true,
            (QueryFieldCategory) 999));
    }
}
