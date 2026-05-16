using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using HKX2;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Parser;

public sealed class BehaviorAnimationLinkParser(IAssetTypeService assetTypeService, ILogger logger) : IFileParser<IAssetLinkGetter> {
    public string Name => "Behavior Animation Links";
    public IAssetType AssetType => assetTypeService.Provider.Behavior;

    public IEnumerable<IAssetLinkGetter> ParseFile(string actualFilePath, DataSourceFileLink fileLink) {
        using var rs = File.OpenRead(actualFilePath);
        var br = new BinaryReaderEx(rs);
        var des = new PackFileDeserializer();

        Dictionary<uint, IHavokObject> objects;
        try {
            des.Deserialize(br, out objects);
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to parse Behavior file {FilePath}: {Exception}", actualFilePath, e);
            yield break;
        }

        var directory = Path.GetDirectoryName(fileLink.DataRelativePath.Path);
        if (directory is null) {
            logger.Here().Error("Failed to get the directory for Behavior file {FilePath}", actualFilePath);
            yield break;
        }

        var parentDirectory = Path.GetDirectoryName(directory);
        if (parentDirectory is null) {
            logger.Here().Error("Failed to get parent directory for Behavior file {FilePath}", actualFilePath);
            yield break;
        }

        foreach (var havokObject in objects.Values) {
            switch (havokObject) {
                case hkbCharacterStringData hkbCharacterStringData:
                    if (GetAssetLink(hkbCharacterStringData.m_rigName, parentDirectory) is {} rigLink) yield return rigLink;
                    if (GetAssetLink(hkbCharacterStringData.m_behaviorFilename, parentDirectory) is {} behaviorLink) yield return behaviorLink;

                    foreach (var animationName in hkbCharacterStringData.m_animationNames) {
                        if (GetAssetLink(animationName, parentDirectory) is {} animationLink) yield return animationLink;
                    }

                    foreach (var animationName in hkbCharacterStringData.m_animationFilenames) {
                        if (GetAssetLink(animationName, parentDirectory) is {} animationLink) yield return animationLink;
                    }
                    break;
                case hkbClipGenerator hkbClipGenerator:
                    if (GetAssetLink(hkbClipGenerator.m_animationName, parentDirectory) is {} animLink) yield return animLink;

                    break;
                case hkbBehaviorReferenceGenerator hkbBehaviorReferenceGenerator:
                    if (GetAssetLink(hkbBehaviorReferenceGenerator.m_behaviorName, parentDirectory) is {} behLink) yield return behLink;

                    break;
                case hkbBehaviorInfoIdToNamePair hkbBehaviorInfoIdToNamePair:
                    if (GetAssetLink(hkbBehaviorInfoIdToNamePair.m_behaviorName, parentDirectory) is {} behLink2) yield return behLink2;

                    break;
                case hkbAuxiliaryNodeInfo hkbAuxiliaryNodeInfo:
                    if (GetAssetLink(hkbAuxiliaryNodeInfo.m_referenceBehaviorName, parentDirectory) is {} behLink3) yield return behLink3;

                    break;
                case hkbProjectStringData hkbProjectStringData:
                    foreach (var characterFilename in hkbProjectStringData.m_characterFilenames) {
                        Console.WriteLine("charfilename: " + characterFilename);
                        if (characterFilename.Contains("Default", StringComparison.OrdinalIgnoreCase) || characterFilename.Contains("FirstPerson", StringComparison.OrdinalIgnoreCase)) {
                            Console.WriteLine("characterfilename: " + characterFilename);
                        }
                        if (GetAssetLink(characterFilename, directory) is {} link) yield return link;
                    }

                    foreach (var behaviorFilename in hkbProjectStringData.m_behaviorFilenames) {
                        if (GetAssetLink(behaviorFilename, parentDirectory) is {} link) yield return link;
                    }

                    foreach (var animFilename in hkbProjectStringData.m_animationFilenames) {
                        if (GetAssetLink(animFilename, parentDirectory) is {} link) yield return link;
                    }

                    break;
            }
        }

        IAssetLink? GetAssetLink(string path, string baseDirectory) {
            try {
                var dataRelativePath = fileLink.FileSystem.Path.Combine(baseDirectory, path);
                if (path.StartsWith('.')) {
                    var root = System.Environment.CurrentDirectory;
                    var combined = fileLink.FileSystem.Path.Combine(baseDirectory, path);
                    combined = fileLink.FileSystem.Path.GetFullPath(combined);
                    dataRelativePath = fileLink.FileSystem.Path.GetRelativePath(root, combined);
                }
                var assetLink = assetTypeService.GetAssetLink(dataRelativePath);
                if (assetLink is not null) {
                    return assetLink;
                }
            } catch (Exception e) {
                logger.Here().Error(e,
                    "Failed to get asset link for Behavior reference {ReferencePath} in file {FilePath}: {Exception}",
                    path,
                    fileLink.DataRelativePath.Path,
                    e);
                return null;
            }

            logger.Here().Warning("Failed to get asset link for Behavior reference {ReferencePath} in file {FilePath}", path, fileLink.DataRelativePath.Path);
            return null;
        }
    }
}
