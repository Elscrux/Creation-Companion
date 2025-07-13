using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References;
using DynamicData.Binding;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Noggog;
namespace CreationEditor.Avalonia.ViewModels.Reference;

public sealed class AssetReferenceVM : IReferenceVM, IDisposable {
    private readonly DisposableBucket _disposables = new();

    public IAssetLinkGetter Asset { get; }

    private readonly ILinkCacheProvider _linkCacheProvider;
    private readonly IAssetTypeService _assetTypeService;
    private readonly IReferenceService _referenceService;

    public string Name => Asset.DataRelativePath.Path;
    public string Identifier => string.Empty;
    public string Type => Asset.Type.BaseFolder;

    public bool HasChildren => _children is not null ? _children.Count > 0 : ReferencedAsset.HasReferences;

    private ReadOnlyObservableCollection<IReferenceVM>? _children;
    private ObservableCollectionExtended<IReferenceVM>? _childrenCollection;
    public ReadOnlyObservableCollection<IReferenceVM> Children => _children ??= LoadChildren();

    public AssetReferenceVM(
        IAssetLinkGetter asset,
        ILinkCacheProvider linkCacheProvider,
        IAssetTypeService assetTypeService,
        IReferenceService referenceService) {
        Asset = asset;
        _linkCacheProvider = linkCacheProvider;
        _assetTypeService = assetTypeService;
        _referenceService = referenceService;
    }

    public AssetReferenceVM(
        DataRelativePath path,
        ILinkCacheProvider linkCacheProvider,
        IAssetTypeService assetTypeService,
        IReferenceService referenceService) {
        _linkCacheProvider = linkCacheProvider;
        _assetTypeService = assetTypeService;
        _referenceService = referenceService;

        var assetLink = _assetTypeService.GetAssetLink(path);

        Asset = assetLink ?? throw new ArgumentException($"AssetLink for {path} could not be retrieved");
    }

    private ReadOnlyObservableCollection<IReferenceVM> LoadChildren() {
        var references = ReferencedAsset.AssetReferences
            .Select(path => new AssetReferenceVM(path, _linkCacheProvider, _assetTypeService, _referenceService))
            .Cast<IReferenceVM>()
            .Combine(ReferencedAsset.RecordReferences.Select(x => new RecordReferenceVM(x, _linkCacheProvider, _referenceService)));

        _childrenCollection = new ObservableCollectionExtended<IReferenceVM>(references);

        ReferencedAsset.RecordReferences.ObserveCollectionChanges().Subscribe(RecordReferenceUpdate);
        ReferencedAsset.AssetReferences.ObserveCollectionChanges().Subscribe(AssetReferenceUpdate);

        return new ReadOnlyObservableCollection<IReferenceVM>(_childrenCollection);
    }

    private void RecordReferenceUpdate(EventPattern<NotifyCollectionChangedEventArgs> e) {
        if (_childrenCollection is null) return;

        if (e.EventArgs.Action == NotifyCollectionChangedAction.Reset) {
            _childrenCollection.Clear();

            foreach (var path in ReferencedAsset.AssetReferences) {
                var reference = GetAssetReference(path);
                if (reference is not null) {
                    _childrenCollection.Add(reference);
                }
            }
        } else {
            _childrenCollection.Apply(e.EventArgs.Transform<IFormLinkGetter, RecordReferenceVM>(GetRecordReference));
        }
    }

    private void AssetReferenceUpdate(EventPattern<NotifyCollectionChangedEventArgs> e) {
        if (_childrenCollection is null) return;

        if (e.EventArgs.Action == NotifyCollectionChangedAction.Reset) {
            _childrenCollection.Clear();

            foreach (var path in ReferencedAsset.AssetReferences) {
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

    private RecordReferenceVM GetRecordReference(IFormLinkIdentifier formLink) => new(formLink, _linkCacheProvider, _referenceService);
    private AssetReferenceVM? GetAssetReference(DataRelativePath path) {
        var assetLink = _assetTypeService.GetAssetLink(path);
        if (assetLink is null) return null;

        return new AssetReferenceVM(assetLink, _linkCacheProvider, _assetTypeService, _referenceService);
    }

    [field: AllowNull, MaybeNull]
    public IReferencedAsset ReferencedAsset {
        get {
            if (field is null) {
                _referenceService
                    .GetReferencedAsset(Asset, out var referencedAsset)
                    .DisposeWith(_disposables);

                field = referencedAsset;
            }
            return field;
        }
    }

    public void Dispose() => _disposables.Dispose();
}
