using System.IO.Abstractions.TestingHelpers;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using Mutagen.Bethesda.Environments.DI;
using Noggog;
using Serilog.Core;
using ConfigValues = System.Collections.Generic.Dictionary<string, object>;

namespace CreationEditor.Skyrim.Tests.Config;

public sealed class ConfigOptionAutoDetectorTests {
    private static readonly string GamePath = Path.Combine(Path.GetTempPath(), "Games", "Skyrim");
    private static readonly string DataPath = Path.Combine(GamePath, "Data");

    private static ConfigOptionAutoDetector CreateDetector(MockFileSystem fileSystem) => new(
        fileSystem,
        new GameDirectoryInjection(new DirectoryPath(GamePath)),
        new DataDirectoryInjection(new DirectoryPath(DataPath)),
        Logger.None);

    [Fact]
    public void FileOption_FirstExistingCandidateWins() {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(Path.Combine(DataPath, "tool.exe"), new MockFileData([]));
        var option = new FileConfigOption(
            "Tool",
            autoDetectPaths: [
                new AutoDetectPath(AutoDetectBaseDirectory.Data, "missing.exe"),
                new AutoDetectPath(AutoDetectBaseDirectory.Data, "tool.exe"),
            ]);
        var configValues = new ConfigValues();

        CreateDetector(fileSystem).AutoDetect(new TestConfigDefinition(option), configValues);

        Assert.Equal(Path.Combine(DataPath, "tool.exe"), configValues[option.Name]);
    }

    [Fact]
    public void FileOption_NoCandidateExists_ValueStaysUnset() {
        var fileSystem = new MockFileSystem();
        var option = new FileConfigOption(
            "Tool",
            autoDetectPaths: [new AutoDetectPath(AutoDetectBaseDirectory.Data, "missing.exe")]);
        var configValues = new ConfigValues();

        CreateDetector(fileSystem).AutoDetect(new TestConfigDefinition(option), configValues);

        Assert.False(configValues.ContainsKey(option.Name));
    }

    [Fact]
    public void FileOption_AlreadySet_IsSkipped() {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(Path.Combine(DataPath, "tool.exe"), new MockFileData([]));
        var option = new FileConfigOption(
            "Tool",
            autoDetectPaths: [new AutoDetectPath(AutoDetectBaseDirectory.Data, "tool.exe")]);
        var customPath = Path.Combine(Path.GetTempPath(), "Custom", "tool.exe");
        var configValues = new ConfigValues { [option.Name] = customPath };

        CreateDetector(fileSystem).AutoDetect(new TestConfigDefinition(option), configValues);

        Assert.Equal(customPath, configValues[option.Name]);
    }

    [Fact]
    public void FileOption_AlreadySetToWhitespace_IsAutoDetected() {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(Path.Combine(DataPath, "tool.exe"), new MockFileData([]));
        var option = new FileConfigOption(
            "Tool",
            autoDetectPaths: [new AutoDetectPath(AutoDetectBaseDirectory.Data, "tool.exe")]);
        var configValues = new ConfigValues { [option.Name] = "   " };

        CreateDetector(fileSystem).AutoDetect(new TestConfigDefinition(option), configValues);

        Assert.Equal(Path.Combine(DataPath, "tool.exe"), configValues[option.Name]);
    }

    [Fact]
    public void FolderOption_MatchesDirectories() {
        var fileSystem = new MockFileSystem();
        fileSystem.AddDirectory(Path.Combine(DataPath, "Meshes"));
        var option = new FolderConfigOption(
            "Meshes",
            autoDetectPaths: [new AutoDetectPath(AutoDetectBaseDirectory.Data, "Meshes")]);
        var configValues = new ConfigValues();

        CreateDetector(fileSystem).AutoDetect(new TestConfigDefinition(option), configValues);

        Assert.Equal(Path.Combine(DataPath, "Meshes"), configValues[option.Name]);
    }

    [Fact]
    public void FileOption_DoesNotMatchDirectories() {
        var fileSystem = new MockFileSystem();
        fileSystem.AddDirectory(Path.Combine(DataPath, "Meshes"));
        var option = new FileConfigOption(
            "Meshes",
            autoDetectPaths: [new AutoDetectPath(AutoDetectBaseDirectory.Data, "Meshes")]);
        var configValues = new ConfigValues();

        CreateDetector(fileSystem).AutoDetect(new TestConfigDefinition(option), configValues);

        Assert.False(configValues.ContainsKey(option.Name));
    }

    [Fact]
    public void GameBaseDirectory_IsResolved() {
        var fileSystem = new MockFileSystem();
        fileSystem.AddFile(Path.Combine(GamePath, "SkyrimSE.exe"), new MockFileData([]));
        var option = new FileConfigOption(
            "GameExe",
            autoDetectPaths: [new AutoDetectPath(AutoDetectBaseDirectory.Game, "SkyrimSE.exe")]);
        var configValues = new ConfigValues();

        CreateDetector(fileSystem).AutoDetect(new TestConfigDefinition(option), configValues);

        Assert.Equal(Path.Combine(GamePath, "SkyrimSE.exe"), configValues[option.Name]);
    }

    [Fact]
    public void MissingBaseDirectory_IsHandledGracefully() {
        var fileSystem = new MockFileSystem();
        var option = new FileConfigOption(
            "Tool",
            autoDetectPaths: [
                new AutoDetectPath(AutoDetectBaseDirectory.Game, "tool.exe"),
                new AutoDetectPath(AutoDetectBaseDirectory.Data, "tool.exe"),
            ]);
        var configValues = new ConfigValues();

        // Game directory provider has no path (null), data directory provider is set but the file is missing
        var detector = new ConfigOptionAutoDetector(
            fileSystem,
            new GameDirectoryInjection(null),
            new DataDirectoryInjection(new DirectoryPath(DataPath)),
            Logger.None);
        detector.AutoDetect(new TestConfigDefinition(option), configValues);

        Assert.False(configValues.ContainsKey(option.Name));
    }

    [Fact]
    public void OptionsWithoutAutoDetectPaths_AreIgnored() {
        var fileSystem = new MockFileSystem();
        var textOption = new TextConfigOption("Text");
        var configValues = new ConfigValues();

        CreateDetector(fileSystem).AutoDetect(new TestConfigDefinition(textOption), configValues);

        Assert.Empty(configValues);
    }
}
