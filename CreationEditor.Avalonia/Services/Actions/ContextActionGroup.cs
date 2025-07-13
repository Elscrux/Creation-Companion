namespace CreationEditor.Avalonia.Services.Actions;

public sealed record ContextActionGroup(string Keyword, int Priority) {
    public static ContextActionGroup Misc { get; } = new("Misc", 25);
    public static ContextActionGroup Linking { get; } = new("Linking", 50);
    public static ContextActionGroup Inspection { get; } = new("Inspecting", 75);
    public static ContextActionGroup Modification { get; } = new("Modification", 100);
    public static ContextActionGroup Viewing { get; } = new("Viewing", 125);
}
