using CreationEditor.Services.Asset;
using CreationEditor.Skyrim.Tests.AutoData;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Skyrim.Assets;
namespace CreationEditor.Skyrim.Tests.Asset;

public sealed class AssetTypeServiceTests {
    [Theory] [CreationEditorAutoData]
    public void GetAssetType_ResolvesByExtension(AssetTypeService assetTypeService) {
        Assert.Equal(SkyrimTextureAssetType.Instance, assetTypeService.GetAssetType(new DataRelativePath("textures/clutter/test.dds")));
        Assert.Equal(SkyrimTextureAssetType.Instance, assetTypeService.GetAssetType(new DataRelativePath("textures/test.png")));
        Assert.Equal(SkyrimModelAssetType.Instance, assetTypeService.GetAssetType(new DataRelativePath("meshes/clutter/test.nif")));
        Assert.Equal(SkyrimDeformedModelAssetType.Instance, assetTypeService.GetAssetType(new DataRelativePath("meshes/test.tri")));
        Assert.Equal(SkyrimBodyTextureAssetType.Instance, assetTypeService.GetAssetType(new DataRelativePath("meshes/test.egt")));
        Assert.Equal(SkyrimBehaviorAssetType.Instance, assetTypeService.GetAssetType(new DataRelativePath("meshes/test.hkx")));
        Assert.Equal(SkyrimScriptSourceAssetType.Instance, assetTypeService.GetAssetType(new DataRelativePath("scripts/source/test.psc")));
        Assert.Equal(SkyrimScriptCompiledAssetType.Instance, assetTypeService.GetAssetType(new DataRelativePath("scripts/test.pex")));
        Assert.Equal(SkyrimInterfaceAssetType.Instance, assetTypeService.GetAssetType(new DataRelativePath("interface/test.swf")));
        Assert.Equal(SkyrimInterfaceAssetType.Instance, assetTypeService.GetAssetType(new DataRelativePath("interface/test.gfx")));
        Assert.Equal(SkyrimTranslationAssetType.Instance, assetTypeService.GetAssetType(new DataRelativePath("strings/skyrim_english.strings")));
        Assert.Equal(SkyrimTranslationAssetType.Instance, assetTypeService.GetAssetType(new DataRelativePath("strings/skyrim_english.dlstrings")));
        Assert.Equal(SkyrimSeqAssetType.Instance, assetTypeService.GetAssetType(new DataRelativePath("seq/test.seq")));
    }

    [Theory] [CreationEditorAutoData]
    public void GetAssetType_MusicFolder_WavAndXwmAreMusic(AssetTypeService assetTypeService) {
        // .wav and .xwm inside the music folder are music, not sound
        Assert.Equal(SkyrimMusicAssetType.Instance, assetTypeService.GetAssetType(new DataRelativePath(@"music\test.wav")));
        Assert.Equal(SkyrimMusicAssetType.Instance, assetTypeService.GetAssetType(new DataRelativePath(@"music\battle\test.xwm")));
    }

    [Theory] [CreationEditorAutoData]
    public void GetAssetType_SoundFolder_WavIsSound(AssetTypeService assetTypeService) {
        Assert.Equal(SkyrimSoundAssetType.Instance, assetTypeService.GetAssetType(new DataRelativePath("sound/fx/test.wav")));
        Assert.Equal(SkyrimSoundAssetType.Instance, assetTypeService.GetAssetType(new DataRelativePath("sound/test.fuz")));
        Assert.Equal(SkyrimSoundAssetType.Instance, assetTypeService.GetAssetType(new DataRelativePath("test.xwm")));
    }

    [Theory] [CreationEditorAutoData]
    public void GetAssetType_ScriptsSourceFolder_ReturnsNull(AssetTypeService assetTypeService) {
        // Paths containing "source\scripts" are ignored (temporary adjustment for mod authoring folders)
        Assert.Null(assetTypeService.GetAssetType(new DataRelativePath(@"source\scripts\test.psc")));
    }

    [Theory] [CreationEditorAutoData]
    public void GetAssetType_UnknownOrMissingExtension_ReturnsNull(AssetTypeService assetTypeService) {
        Assert.Null(assetTypeService.GetAssetType(new DataRelativePath("textures/test.xyz")));
        Assert.Null(assetTypeService.GetAssetType(new DataRelativePath("textures/test")));
        Assert.Null(assetTypeService.GetAssetType(new DataRelativePath("textures/")));
    }

    [Theory] [CreationEditorAutoData]
    public void GetAssetLink_CreatesTypedAssetLink(AssetTypeService assetTypeService) {
        var link = assetTypeService.GetAssetLink(new DataRelativePath("textures/clutter/test.dds"));

        Assert.NotNull(link);
        Assert.Equal(SkyrimTextureAssetType.Instance, link.AssetTypeInstance);
        Assert.Equal(Path.Combine("textures", "clutter", "test.dds"), link.DataRelativePath.Path);
    }

    [Theory] [CreationEditorAutoData]
    public void GetAssetLink_UnknownType_ReturnsNull(AssetTypeService assetTypeService) {
        Assert.Null(assetTypeService.GetAssetLink(new DataRelativePath("textures/test.xyz")));
    }

    [Theory] [CreationEditorAutoData]
    public void GetAssetLink_WithExplicitType_UsesGivenType(AssetTypeService assetTypeService) {
        var link = assetTypeService.GetAssetLink(new DataRelativePath("meshes/test.nif"), SkyrimModelAssetType.Instance);

        Assert.NotNull(link);
        Assert.Equal(SkyrimModelAssetType.Instance, link.AssetTypeInstance);
    }

    [Theory] [CreationEditorAutoData]
    public void IdentifierRoundTrip_MapsBetweenTypeAndIdentifier(AssetTypeService assetTypeService) {
        Assert.Equal("TXR", assetTypeService.GetAssetTypeIdentifier(SkyrimTextureAssetType.Instance));
        Assert.Equal("MDL", assetTypeService.GetAssetTypeIdentifier(SkyrimModelAssetType.Instance));
        Assert.Equal(SkyrimTextureAssetType.Instance, assetTypeService.GetAssetTypeFromIdentifier("TXR"));
        Assert.Equal(SkyrimModelAssetType.Instance, assetTypeService.GetAssetTypeFromIdentifier("MDL"));
    }

    [Theory] [CreationEditorAutoData]
    public void FileExtensions_ContainsKnownExtensions(AssetTypeService assetTypeService) {
        Assert.Contains(".dds", assetTypeService.FileExtensions);
        Assert.Contains(".nif", assetTypeService.FileExtensions);
        Assert.Contains(".psc", assetTypeService.FileExtensions);
        Assert.Contains(".pex", assetTypeService.FileExtensions);
        Assert.DoesNotContain(".xyz", assetTypeService.FileExtensions);
    }
}
