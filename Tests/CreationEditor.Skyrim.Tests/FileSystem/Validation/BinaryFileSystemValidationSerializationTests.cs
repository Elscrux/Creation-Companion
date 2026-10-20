using System.IO.Abstractions.TestingHelpers;
using CreationEditor.Services.Cache;
using CreationEditor.Services.FileSystem.Validation;
using Serilog.Core;
namespace CreationEditor.Skyrim.Tests.FileSystem.Validation;

public sealed class BinaryFileSystemValidationSerializationTests {
    private static readonly string TestDataPath = Path.Combine(Path.GetTempPath(), "TestData");

    private static (BinaryFileSystemValidationSerialization Serialization, MockFileSystem FileSystem, string CacheFile) CreateSerialization() {
        var fileSystem = new MockFileSystem();
        ICacheLocationProvider CacheLocationProviderFactory(IReadOnlyList<string> subDirectories) => new CacheLocationProvider(fileSystem, subDirectories);
        var serialization = new BinaryFileSystemValidationSerialization(Logger.None, CacheLocationProviderFactory, fileSystem);
        var cacheFile = new CacheLocationProvider(fileSystem, "Validation", "FileSystem").CacheFile(TestDataPath);
        return (serialization, fileSystem, cacheFile);
    }

    private static HashFileSystemCacheData CreateCacheData() => new(
        8,
        new HashDirectoryCacheData("Data",
            [
                new HashDirectoryCacheData("meshes", [], [new HashFileCacheData("test.nif", [1, 2, 3, 4, 5, 6, 7, 8])]),
            ],
            [new HashFileCacheData("root.nif", [8, 7, 6, 5, 4, 3, 2, 1])]));

    [Fact]
    public void SerializeThenValidate_ReturnsTrue() {
        var (serialization, _, _) = CreateSerialization();

        serialization.Serialize(CreateCacheData(), TestDataPath);

        Assert.True(serialization.Validate(TestDataPath));
    }

    [Fact]
    public void Validate_MissingCacheFile_ReturnsFalse() {
        var (serialization, _, _) = CreateSerialization();

        Assert.False(serialization.Validate(TestDataPath));
    }

    [Fact]
    public void Validate_WrongVersion_ReturnsFalse() {
        var (serialization, fileSystem, cacheFile) = CreateSerialization();
        fileSystem.Directory.CreateDirectory(fileSystem.Path.GetDirectoryName(cacheFile)!);
        using (var stream = fileSystem.File.OpenWrite(cacheFile))
        using (var writer = new BinaryWriter(stream)) {
            writer.Write("0.0.0.1");
        }

        Assert.False(serialization.Validate(TestDataPath));
    }

    [Fact]
    public void Validate_CorruptedCacheFile_ReturnsFalse() {
        var (serialization, fileSystem, cacheFile) = CreateSerialization();
        serialization.Serialize(CreateCacheData(), TestDataPath);

        fileSystem.File.WriteAllBytes(cacheFile, [1, 2, 3]);

        Assert.False(serialization.Validate(TestDataPath));
    }

    [Fact]
    public void SerializeThenTryDeserialize_RoundTripsCacheData() {
        var (serialization, _, _) = CreateSerialization();
        var cacheData = CreateCacheData();

        serialization.Serialize(cacheData, TestDataPath);
        var success = serialization.TryDeserialize(TestDataPath, out var deserialized);

        Assert.True(success);
        Assert.Equal(cacheData.HashLength, deserialized!.HashLength);
        Assert.Equal(cacheData.RootDirectory.Name, deserialized.RootDirectory.Name);
        var subDirectory = Assert.Single(deserialized.RootDirectory.SubDirectories);
        Assert.Equal("meshes", subDirectory.Name);
        var subFile = Assert.Single(subDirectory.Files);
        Assert.Equal("test.nif", subFile.Name);
        Assert.Equal([1, 2, 3, 4, 5, 6, 7, 8], subFile.Hash);
        var rootFile = Assert.Single(deserialized.RootDirectory.Files);
        Assert.Equal("root.nif", rootFile.Name);
        Assert.Equal([8, 7, 6, 5, 4, 3, 2, 1], rootFile.Hash);
    }

    [Fact]
    public void TryDeserialize_MissingCacheFile_Throws() {
        var (serialization, _, _) = CreateSerialization();

        Assert.ThrowsAny<IOException>(() => serialization.TryDeserialize(TestDataPath, out _));
    }
}
