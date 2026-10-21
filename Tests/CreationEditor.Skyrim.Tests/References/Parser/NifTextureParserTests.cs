using System.IO.Abstractions;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Parser;
using CreationEditor.Skyrim.Services.Asset;
using Mutagen.Bethesda.Skyrim.Assets;
using Serilog.Core;
namespace CreationEditor.Skyrim.Tests.References.Parser;

public sealed class NifTextureParserTests {
    private static string TestAssetPath(string fileName) => Path.Combine(AppContext.BaseDirectory, "TestAssets", fileName);

    private static (NifTextureParser Parser, IFileSystem FileSystem, FileSystemDataSource DataSource) CreateParser() {
        // NIF files are read via the native nifly library, which bypasses System.IO.Abstractions.
        // The parser therefore has to run against the real file system.
        var fileSystem = new System.IO.Abstractions.FileSystem();
        var assetTypeService = new AssetTypeService(Logger.None, fileSystem, new SkyrimAssetTypeProvider());
        var parser = new NifTextureParser(Logger.None, assetTypeService);
        return (parser, fileSystem, new FileSystemDataSource(fileSystem, AppContext.BaseDirectory));
    }

    [Fact]
    public void ParseFileTextureStrings_ReturnsTextureSetTextures() {
        var (parser, _, dataSource) = CreateParser();
        var link = new DataSourceFileLink(dataSource, "TestAssets/testcube.nif");
        using var stream = link.ReadFileStream();

        var textures = parser.ParseFileTextureStrings(stream, link).ToList();

        Assert.Equal(2, textures.Count);
        // Raw strings come straight from the NIF binary and always use backslashes, regardless of OS
        Assert.Contains(@"textures\testcube.dds", textures);
        Assert.Contains(@"textures\testcube_n.dds", textures);
    }

    [Fact]
    public void ParseFile_ReturnsTextureAssetLinks() {
        var (parser, _, dataSource) = CreateParser();
        var link = new DataSourceFileLink(dataSource, "TestAssets/testcube.nif");
        using var stream = link.ReadFileStream();

        var results = parser.ParseFile(stream, link).ToList();

        Assert.Equal(2, results.Count);
        Assert.All(results, l => Assert.Equal(SkyrimTextureAssetType.Instance, l.Type));
        Assert.Contains(results, l => l.DataRelativePath.Path == Path.Combine("textures", "testcube.dds"));
        Assert.Contains(results, l => l.DataRelativePath.Path == Path.Combine("textures", "testcube_n.dds"));
    }

    [Fact]
    public void ParseFile_MissingFile_ReturnsNoLinks() {
        var (parser, _, dataSource) = CreateParser();
        var link = new DataSourceFileLink(dataSource, "TestAssets/missing.nif");

        var results = parser.ParseFile(link.ReadFileStream(), link).ToList();
        Assert.Empty(results);
    }

    [Fact]
    public void ParseFile_InvalidNif_ReturnsNoLinks() {
        var (parser, fileSystem, dataSource) = CreateParser();
        var invalidPath = TestAssetPath("invalid.nif");
        fileSystem.File.WriteAllText(invalidPath, "this is not a nif file");
        var link = new DataSourceFileLink(dataSource, "TestAssets/invalid.nif");

        try {
            using var stream = link.ReadFileStream();
            var results = parser.ParseFile(stream, link).ToList();

            Assert.Empty(results);
        } finally {
            fileSystem.File.Delete(invalidPath);
        }
    }

    [Fact]
    public void AssetType_IsModel() {
        var (parser, _, _) = CreateParser();

        Assert.Equal("Nif Textures", parser.Name);
        Assert.Equal(SkyrimModelAssetType.Instance, parser.AssetType);
    }
}
