using CreationEditor.Services.Query.Select;
using CreationEditor.Services.Query.Where;
namespace CreationEditor.Skyrim.Tests.Query;

public sealed class QueryConditionTests {
    private sealed class TestQueryRecord {
        public string EditorID { get; set; } = string.Empty;
        public int Count { get; set; }
        public DateTime Unsupported { get; set; }
    }

    private sealed class TestCompareFunctionFactory : IQueryCompareFunctionFactory {
        public IEnumerable<IQueryCompareFunction> Get(Type type) {
            if (type == typeof(string)) {
                yield return new QueryCompareFunction<string, string>("Equals", (a, b) => a == b);
                yield return new QueryCompareFunction<string, string>("Contains", (a, b) => a.Contains(b, StringComparison.OrdinalIgnoreCase));
            } else if (type == typeof(int)) {
                yield return new QueryCompareFunction<int, int>("Equals", (a, b) => a == b);
            }
        }
    }

    private sealed class TestConditionFactory : IQueryConditionFactory {
        public IQueryCondition Create(Type? type = null) => new QueryCondition(this, new TestCompareFunctionFactory(), type);
    }

    private static QueryCondition CreateCondition(Type? recordType = null) => new(new TestConditionFactory(), new TestCompareFunctionFactory(), recordType);

    private static void SelectField(IQueryCondition condition, string fieldName, Type fieldType) {
        condition.FieldSelector.SelectedField = new ReflectionQueryField(fieldType, fieldName);
    }

    [Fact]
    public void Constructor_SelectsEditorIDFieldByDefault() {
        using var condition = CreateCondition(typeof(TestQueryRecord));

        Assert.Equal(nameof(TestQueryRecord.EditorID), condition.FieldSelector.SelectedField?.Name);
        Assert.NotNull(condition.SelectedCompareFunction);
        Assert.Equal("Equals", condition.SelectedCompareFunction.Operator);
    }

    [Fact]
    public void Evaluate_MatchingFieldValue_ReturnsTrue() {
        using var condition = CreateCondition(typeof(TestQueryRecord));
        condition.CompareValue = "skyrim";

        Assert.True(condition.Evaluate(new TestQueryRecord { EditorID = "skyrim" }));
        Assert.False(condition.Evaluate(new TestQueryRecord { EditorID = "other" }));
    }

    [Fact]
    public void Evaluate_NullFieldValue_ReturnsFalse() {
        using var condition = CreateCondition(typeof(TestQueryRecord));
        condition.CompareValue = "skyrim";

        Assert.False(condition.Evaluate(new TestQueryRecord { EditorID = null! }));
    }

    [Fact]
    public void Evaluate_NullObject_ReturnsFalse() {
        using var condition = CreateCondition(typeof(TestQueryRecord));
        condition.CompareValue = "skyrim";

        Assert.False(condition.Evaluate(null));
    }

    [Fact]
    public void Evaluate_NoCompareFunctionsAvailable_ReturnsFalse() {
        using var condition = CreateCondition(typeof(TestQueryRecord));
        SelectField(condition, nameof(TestQueryRecord.Unsupported), typeof(DateTime));
        condition.CompareValue = "anything";

        Assert.Null(condition.SelectedCompareFunction);
        Assert.False(condition.Evaluate(new TestQueryRecord()));
    }

    [Fact]
    public void Evaluate_ChangingField_ReloadsCompareFunctions() {
        using var condition = CreateCondition(typeof(TestQueryRecord));

        SelectField(condition, nameof(TestQueryRecord.Count), typeof(int));

        Assert.Equal(nameof(TestQueryRecord.Count), condition.FieldSelector.SelectedField?.Name);
        Assert.Equal("Equals", condition.SelectedCompareFunction?.Operator);

        condition.CompareValue = 5;
        Assert.True(condition.Evaluate(new TestQueryRecord { Count = 5 }));
        Assert.False(condition.Evaluate(new TestQueryRecord { Count = 3 }));
    }

