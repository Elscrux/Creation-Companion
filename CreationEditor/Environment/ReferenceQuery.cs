using System.IO.Abstractions;
using Elscrux.Notification;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using MutagenLibrary.Core.Plugins;
using MutagenLibrary.References.ReferenceCache;
using Serilog;
namespace CreationEditor.Environment; 

public class BackgroundReferenceQuery : IReferenceQuery {
    private readonly IBackgroundTaskManager _backgroundTaskManager;
    private readonly ReferenceQuery _referenceQuery;

    public BackgroundReferenceQuery(
        ISimpleEnvironmentContext simpleEnvironmentContext,
        IFileSystem fileSystem,
        INotifier notifier,
        ILogger logger,
        IBackgroundTaskManager backgroundTaskManager) {
        _backgroundTaskManager = backgroundTaskManager;

        _referenceQuery = new ReferenceQuery(simpleEnvironmentContext, fileSystem, notifier, logger);
    }
    
    public void LoadModReferences(IModGetter mod) => _referenceQuery.LoadModReferences(mod);
    public void LoadModReferences(IReadOnlyList<IModGetter> mods) => _referenceQuery.LoadModReferences(mods);
    public void LoadModReferences(ILinkCache linkCache) => _referenceQuery.LoadModReferences(linkCache);
    public void LoadModReferences(IGameEnvironment environment) => _referenceQuery.LoadModReferences(environment);
    
    public HashSet<IFormLinkIdentifier> GetReferences(FormKey formKey) => _backgroundTaskManager.ReferencesLoaded ? _referenceQuery.GetReferences(formKey) : new HashSet<IFormLinkIdentifier>();
    public HashSet<IFormLinkIdentifier> GetReferences(FormKey formKey, IModGetter mod) => _backgroundTaskManager.ReferencesLoaded ? _referenceQuery.GetReferences(formKey, mod) : new HashSet<IFormLinkIdentifier>();
    public HashSet<IFormLinkIdentifier> GetReferences(FormKey formKey, IReadOnlyList<IModGetter> mods) => _backgroundTaskManager.ReferencesLoaded ? _referenceQuery.GetReferences(formKey, mods) : new HashSet<IFormLinkIdentifier>();
    public HashSet<IFormLinkIdentifier> GetReferences(FormKey formKey, ILinkCache linkCache) => _backgroundTaskManager.ReferencesLoaded ? _referenceQuery.GetReferences(formKey, linkCache) : new HashSet<IFormLinkIdentifier>();
    public HashSet<IFormLinkIdentifier> GetReferences(FormKey formKey, IGameEnvironment environment) => _backgroundTaskManager.ReferencesLoaded ? _referenceQuery.GetReferences(formKey, environment) : new HashSet<IFormLinkIdentifier>();
}