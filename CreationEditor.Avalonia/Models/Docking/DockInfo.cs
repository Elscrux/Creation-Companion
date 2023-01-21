using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Models.Docking;

public sealed class DockInfo {
    public string Header { get; init; } = string.Empty;
    public IconSource? IconSource { get; init; } = null;
    
    public double? Size { get; init; }
    
    public bool CanClose { get; init; } = true;
}
