namespace LeveledList.Model.List;

public record ListTypeDefinition(
    ListRecordType Type,
    List<TierIdentifier> Tiers,
    Dictionary<ListDefinitionIdentifier, ListDefinition> Lists,
    Dictionary<string, Item>? Items = null
) {
    public ListTypeDefinition() : this(ListRecordType.Armor, [], []) {}
}
