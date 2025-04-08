using CreationEditor.Services.Mutagen.References.Record;
using DynamicData.Binding;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset;

public interface IReferencedAsset : IReferenced {
    ModKey ModKey { get; }
    IAssetLinkGetter AssetLink { get; }
    IObservableCollection<DataRelativePath> NifDirectoryReferences { get; }
    IObservableCollection<DataRelativePath> NifArchiveReferences { get; }

    bool HasReferences { get; }
    IObservable<int> ReferenceCount { get; }
}
