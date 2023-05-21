namespace CreationEditor.Services.Asset; 

public static class AssetCompare {
    public const StringComparison PathComparison = StringComparison.OrdinalIgnoreCase;
    public static readonly StringComparer PathComparer = StringComparer.OrdinalIgnoreCase;
}
