using Avalonia.Data.Converters;
using Avalonia.Input;
namespace CreationEditor.Avalonia.Services.Plugin;

public interface IMenuPluginDefinition : IVisualPluginDefinition {
    public KeyGesture? KeyGesture { get; }
    public object? GetIcon();

    public static readonly FuncValueConverter<IMenuPluginDefinition, object?> ToIcon
        = new(plugin => plugin?.GetIcon());
}
