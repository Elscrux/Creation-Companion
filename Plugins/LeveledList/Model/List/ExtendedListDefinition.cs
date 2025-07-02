namespace LeveledList.Model.List;

public sealed record ExtendedListDefinition(
    ListTypeDefinition TypeDefinition,
    ListDefinitionIdentifier Name,
    ListDefinition ListDefinition);
