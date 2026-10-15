namespace CreationEditor.Services.Config.Options;

/// <summary>Non-generic access to a <see cref="ListConfigOption{TDefinition}" /> for editors.</summary>
public interface IListConfigOption : IConfigOption {
    /// <summary>The item type edited by this list</summary>
    Type ItemType { get; }

    /// <summary>Creates a typed list from the given items, e.g. a <c>List&lt;TItem&gt;</c></summary>
    object CreateItemList(IEnumerable<object> items);
}

/// <summary>
/// Config option holding an ordered list of items. Each item is a
/// <see cref="ConfiguredValues{TDefinition}" /> pairing the item's config definition with its
/// configured values; editors edit the values through the definition's options.
/// </summary>
public sealed class ListConfigOption<TDefinition>(
    string name,
    string? description = null,
    bool isOptional = false,
    Func<IReadOnlyList<ConfiguredValues<TDefinition>>, ConfigOptionValidationError?>? validate = null)
    : IListConfigOption, IConfigOption<IReadOnlyList<ConfiguredValues<TDefinition>>>
    where TDefinition : IConfigDefinition, new() {
    public string Name { get; } = name;
    public string? Description { get; } = description;
    public bool IsOptional { get; } = isOptional;
    public IReadOnlyList<ConfiguredValues<TDefinition>> DefaultValue { get; } = [];
    public object DefaultObjectValue => DefaultValue;
    public Type ValueType => typeof(List<ConfiguredValues<TDefinition>>);
    public Func<IReadOnlyList<ConfiguredValues<TDefinition>>, ConfigOptionValidationError?>? Validate { get; } = validate;

    public Type ItemType => typeof(ConfiguredValues<TDefinition>);

    public object CreateItemList(IEnumerable<object> items) => items.Cast<ConfiguredValues<TDefinition>>().ToList();
}
