using Avalonia.Controls;
using CreationEditor.Services.Plugin;
namespace CreationEditor.Avalonia.Services.Plugin;

public interface IVisualPluginDefinition : IPluginDefinition {
    Control GetControl();
}