    [Fact]
    public void EvaluateConditions_Negate_InvertsResult() {
        using var condition = CreateCondition(typeof(TestQueryRecord));
        condition.CompareValue = "skyrim";
        condition.Negate = true;

        var conditions = new[] { condition };
        Assert.False(conditions.EvaluateConditions(new TestQueryRecord { EditorID = "skyrim" }));
        Assert.True(conditions.EvaluateConditions(new TestQueryRecord { EditorID = "other" }));
    }

    [Fact]
    public void EvaluateConditions_MultipleConditions_AndByDefault() {
        using var idCondition = CreateCondition(typeof(TestQueryRecord));
        idCondition.CompareValue = "skyrim";

        using var countCondition = CreateCondition(typeof(TestQueryRecord));
        SelectField(countCondition, nameof(TestQueryRecord.Count), typeof(int));
        countCondition.CompareValue = 5;

        var conditions = new[] { idCondition, countCondition };

        Assert.True(conditions.EvaluateConditions(new TestQueryRecord { EditorID = "skyrim", Count = 5 }));
        Assert.False(conditions.EvaluateConditions(new TestQueryRecord { EditorID = "skyrim", Count = 3 }));
        Assert.False(conditions.EvaluateConditions(new TestQueryRecord { EditorID = "other", Count = 5 }));
    }

    [Fact]
    public void EvaluateConditions_OrCondition_PassesIfEitherMatches() {
        using var idCondition = CreateCondition(typeof(TestQueryRecord));
        idCondition.CompareValue = "skyrim";
        idCondition.IsOr = true;

        using var countCondition = CreateCondition(typeof(TestQueryRecord));
        SelectField(countCondition, nameof(TestQueryRecord.Count), typeof(int));
        countCondition.CompareValue = 5;
        countCondition.IsOr = true;

        var conditions = new[] { idCondition, countCondition };

        Assert.True(conditions.EvaluateConditions(new TestQueryRecord { EditorID = "skyrim", Count = 3 }));
        Assert.True(conditions.EvaluateConditions(new TestQueryRecord { EditorID = "other", Count = 5 }));
        Assert.False(conditions.EvaluateConditions(new TestQueryRecord { EditorID = "other", Count = 3 }));
    }

    [Fact]
    public void Memento_RoundTrip_PreservesEvaluation() {
        using var condition = CreateCondition(typeof(TestQueryRecord));
        condition.CompareValue = "skyrim";
        condition.Negate = true;
        condition.IsOr = true;

        var memento = condition.CreateMemento();

        using var restored = CreateCondition(typeof(TestQueryRecord));
        restored.RestoreMemento(memento);

        Assert.Equal("skyrim", restored.CompareValue);
        Assert.True(restored.Negate);
        Assert.True(restored.IsOr);
        Assert.Equal("Equals", restored.SelectedCompareFunction?.Operator);
        Assert.Equal(nameof(TestQueryRecord.EditorID), restored.FieldSelector.SelectedField?.Name);

        var conditions = new[] { restored };
        Assert.False(conditions.EvaluateConditions(new TestQueryRecord { EditorID = "skyrim" }));
        Assert.True(conditions.EvaluateConditions(new TestQueryRecord { EditorID = "other" }));
    }

    [Fact]
    public void Summary_ContainsFieldOperatorAndValue() {
        using var condition = CreateCondition(typeof(TestQueryRecord));
        condition.CompareValue = "skyrim";

        var summaries = new List<string>();
        using var subscription = condition.Summary.Subscribe(summaries.Add);

        Assert.True(
            SpinWait.SpinUntil(
                () => summaries.Any(s => s.Contains("EditorID") && s.Contains("Equals") && s.Contains("skyrim")),
                TimeSpan.FromSeconds(5)),
            $"Summary did not emit expected value. Got: [{string.Join(" | ", summaries)}]");
    }
}
