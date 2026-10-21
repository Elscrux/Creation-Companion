using System.IO.Abstractions.TestingHelpers;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Parser;
using CreationEditor.Skyrim.Services.Asset;
using Mutagen.Bethesda.Skyrim.Assets;
using Noggog.Testing.IO;
using Serilog.Core;
namespace CreationEditor.Skyrim.Tests.References.Parser;

public sealed class ScriptFileParserTests {
    private static readonly string DataSourcePath = Path.Combine(PathingUtil.DrivePrefix, "TestData");

    private static (ScriptFileParser Parser, MockFileSystem FileSystem, FileSystemDataSource DataSource) CreateParser() {
        var fileSystem = new MockFileSystem();
        var assetTypeService = new AssetTypeService(Logger.None, fileSystem, new SkyrimAssetTypeProvider());
        var parser = new ScriptFileParser(assetTypeService, Logger.None);
        return (parser, fileSystem, new FileSystemDataSource(fileSystem, DataSourcePath));
    }

    private static DataSourceFileLink CreateScriptFile(MockFileSystem fileSystem, FileSystemDataSource dataSource, string relativePath, string content) {
        var link = new DataSourceFileLink(dataSource, relativePath);
        var directory = fileSystem.Path.GetDirectoryName(link.FullPath);
        if (!string.IsNullOrEmpty(directory)) fileSystem.Directory.CreateDirectory(directory);
        fileSystem.File.WriteAllText(link.FullPath, content);
        return link;
    }

    [Fact]
    public void ParseFile_WithExtends_ReturnsParentScriptLinks() {
        var (parser, fileSystem, dataSource) = CreateParser();
        var link = CreateScriptFile(fileSystem, dataSource, "scripts/source/myscript.psc", "ScriptName MyScript extends ObjectReference\n");

        using var stream = link.ReadFileStream();

        var results = parser.ParseFile(stream, link).ToList();

        Assert.Equal(2, results.Count);
        Assert.Contains(results, l => l.DataRelativePath.Path == Path.Combine("Scripts", "Source", "ObjectReference.psc"));
        Assert.Contains(results, l => l.DataRelativePath.Path == Path.Combine("Scripts", "ObjectReference.pex"));
    }

    [Fact]
    public void ParseFile_WithoutExtends_ReturnsNoLinks() {
        var (parser, fileSystem, dataSource) = CreateParser();
        var link = CreateScriptFile(fileSystem, dataSource, "scripts/source/myscript.psc", "ScriptName MyScript\n");

        using var stream = link.ReadFileStream();

        var results = parser.ParseFile(stream, link).ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void ParseFile_ScriptNameOnLaterLine_StillFindsParent() {
        var (parser, fileSystem, dataSource) = CreateParser();
        var link = CreateScriptFile(fileSystem, dataSource, "scripts/source/myscript.psc", "; a comment\n{ a block }\nScriptName MyScript extends Form\n");

        using var stream = link.ReadFileStream();

        var results = parser.ParseFile(stream, link).ToList();

        Assert.Equal(2, results.Count);
        Assert.Contains(results, l => l.DataRelativePath.Path == Path.Combine("Scripts", "Source", "Form.psc"));
    }

    [Fact]
    public void ParseFile_IsCaseInsensitive() {
        var (parser, fileSystem, dataSource) = CreateParser();
        var link = CreateScriptFile(fileSystem, dataSource, "scripts/source/myscript.psc", "scriptname MyScript EXTENDS ObjectReference\n");

        using var stream = link.ReadFileStream();

        var results = parser.ParseFile(stream, link).ToList();

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void ParseFile_MissingFile_ReturnsNoLinks() {
        var (parser, _, dataSource) = CreateParser();
        var link = new DataSourceFileLink(dataSource, "scripts/source/missing.psc");

        using var stream = link.ReadFileStream();

        var results = parser.ParseFile(stream, link).ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void AssetType_IsScriptSource() {
        var (parser, _, _) = CreateParser();

        Assert.Equal("Scripts", parser.Name);
        Assert.Equal(SkyrimScriptSourceAssetType.Instance, parser.AssetType);
    }
}
