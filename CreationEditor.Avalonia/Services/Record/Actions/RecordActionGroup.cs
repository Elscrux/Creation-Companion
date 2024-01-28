namespace CreationEditor.Avalonia.Services.Record.Actions;

public sealed record RecordActionGroup(string Keyword, int Priority) {
    public static RecordActionGroup Inspection { get; } = new("Inspecting", 75);
    public static RecordActionGroup Modification { get; } = new("Modification", 100);
    public static RecordActionGroup Viewing { get; } = new("Viewing", 125);
}
