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
    : IReferenceUpdateTrigger<IDataSource, DataSourceLink, TCache, TLink, DataRelativePath, TSubscriber>
    where TCache : IReferenceCache<TCache, TLink, DataRelativePath>
    where TLink : notnull
    where TSubscriber : IReferenced {
    private readonly CompositeDisposable _dataSourceUpdatedDisposables = new();
    public DataRelativePath ToReference(DataSourceLink element) => element.DataRelativePath;
    public IDataSource GetSourceFor(DataSourceLink reference) => reference.DataSource;
    public void SetupSubscriptions(
        ReferenceController<IDataSource, DataSourceLink, TCache, TLink, DataRelativePath, TSubscriber> referenceController,
        CompositeDisposable disposables) {

        dataSourceService.DataSourcesChanged
            .ObserveOnTaskpool()
            .Subscribe(dataSources => {
                _dataSourceUpdatedDisposables.Clear();

                Task.Run(async () => {
                        await referenceController.UpdateSources(dataSources);

                        logger.Here().Information("Loaded all {Name} References for {Count} Data Sources",
                            referenceController.Name,
                            dataSources.Count);
                    })
                    .FireAndForget(e => logger.Here().Error(e,
                        "Failed to load {Name} References for {Count} Data Sources {Exception}",
                        referenceController.Name,
                        dataSources.Count,
                        e.Message));

                foreach (var dataSource in dataSources.OfType<FileSystemDataSource>()) {
                    var watcher = dataSourceWatcherProvider.GetWatcher(dataSource);
                    watcher.CreatedFile
                        .Subscribe(referenceController.RegisterCreation)
                        .DisposeWith(_dataSourceUpdatedDisposables);

                    watcher.DeletedFile
                        .Subscribe(referenceController.RegisterDeletion)
                        .DisposeWith(_dataSourceUpdatedDisposables);

                    watcher.RenamedFile
                        .Subscribe(x => {
                            referenceController.RegisterDeletion(x.Old);
                            referenceController.RegisterCreation(x.New);
                        })
                        .DisposeWith(_dataSourceUpdatedDisposables);

                    watcher.Changed
                        .Subscribe(link => {
                            var registerUpdate = referenceController.RegisterUpdate(link);
                            registerUpdate(link);
                        })
                        .DisposeWith(_dataSourceUpdatedDisposables);

                }
            })
            .DisposeWith(disposables);
    }
}