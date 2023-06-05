using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References.Asset;
using CreationEditor.Services.Mutagen.References.Asset.Controller;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Noggog;
namespace CreationEditor.Avalonia.Models.Reference;

public sealed class AssetReference : IReference, IDisposable {
    private readonly IDisposableBucket _disposables = new DisposableBucket();

    public IAssetLink Asset { get; }

    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IAssetTypeService _assetTypeService;
    private readonly IAssetReferenceController _assetReferenceController;
    private readonly IRecordReferenceController _recordReferenceController;

    public string Name => Asset.DataRelativePath;
    public string Identifier => string.Empty;
    public string Type => Asset.Type.BaseFolder;

    private ReadOnlyObservableCollection<IReference>? _children;
    public ReadOnlyObservableCollection<IReference> Children => _children ??= LoadChildren();
    private ReadOnlyObservableCollection<IReference> LoadChildren() {
        RecordReference? GetRecordReference(IFormLinkGetter formLink) => new(formLink, _editorEnvironment, _recordReferenceController);
        AssetReference? GetAssetReference(string path) {
            var assetLink = _assetTypeService.GetAssetLink(path);
            if (assetLink == null) return null;

            return new AssetReference(assetLink, _editorEnvironment, _assetTypeService, _assetReferenceController, _recordReferenceController);
        }

        var references = ReferencedAsset.NifReferences
            .Select(path => new AssetReference(path, _editorEnvironment, _assetTypeService, _assetReferenceController, _recordReferenceController))
            .Cast<IReference>()
            .Combine(ReferencedAsset.RecordReferences.Select(x => new RecordReference(x, _editorEnvironment, _recordReferenceController)));

        var collection = new ObservableCollectionExtended<IReference>(references);

        ReferencedAsset.RecordReferences.ObserveCollectionChanges()
            .Subscribe(e => {
                if (e.EventArgs.Action == NotifyCollectionChangedAction.Reset) {
                    collection.Clear();

                    foreach (var path in ReferencedAsset.NifArchiveReferences.Concat(ReferencedAsset.NifDirectoryReferences)) {
                        var reference = GetAssetReference(path);
                        if (reference != null) {
                            collection.Add(reference);
                        }
                    }
                } else {
                    collection.Apply(e.EventArgs.Transform<IFormLinkGetter, RecordReference>(GetRecordReference));
                }
            });

        ReferencedAsset.NifArchiveReferences.ObserveCollectionChanges()
            .Subscribe(e => {
                if (e.EventArgs.Action == NotifyCollectionChangedAction.Reset) {
                    collection.Clear();

                    foreach (var path in ReferencedAsset.NifDirectoryReferences) {
                        var reference = GetAssetReference(path);
                        if (reference != null) {
                            collection.Add(reference);
                        }
                    }

                    foreach (var formLink in ReferencedAsset.RecordReferences) {
                        var reference = GetRecordReference(formLink);
                        if (reference != null) {
                            collection.Add(reference);
                        }
                    }
                } else {
                    collection.Apply(e.EventArgs.Transform<string, AssetReference>(GetAssetReference));
                }
            });

        ReferencedAsset.NifDirectoryReferences.ObserveCollectionChanges()
            .Subscribe(e => {
                if (e.EventArgs.Action == NotifyCollectionChangedAction.Reset) {
                    collection.Clear();

                    foreach (var path in ReferencedAsset.NifArchiveReferences) {
                        var reference = GetAssetReference(path);
                        if (reference != null) {
                            collection.Add(reference);
                        }
                    }

                    foreach (var formLink in ReferencedAsset.RecordReferences) {
                        var reference = GetRecordReference(formLink);
                        if (reference != null) {
                            collection.Add(reference);
                        }
                    }
                } else {
                    collection.Apply(e.EventArgs.Transform<string, AssetReference>(GetAssetReference));
                }
            });

        return new ReadOnlyObservableCollection<IReference>(collection);
    }

    private IReferencedAsset? _referencedAsset;
    public IReferencedAsset ReferencedAsset {
        get {
            if (_referencedAsset == null) {
                _assetReferenceController
                    .GetReferencedAsset(Asset, out var referencedAsset)
                    .DisposeWith(_disposables);

                _referencedAsset = referencedAsset;
            }
            return _referencedAsset;
        }
    }

    public bool HasChildren => _children is not null ? _children.Count > 0 : ReferencedAsset.HasReferences;

    public AssetReference(
        IAssetLink asset,
        IEditorEnvironment editorEnvironment,
        IAssetTypeService assetTypeService,
        IAssetReferenceController assetReferenceController,
        IRecordReferenceController recordReferenceController) {
        Asset = asset;
        _editorEnvironment = editorEnvironment;
        _assetTypeService = assetTypeService;
        _assetReferenceController = assetReferenceController;
        _recordReferenceController = recordReferenceController;
    }

    public AssetReference(
        string path,
        IEditorEnvironment editorEnvironment,
        IAssetTypeService assetTypeService,
        IAssetReferenceController assetReferenceController,
        IRecordReferenceController recordReferenceController) {
        _editorEnvironment = editorEnvironment;
        _assetTypeService = assetTypeService;
        _assetReferenceController = assetReferenceController;
        _recordReferenceController = recordReferenceController;

        var assetLink = _assetTypeService.GetAssetLink(path);

        Asset = assetLink ?? throw new Exception($"AssetLink for {path} could not be retrieved");
    }

    public void Dispose() => _disposables.Dispose();
}
