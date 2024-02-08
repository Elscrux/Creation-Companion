using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Environment;

public interface IEditorEnvironmentResult;

public sealed record EditorEnvironmentResult<TMod, TModGetter>(
    GameEnvironmentBuilder<TMod, TModGetter> EnvironmentBuilder,
    TMod ActiveMod)
    : IEditorEnvironmentResult
    where TMod : class, TModGetter, IContextMod<TMod, TModGetter>
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>;
