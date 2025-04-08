using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References.Asset;
using CreationEditor.Services.Mutagen.References.Asset.Controller;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using DynamicData.Binding;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Noggog;
namespace CreationEditor.Avalonia.ViewModels.Reference;

public sealed class AssetReferenceVM : IReferenceVM, IDisposable {
    private readonly DisposableBucket _disposables = new();

    public IAssetLink Asset { get; }

    private readonly ILinkCacheProvider _linkCacheProvider;
    private readonly IAssetTypeService _assetTypeService;
    private readonly IAssetReferenceController _assetReferenceController;
    private readonly IRecordReferenceController _recordReferenceController;

    public string Name => Asset.DataRelativePath.Path;
    public string Identifier => string.Empty;
    public string Type => Asset.Type.BaseFolder;


    private ReadOnlyObservableCollection<IReferenceVM>? _children;
    private ObservableCollectionExtended<IReferenceVM>? _childrenCollection;
    public ReadOnlyObservableCollection<IReferenceVM> Children => _children ??= LoadChildren();

    private ReadOnlyObservableCollection<IReferenceVM> LoadChildren() {
        var references = ReferencedAsset.NifReferences
            .Select(path => new AssetReferenceVM(path, _linkCacheProvider, _assetTypeService, _assetReferenceController, _recordReferenceController))
            .Cast<IReferenceVM>()
            .Combine(ReferencedAsset.RecordReferences.Select(x => new RecordReferenceVM(x, _linkCacheProvider, _recordReferenceController)));

        _childrenCollection = new ObservableCollectionExtended<IReferenceVM>(references);

        ReferencedAsset.RecordReferences.ObserveCollectionChanges().Subscribe(RecordReferenceUpdate);
        ReferencedAsset.NifArchiveReferences.ObserveCollectionChanges().Subscribe(NifArchiveReferenceUpdate);
        ReferencedAsset.NifDirectoryReferences.ObserveCollectionChanges().Subscribe(NifDirectoryReferenceUpdate);

        return new ReadOnlyObservableCollection<IReferenceVM>(_childrenCollection);
    }

    private void RecordReferenceUpdate(EventPattern<NotifyCollectionChangedEventArgs> e) {
        if (_childrenCollection is null) return;

        if (e.EventArgs.Action == NotifyCollectionChangedAction.Reset) {
            _childrenCollection.Clear();

            foreach (var path in ReferencedAsset.NifArchiveReferences.Concat(ReferencedAsset.NifDirectoryReferences)) {
                var reference = GetAssetReference(path);
                if (reference is not null) {
                    _childrenCollection.Add(reference);
                }
            }
        } else {
            _childrenCollection.Apply(e.EventArgs.Transform<IFormLinkGetter, RecordReferenceVM>(GetRecordReference));
        }
    }

    private void NifArchiveReferenceUpdate(EventPattern<NotifyCollectionChangedEventArgs> e) {
        if (_childrenCollection is null) return;

        if (e.EventArgs.Action == NotifyCollectionChangedAction.Reset) {
            _childrenCollection.Clear();

            foreach (var path in ReferencedAsset.NifDirectoryReferences) {
                var reference = GetAssetReference(path);
                if (reference is not null) {
                    _childrenCollection.Add(reference);
                }
            }

            foreach (var formLink in ReferencedAsset.RecordReferences) {
                var reference = GetRecordReference(formLink);
                _childrenCollection.Add(reference);
            }
        } else {
            _childrenCollection.Apply(e.EventArgs.Transform<DataRelativePath, AssetReferenceVM>(GetAssetReference));
        }
    }

    private void NifDirectoryReferenceUpdate(EventPattern<NotifyCollectionChangedEventArgs> e) {
        if (_childrenCollection is null) return;

        if (e.EventArgs.Action == NotifyCollectionChangedAction.Reset) {
            _childrenCollection.Clear();

            foreach (var path in ReferencedAsset.NifArchiveReferences) {
                var reference = GetAssetReference(path);
                if (reference is not null) {
                    _childrenCollection.Add(reference);
                }
            }

            foreach (var formLink in ReferencedAsset.RecordReferences) {
                var reference = GetRecordReference(formLink);
                _childrenCollection.Add(reference);
            }
        } else {
            _childrenCollection.Apply(e.EventArgs.Transform<DataRelativePath, AssetReferenceVM>(GetAssetReference));
        }
    }

    private RecordReferenceVM GetRecordReference(IFormLinkIdentifier formLink) => new(formLink, _linkCacheProvider, _recordReferenceController);
    private AssetReferenceVM? GetAssetReference(DataRelativePath path) {
        var assetLink = _assetTypeService.GetAssetLink(path);
        if (assetLink is null) return null;

        return new AssetReferenceVM(assetLink, _linkCacheProvider, _assetTypeService, _assetReferenceController, _recordReferenceController);
    }

    [field: AllowNull, MaybeNull]
    public IReferencedAsset ReferencedAsset {
        get {
            if (field is null) {
                _assetReferenceController
                    .GetReferencedAsset(Asset, out var referencedAsset)
                    .DisposeWith(_disposables);

                field = referencedAsset;
            }
            return field;
        }
    }

    public bool HasChildren => _children is not null ? _children.Count > 0 : ReferencedAsset.HasReferences;

    public AssetReferenceVM(
        IAssetLink asset,
        ILinkCacheProvider linkCacheProvider,
        IAssetTypeService assetTypeService,
        IAssetReferenceController assetReferenceController,
        IRecordReferenceController recordReferenceController) {
        Asset = asset;
        _linkCacheProvider = linkCacheProvider;
        _assetTypeService = assetTypeService;
        _assetReferenceController = assetReferenceController;
        _recordReferenceController = recordReferenceController;
    }

    public AssetReferenceVM(
        DataRelativePath path,
        ILinkCacheProvider linkCacheProvider,
        IAssetTypeService assetTypeService,
        IAssetReferenceController assetReferenceController,
        IRecordReferenceController recordReferenceController) {
        _linkCacheProvider = linkCacheProvider;
        _assetTypeService = assetTypeService;
        _assetReferenceController = assetReferenceController;
        _recordReferenceController = recordReferenceController;

        var assetLink = _assetTypeService.GetAssetLink(path);

        Asset = assetLink ?? throw new ArgumentException($"AssetLink for {path} could not be retrieved");
    }

    public void Dispose() => _disposables.Dispose();
}
