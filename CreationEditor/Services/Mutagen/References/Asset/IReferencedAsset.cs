using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset;

public interface IReferencedAsset {
    ModKey ModKey { get; }
    IAssetLink AssetLink { get; }
    IObservableCollection<IFormLinkGetter> RecordReferences { get; }
    IEnumerable<string> NifReferences { get; }
    internal IObservableCollection<string> NifDirectoryReferences { get; }
    internal IObservableCollection<string> NifArchiveReferences { get; }

    bool HasReferences { get; }
    IObservable<int> ReferenceCount { get; }
}
