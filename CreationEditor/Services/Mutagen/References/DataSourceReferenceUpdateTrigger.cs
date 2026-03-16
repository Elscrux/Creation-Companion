using System.Reactive.Disposables;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Cache;
using Mutagen.Bethesda.Assets;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Mutagen.References;

public sealed class DataSourceReferenceUpdateTrigger<TCache, TLink, TSubscriber>(
    ILogger logger,
    IDataSourceService dataSourceService,
    IDataSourceWatcherProvider dataSourceWatcherProvider)
    : IDisposable, IReferenceUpdateTrigger<IDataSource, DataSourceFileLink, TCache, TLink, DataRelativePath, TSubscriber>
    where TCache : IReferenceCache<TCache, TLink, DataRelativePath>
    where TLink : notnull
    where TSubscriber : IReferenced {
    private CancellationTokenSource _tokenSource = new();
    private readonly CompositeDisposable _dataSourceUpdatedDisposables = new();
    public DataRelativePath ToReference(DataSourceFileLink element) => element.DataRelativePath;
    public IDataSource GetSourceFor(DataSourceFileLink reference) => reference.DataSource;
    public void SetupSubscriptions(
        ReferenceController<IDataSource, DataSourceFileLink, TCache, TLink, DataRelativePath, TSubscriber> referenceController,
        CompositeDisposable disposables) {

        dataSourceService.DataSourcesChanged
            .ObserveOnTaskpool()
            .Subscribe(dataSources => {
                _tokenSource.Cancel();

                _dataSourceUpdatedDisposables.Clear();

                Task.Run(async () => {
                        _tokenSource = new CancellationTokenSource();
                        await referenceController.UpdateSources(dataSources, _tokenSource.Token);

                        logger.Here().Information("Loaded all {Name} References for {Count} Data Sources",
                            referenceController.Name,
                            dataSources.Count);
                    })
                    .FireAndForget(e => logger.Here().Error(e,
                        "Failed to load {Name} References for {Count} Data Sources {Exception}",
                        referenceController.Name,
                        dataSources.Count,
                        e));

                foreach (var dataSource in dataSources.OfType<FileSystemDataSource>()) {
                    var watcher = dataSourceWatcherProvider.GetWatcher(dataSource);
                    watcher.CreatedFile
                        .Subscribe(referenceController.RegisterCreation)
                        .DisposeWithComposite(_dataSourceUpdatedDisposables);

                    watcher.DeletedFile
                        .Subscribe(referenceController.RegisterDeletion)
                        .DisposeWithComposite(_dataSourceUpdatedDisposables);

                    watcher.RenamedFile
                        .Subscribe(x => {
                            referenceController.RegisterDeletion(x.Old);
                            referenceController.RegisterCreation(x.New);
                        })
                        .DisposeWithComposite(_dataSourceUpdatedDisposables);

                    watcher.ChangedFile
                        .Subscribe(link => {
                            var registerUpdate = referenceController.RegisterUpdate(link);
                            registerUpdate(link);
                        })
                        .DisposeWithComposite(_dataSourceUpdatedDisposables);

                }
            })
            .DisposeWithComposite(disposables);
    }

    public void Dispose() => _tokenSource.Dispose();
}