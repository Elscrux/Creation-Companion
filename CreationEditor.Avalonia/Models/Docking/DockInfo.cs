using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Models.Docking;

public sealed class DockInfo {
    public string Header { get; init; } = string.Empty;
    public IconSource? IconSource { get; init; } = null;
    
    public bool CanClose { get; init; } = true;
}
