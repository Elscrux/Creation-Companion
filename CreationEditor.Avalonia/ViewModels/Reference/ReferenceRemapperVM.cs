using System.Collections;
using System.Reactive;
using System.Reactive.Subjects;
using Avalonia.Threading;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Reference;

public sealed partial class ReferenceRemapperVM : ViewModel {
    public IDataSourceService DataSourceService { get; }
    public ILinkCacheProvider LinkCacheProvider { get; }
    public object? Context { get; }

    public DataSourceFileLink? DataSourceLink { get; }
    public IAssetType? AssetType { get; }

    public IReferencedRecord? ReferencedRecordContext { get; }

    public bool ContextCanBeRemapped { get; }
    public Type? ContextType { get; }
    public IList<Type>? ScopedTypes { get; }

    [Reactive] public partial bool IsRemapping { get; set; }
    public Subject<Unit> ShowReferenceRemapDialog { get; } = new();

    public ReactiveCommand<FormKey, Unit> RemapRecordReferences { get; }
    public ReactiveCommand<DataSourceFileLink, Unit> RemapAssetReferences { get; }

    public ReferenceRemapperVM(
        IDataSourceService dataSourceService,
        ILinkCacheProvider linkCacheProvider,
        IAssetController assetController,
        IAssetTypeService assetTypeService,
        IRecordController recordController,
        object? context = null) {
        DataSourceService = dataSourceService;
        LinkCacheProvider = linkCacheProvider;
        Context = context;

        DataSourceLink = ParseAssetContext(context);
        if (DataSourceLink is not null) {
            ContextCanBeRemapped = true;
            var assetLink = assetTypeService.GetAssetLink(DataSourceLink.DataRelativePath);
            if (assetLink is not null) {
                AssetType = assetLink.AssetTypeInstance;
            }
        }

        var referencedRecord = ParseRecordContext(context);
        if (referencedRecord is not null) {
            ReferencedRecordContext = referencedRecord;
            ContextCanBeRemapped = true;
            ContextType = referencedRecord.Record.Registration.GetterType;
            ScopedTypes = ContextType.AsEnumerable().ToArray();
        }

        RemapRecordReferences = ReactiveCommand.Create<FormKey>(formKey => {
            if (ContextType is null || ReferencedRecordContext is null) return;
            if (!linkCacheProvider.LinkCache.TryResolve(formKey, ContextType, out var record)) return;

            IsRemapping = true;
            Task.Run(() => {
                recordController.RemapReferences(ReferencedRecordContext, record);
                Dispatcher.UIThread.Post(() => IsRemapping = false);
            });
        });

        RemapAssetReferences = ReactiveCommand.Create<DataSourceFileLink>(dataSourceLink => {
            if (DataSourceLink is null || AssetType is null) return;

            IsRemapping = true;
            Task.Run(() => {
                assetController.RemapFileReferences(DataSourceLink, dataSourceLink);
                Dispatcher.UIThread.Post(() => IsRemapping = false);
            });
        });
    }

    private static DataSourceFileLink? ParseAssetContext(object? context) {
        return context switch {
            DataSourceFileLink dataSourceLink => dataSourceLink,
            _ => null,
        };
    }

    private static IReferencedRecord? ParseRecordContext(object? context) {
        return context switch {
            IReferencedRecord referencedRecord => referencedRecord,
            IEnumerable enumerable => enumerable.OfType<IReferencedRecord>().FirstOrDefault(),
            _ => null,
        };
    }

    public void Remap() {
        if (ContextCanBeRemapped) {
            ShowReferenceRemapDialog.OnNext(Unit.Default);
        }
    }
}
