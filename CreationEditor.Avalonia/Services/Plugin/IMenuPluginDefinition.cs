using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Input;
using CreationEditor.Avalonia.Models.Docking;
namespace CreationEditor.Avalonia.Services.Plugin;

public interface IMenuPluginDefinition : IVisualPluginDefinition {
    public KeyGesture? KeyGesture => null;
    DockMode DockMode { get; set; }
    Dock Dock { get; set; }
    public object? GetIcon();

    public static readonly FuncValueConverter<IMenuPluginDefinition, object?> ToIcon
        = new(plugin => plugin?.GetIcon());
}
