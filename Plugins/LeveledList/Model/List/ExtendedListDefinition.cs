using CreationEditor.Avalonia.Services.Record.Prefix;
namespace LeveledList.Model.List;

public sealed record ExtendedListDefinition(
    string Path,
    string FileName,
    ListTypeDefinition TypeDefinition,
    ListDefinitionIdentifier Name,
    ListDefinition ListDefinition,
    IRecordPrefixService recordPrefixService) {
    public bool Matches(LeveledList leveledList) {
        var featureWildcards = ListDefinition.Name.GetFeatureWildcards().ToArray();
        if (leveledList.Features.Count != featureWildcards.Length) return false;
        if (leveledList.Features.Any(f => !featureWildcards.Contains(f.Wildcard.Identifier))) return false;
        if (!ListDefinition.Restricts(leveledList.Features)) return false;

        return leveledList.EditorID == recordPrefixService.Prefix + ListDefinition.GetFullName(leveledList.Features);
    }
}
