using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Mutagen.References;

public interface IReferencedAsset : IReferenced {
    IAssetLinkGetter AssetLink { get; }
}
