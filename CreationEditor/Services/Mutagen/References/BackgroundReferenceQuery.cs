using System.IO.Abstractions;
using CreationEditor.Services.Background;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Mod;
using CreationEditor.Services.Mutagen.Type;
using CreationEditor.Services.Notification;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Serilog;
namespace CreationEditor.Services.Mutagen.References;

public sealed class BackgroundReferenceQuery : IReferenceQuery {
    private readonly IBackgroundTaskManager _backgroundTaskManager;
    private readonly ReferenceQuery _referenceQuery;

    public BackgroundReferenceQuery(
        IEnvironmentContext environmentContext,
        IFileSystem fileSystem,
        IMutagenTypeProvider mutagenTypeProvider,
        INotificationService notificationService,
        IModInfoProvider<IModGetter> modInfoProvider,
        ILogger logger,
        IBackgroundTaskManager backgroundTaskManager) {
        _backgroundTaskManager = backgroundTaskManager;

        _referenceQuery = new ReferenceQuery(environmentContext, fileSystem, mutagenTypeProvider, notificationService, modInfoProvider, logger);
    }

    public void LoadModReferences(IModGetter mod) => _referenceQuery.LoadModReferences(mod);
    public void LoadModReferences(IReadOnlyList<IModGetter> mods) => _referenceQuery.LoadModReferences(mods);
    public void LoadModReferences(ILinkCache linkCache) => _referenceQuery.LoadModReferences(linkCache);
    public void LoadModReferences(IGameEnvironment environment) => _referenceQuery.LoadModReferences(environment);

    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey) => _backgroundTaskManager.ReferencesLoaded ? _referenceQuery.GetReferences(formKey) : new HashSet<IFormLinkIdentifier>();
    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey, IModGetter mod) => _backgroundTaskManager.ReferencesLoaded ? _referenceQuery.GetReferences(formKey, mod) : new HashSet<IFormLinkIdentifier>();
    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey, IReadOnlyList<IModGetter> mods) => _backgroundTaskManager.ReferencesLoaded ? _referenceQuery.GetReferences(formKey, mods) : new HashSet<IFormLinkIdentifier>();
    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey, ILinkCache linkCache) => _backgroundTaskManager.ReferencesLoaded ? _referenceQuery.GetReferences(formKey, linkCache) : new HashSet<IFormLinkIdentifier>();
    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey, IGameEnvironment environment) => _backgroundTaskManager.ReferencesLoaded ? _referenceQuery.GetReferences(formKey, environment) : new HashSet<IFormLinkIdentifier>();
}
