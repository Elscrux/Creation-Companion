using LeveledList.Services;
using Mutagen.Bethesda.Plugins.Records;
namespace LeveledList.Model;

public record ListTypeDefinition(
    string Type,
    List<TierIdentifier> Tiers,
    Dictionary<ListDefinitionIdentifier, Model.ListDefinition> Lists,
    Dictionary<string, Item>? Items = null
) : ITierProvider {
    public ListTypeDefinition() : this(string.Empty, [], []) {}

    public TierIdentifier? GetTier(IMajorRecordGetter record) {
        var recordEditorId = record.EditorID;
        if (recordEditorId is null) return null;

        return Tiers.FirstOrDefault(tier => recordEditorId.Contains(tier, StringComparison.OrdinalIgnoreCase));
    }
}
