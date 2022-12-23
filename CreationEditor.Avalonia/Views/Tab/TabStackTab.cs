using Avalonia.Controls;
namespace CreationEditor.Avalonia.Views.Tab;

public class TabStackTab {
    public required string Header { get; init; }
    public required Control Control { get; init; }
    public bool IsActive { get; set; } = false;
    public float Size { get; set; } = 20;
}
