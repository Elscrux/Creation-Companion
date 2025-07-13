using System.Reactive.Disposables;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References.Cache;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Mutagen.References;

public sealed class ModReferenceUpdateTrigger<TCache, TLink, TSubscriber>(
    ILogger logger,
    IEditorEnvironment editorEnvironment)
    : IReferenceUpdateTrigger<IModGetter, RecordModPair, TCache, TLink, IFormLinkIdentifier, TSubscriber>
    where TCache : IReferenceCache<TCache, TLink, IFormLinkIdentifier>
    where TLink : notnull
    where TSubscriber : IReferenced {
    public IFormLinkIdentifier ToReference(RecordModPair element) => element.Record;
    public IModGetter GetSourceFor(RecordModPair element) => element.Mod;
    public void SetupSubscriptions(
        ReferenceController<IModGetter, RecordModPair, TCache, TLink, IFormLinkIdentifier, TSubscriber> referenceController,
        CompositeDisposable disposables) {
        editorEnvironment.LoadOrderChanged
            .ObserveOnTaskpool()
            .Subscribe(_ => {
                Task.Run(async () => {
                        await referenceController.UpdateSources(editorEnvironment.LinkCache.ListedOrder);
                        logger.Here().Information("Loaded all {Name} References for {Count} Mods",
                            referenceController.Name,
                            editorEnvironment.LinkCache.ListedOrder.Count);
                    })
                    .FireAndForget(e => logger.Here().Error(e,
                        "Failed to load {Name} References {Exception}",
                        referenceController.Name,
                        e.Message));
            })
            .DisposeWith(disposables);
    }
}
