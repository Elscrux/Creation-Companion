using System.IO.Abstractions.TestingHelpers;
using CreationEditor.Services.Cache;
namespace CreationEditor.Skyrim.Tests.Cache;

public sealed class CacheLocationProviderTests {
    private static string AppDataCacheDirectory => Path.Combine(
        System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
        "MutagenCache");

    [Fact]
    public void CacheFile_SingleIdentifier_ReturnsPathInCacheDirectory() {
        var fileSystem = new MockFileSystem();
        var provider = new CacheLocationProvider(fileSystem);

        var path = provider.CacheFile("identifier");

        Assert.Equal(Path.Combine(AppDataCacheDirectory, "identifier.cache"), path);
    }

    [Fact]
    public void CacheFile_MultipleIdentifiers_BuildsNestedPath() {
        var fileSystem = new MockFileSystem();
        var provider = new CacheLocationProvider(fileSystem, "plugin");

        var path = provider.CacheFile("records", "armor");

        Assert.Equal(Path.Combine(AppDataCacheDirectory, "plugin", "records", "armor.cache"), path);
    }

    [Fact]
    public void CacheFile_SanitizesIllegalCharactersInFileName() {
        var fileSystem = new MockFileSystem();
        var provider = new CacheLocationProvider(fileSystem);

        var path = provider.CacheFile(@"a/b:c*d?e""f<g>h|i");

        Assert.Equal(Path.Combine(AppDataCacheDirectory, "abcdefghi.cache"), path);
    }

    [Fact]
    public void CacheFile_SanitizesIllegalCharactersInDirectoryIdentifiers() {
        var fileSystem = new MockFileSystem();
        var provider = new CacheLocationProvider(fileSystem);

        var path = provider.CacheFile("a/b", "identifier");

        Assert.Equal(Path.Combine(AppDataCacheDirectory, "ab", "identifier.cache"), path);
    }

    [Fact]
    public void Constructor_SanitizesIllegalCharactersInSubDirectories() {
        var fileSystem = new MockFileSystem();
        var provider = new CacheLocationProvider(fileSystem, "x/y:z");

        var path = provider.CacheFile("identifier");

        Assert.Equal(Path.Combine(AppDataCacheDirectory, "xyz", "identifier.cache"), path);
    }

    [Fact]
    public void CacheFile_NoIdentifiers_Throws() {
        var fileSystem = new MockFileSystem();
        var provider = new CacheLocationProvider(fileSystem);

        // The last-identifier access in CacheFile throws before the ArgumentException guard in GetIdentifierPath is reached
        Assert.ThrowsAny<ArgumentException>(() => provider.CacheFile());
    }
}
