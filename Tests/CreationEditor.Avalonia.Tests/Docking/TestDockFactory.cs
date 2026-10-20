using Avalonia.Controls;
using Avalonia.Input;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Docking;
namespace CreationEditor.Avalonia.Tests.Docking;

/// <summary>
/// Minimal dockable definition for tests, identified by name.
/// </summary>
public sealed class TestDockable(string identifier) : IDockableDefinition {
    public string Type => identifier;
    public string Header => identifier;
    public object? Icon => null;
    public string Category => "View";
    public KeyGesture? HotKey => null;
    public double? Size => null;
    public bool CanClose => true;
    public DockMode DockMode => DockMode.Side;
    public Dock Dock => Dock.Left;
    public GridLength GridSize => new(1, GridUnitType.Star);

    public Task<Control?> CreateControl(object? parameter = null) => Task.FromResult<Control?>(new Grid { Name = identifier });
}

public sealed class TestDockableRegistry(params IDockableDefinition[] definitions) : IDockableRegistry {
    public IReadOnlyList<IDockableDefinition> Definitions { get; } = definitions;

    public IDockableDefinition? GetByType(string type) {
        return Definitions.FirstOrDefault(definition => definition.Type == type);
    }
}
