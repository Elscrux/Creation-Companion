using CreationEditor.Services.Query.Select;
namespace CreationEditor.Skyrim.Tests.Query;

public sealed class QueryFieldSelectorCollectionTests {
    private sealed class TestChild {
        public string EditorID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    private sealed class TestRecord {
        public string EditorID { get; set; } = string.Empty;
        public TestChild Child { get; set; } = new();
    }

    [Fact]
    public void SetRootType_AddsSelectorSelectingEditorIDByDefault() {
        using var collection = new QueryFieldSelectorCollection();

        collection.SetRootType(typeof(TestRecord));

        var selector = Assert.Single(collection.Selectors);
        Assert.Equal(nameof(TestRecord.EditorID), selector.SelectedField?.Name);
    }

    [Fact]
    public void SetRootType_SameType_DoesNotAddAnotherSelector() {
        using var collection = new QueryFieldSelectorCollection();

        collection.SetRootType(typeof(TestRecord));
        collection.SetRootType(typeof(TestRecord));

        Assert.Single(collection.Selectors);
    }

    [Fact]
    public void GetValue_NavigatesNestedFields() {
        using var collection = new QueryFieldSelectorCollection();
        collection.SetRootType(typeof(TestRecord));
        collection.Selectors[0].SelectedField = new ReflectionQueryField(typeof(TestChild), nameof(TestRecord.Child));
        collection.AddSelector(typeof(TestChild));

        var value = collection.GetValue(new TestRecord { Child = new TestChild { EditorID = "childId" } });

        Assert.Equal("childId", value);
    }

    [Fact]
    public void GetValue_UnselectedLaterField_ReturnsIntermediateObject() {
        using var collection = new QueryFieldSelectorCollection();
        collection.SetRootType(typeof(TestRecord));
        collection.Selectors[0].SelectedField = new ReflectionQueryField(typeof(TestChild), nameof(TestRecord.Child));
        collection.AddSelector(typeof(TestChild));
        collection.Selectors[1].SelectedField = null;
        var child = new TestChild();

        var value = collection.GetValue(new TestRecord { Child = child });

        Assert.Same(child, value);
    }

    [Fact]
    public void GetValue_UnselectedFirstField_ReturnsNull() {
        using var collection = new QueryFieldSelectorCollection();
        collection.SetRootType(typeof(TestRecord));
        collection.Selectors[0].SelectedField = null;

        Assert.Null(collection.GetValue(new TestRecord()));
    }

    [Fact]
    public void GetValue_NullObject_ReturnsNull() {
        using var collection = new QueryFieldSelectorCollection();
        collection.SetRootType(typeof(TestRecord));

        Assert.Null(collection.GetValue(null));
    }

    [Fact]
    public void GetFieldName_JoinsSelectedFieldNames() {
        using var collection = new QueryFieldSelectorCollection();
        collection.SetRootType(typeof(TestRecord));
        collection.Selectors[0].SelectedField = new ReflectionQueryField(typeof(TestChild), nameof(TestRecord.Child));
        collection.AddSelector(typeof(TestChild));

        Assert.Equal("Child.EditorID", collection.GetFieldName());
    }

    [Fact]
    public void ChangingField_RemovesIncompatibleFollowingSelectors() {
        using var collection = new QueryFieldSelectorCollection();
        collection.SetRootType(typeof(TestRecord));
        collection.Selectors[0].SelectedField = new ReflectionQueryField(typeof(TestChild), nameof(TestRecord.Child));
        collection.AddSelector(typeof(TestChild));
        Assert.Equal(2, collection.Selectors.Count);

        // Selecting a string field makes the following TestChild selector incompatible
        collection.Selectors[0].SelectedField = new ReflectionQueryField(typeof(string), nameof(TestRecord.EditorID));

        Assert.Single(collection.Selectors);
    }

    [Fact]
    public void CanAddNextSelector_StringField_IsFalse() {
        using var collection = new QueryFieldSelectorCollection();
        bool? canAdd = null;
        using var subscription = collection.CanAddNextSelector.Subscribe(value => canAdd = value);

        collection.SetRootType(typeof(TestRecord));

        Assert.False(canAdd!.Value);
    }

    [Fact]
    public void CanAddNextSelector_NestedObjectField_IsTrue() {
        using var collection = new QueryFieldSelectorCollection();
        bool? canAdd = null;
        using var subscription = collection.CanAddNextSelector.Subscribe(value => canAdd = value);

        collection.SetRootType(typeof(TestRecord));
        collection.Selectors[0].SelectedField = new ReflectionQueryField(typeof(TestChild), nameof(TestRecord.Child));

        Assert.True(canAdd!.Value);
    }

    [Fact]
    public void Memento_RoundTrip_PreservesSelection() {
        using var collection = new QueryFieldSelectorCollection();
        collection.SetRootType(typeof(TestRecord));
        collection.Selectors[0].SelectedField = new ReflectionQueryField(typeof(TestChild), nameof(TestRecord.Child));
        collection.AddSelector(typeof(TestChild));

        var memento = collection.CreateMemento();

        using var restored = new QueryFieldSelectorCollection();
        restored.RestoreMemento(memento);

        Assert.Equal(2, restored.Selectors.Count);
        Assert.Equal("Child.EditorID", restored.GetFieldName());
        Assert.Equal("childId", restored.GetValue(new TestRecord { Child = new TestChild { EditorID = "childId" } }));
    }
}
