namespace LeveledList.Model.List;

public sealed record ExtendedListDefinition(
    string FileName,
    ListTypeDefinition TypeDefinition,
    ListDefinitionIdentifier Name,
    ListDefinition ListDefinition);
