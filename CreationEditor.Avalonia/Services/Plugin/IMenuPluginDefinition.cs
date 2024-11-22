using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Input;
using CreationEditor.Avalonia.Models.Docking;
namespace CreationEditor.Avalonia.Services.Plugin;

public interface IMenuPluginDefinition : IVisualPluginDefinition {
    KeyGesture? KeyGesture => null;
    DockMode DockMode { get; set; }
    Dock Dock { get; set; }
    object? GetIcon();

    static readonly FuncValueConverter<IMenuPluginDefinition, object?> ToIcon
        = new(plugin => plugin?.GetIcon());
}
