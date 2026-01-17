using CreationEditor.Avalonia.Services.Record.Prefix;
namespace LeveledList.Model.List;

public sealed record ExtendedListDefinition(
    string Path,
    string FileName,
    ListTypeDefinition TypeDefinition,
    ListDefinitionIdentifier Name,
    ListDefinition ListDefinition,
    IRecordPrefixService RecordPrefixService) {
    public bool Matches(LeveledList leveledList, Dictionary<TierIdentifier, TierIdentifier> tierAliases) {
        var featureWildcards = ListDefinition.Name.GetFeatureWildcards().ToArray();
        if (leveledList.Features.Count != featureWildcards.Length) return false;
        if (leveledList.Features.Any(f => !featureWildcards.Contains(f.Wildcard.Identifier))) return false;
        if (!ListDefinition.Restricts(leveledList.Features, tierAliases)) return false;

        return leveledList.EditorID == RecordPrefixService.Prefix + ListDefinition.GetFullName(leveledList.Features);
    }
}
