using Avalonia.Data.Converters;
using Avalonia.Input;
using CreationEditor.Services.Plugin;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Services.Plugin;

public interface IMenuPluginDefinition : IVisualPluginDefinition {
    public KeyGesture? KeyGesture { get; }
    public object? GetIcon();

    public static readonly FuncValueConverter<IMenuPluginDefinition, object?> ToIcon
        = new(plugin => plugin?.GetIcon());
}

public interface IMenuPlugin<TMod, TModGetter> : IMenuPluginDefinition, IPlugin<TMod, TModGetter>
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    where TMod : class, TModGetter, IContextMod<TMod, TModGetter> {}
