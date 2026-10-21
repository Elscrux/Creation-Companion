using System.IO.Abstractions.TestingHelpers;
using CreationEditor.Services.Serialization.Json;
using CreationEditor.Services.Settings;
using Newtonsoft.Json.Serialization;
using Serilog.Core;
namespace CreationEditor.Skyrim.Tests.Settings;

public sealed record TestSettingModel(string Value) : ISettingModel;

public sealed class TestSetting : ISettingNode {
    public string Name => "Test Setting";
    public ISettingModel Model { get; set; } = new TestSettingModel("default");
    public bool Applied { get; private set; }
    public Type? Parent { get; init; }
    public List<ISettingNode> Children { get; } = [];

    public void Apply() => Applied = true;
}

public sealed class SettingPathProviderTests {
    [Fact]
    public void GetFullPath_ReplacesSpacesWithDashes() {
        var pathProvider = new SettingPathProvider(new MockFileSystem());

        var path = pathProvider.GetFullPath(new TestSetting());

        Assert.EndsWith(Path.Combine("Settings", "Test-Setting.json"), path);
    }
}

public sealed class SettingProviderTests {
    [Fact]
    public void BuildsTree_FromParentTypes() {
        var root = new TestSetting();
        var child = new TestSetting { Parent = typeof(TestSetting) };

        var provider = new SettingProvider([root, child], Logger.None);

        var rootSetting = Assert.Single(provider.Settings);
        Assert.Same(root, rootSetting);
        Assert.Same(child, Assert.Single(root.Children));
    }

    [Fact]
    public void SettingsWithoutParent_AreRoots() {
        var root1 = new TestSetting();
        var root2 = new TestSetting();

        var provider = new SettingProvider([root1, root2], Logger.None);

        Assert.Equal(2, provider.Settings.Count());
    }

    [Fact]
    public void OrphanedSettings_AreNotAttachedAndNotRoots() {
        var root = new TestSetting();
        var orphan = new TestSetting { Parent = typeof(Guid) };

        var provider = new SettingProvider([root, orphan], Logger.None);

        // The orphan's parent type is not part of the settings, so it is neither root nor child
        Assert.Same(root, Assert.Single(provider.Settings));
        Assert.Empty(root.Children);
    }
}

public sealed class JsonSettingExportImportTests {
    private static (MockFileSystem FileSystem, JsonSettingExporter Exporter, JsonSettingImporter<TestSettingModel> Importer) Create() {
        var fileSystem = new MockFileSystem();
        var pathProvider = new SettingPathProvider(fileSystem);
        var jsonSettings = new JsonSerializerSettingsProvider(new DefaultContractResolver());
        var exporter = new JsonSettingExporter(Logger.None, fileSystem, pathProvider, jsonSettings);
        var importer = new JsonSettingImporter<TestSettingModel>(Logger.None, fileSystem, pathProvider, jsonSettings);
        return (fileSystem, exporter, importer);
    }

    [Fact]
    public void Export_WritesSettingFile() {
        var (fileSystem, exporter, _) = Create();
        var setting = new TestSetting { Model = new TestSettingModel("exported") };

        Assert.True(exporter.Export(setting));
        Assert.True(fileSystem.File.Exists(fileSystem.Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Settings",
            "Test-Setting.json")));
    }

    [Fact]
    public void ExportThenImport_RoundTripsModel() {
        var (_, exporter, importer) = Create();
        var setting = new TestSetting { Model = new TestSettingModel("exported") };

        exporter.Export(setting);
        var imported = importer.Import(setting);

        Assert.Equal(new TestSettingModel("exported"), imported);
    }

    [Fact]
    public void Import_MissingFile_ReturnsDefault() {
        var (_, _, importer) = Create();

        Assert.Null(importer.Import(new TestSetting()));
    }
}
