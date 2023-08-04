using Autofac;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Plugin;

public sealed record PluginContext<TMod, TModGetter>(
    Version EditorVersion,
    IEditorEnvironment<TMod, TModGetter> EditorEnvironment,
    ILifetimeScope LifetimeScope)
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    where TMod : class, TModGetter, IContextMod<TMod, TModGetter>;
