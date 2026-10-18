using Avalonia.Controls;
using Avalonia.Input;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Docking;
using CreationEditor.Services.Plugin;
namespace CreationEditor.Avalonia.Services.Plugin;

public interface IMenuPluginDefinition : IPluginDefinition, IDockableDefinition {
    string IDockableDefinition.Type => Name.Replace(" ", string.Empty);
    string IDockableDefinition.Header => Name;
    object? IDockableDefinition.Icon => null;
    string IDockableDefinition.Category => "Plugins";
    KeyGesture? IDockableDefinition.HotKey => null;
    double? IDockableDefinition.Size => null;
    bool IDockableDefinition.CanClose => true;
    DockMode IDockableDefinition.DockMode => DockMode.Layout;
    Dock IDockableDefinition.Dock => Dock.Left;
    GridLength IDockableDefinition.GridSize => new(1, GridUnitType.Star);
    Task<Control?> IDockableDefinition.CreateControl(object? parameter) => Task.FromResult<Control?>(GetControl());

    Control GetControl();
}
