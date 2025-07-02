namespace LeveledList.Model.List;

public record ListTypeDefinition(
    string Type,
    List<TierIdentifier> Tiers,
    Dictionary<ListDefinitionIdentifier, ListDefinition> Lists,
    Dictionary<string, Item>? Items = null
) {
    public ListTypeDefinition() : this(string.Empty, [], []) {}
}
