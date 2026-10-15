using System.IO.Abstractions.TestingHelpers;
using CreationEditor.Services.Cache;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.FileSystem.Validation;
using Serilog.Core;
namespace CreationEditor.Skyrim.Tests.FileSystem.Validation;

public sealed class HashFileSystemValidationTests {
    private const string SourcePath = @"C:\TestData";

    private static (HashFileSystemValidation Validation, MockFileSystem FileSystem, FileSystemDataSource Source) CreateValidation() {
        var fileSystem = new MockFileSystem();
        Func<IReadOnlyList<string>, ICacheLocationProvider> cacheLocationProviderFactory =
            subDirectories => new CacheLocationProvider(fileSystem, subDirectories);
        var serialization = new BinaryFileSystemValidationSerialization(Logger.None, cacheLocationProviderFactory, fileSystem);
        var validation = new HashFileSystemValidation(serialization, [".nif"]);
        var source = new FileSystemDataSource(fileSystem, SourcePath);
        return (validation, fileSystem, source);
    }

    private static void WriteFile(MockFileSystem fileSystem, string relativePath, params byte[] content) {
        var fullPath = fileSystem.Path.Combine(SourcePath, relativePath);
        var directory = fileSystem.Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory)) fileSystem.Directory.CreateDirectory(directory);
        fileSystem.File.WriteAllBytes(fullPath, content);
    }

    private static async Task BuildCacheAndGetValidResult(
        HashFileSystemValidation validation,
        FileSystemDataSource source) {
        // First call builds the cache and reports everything as invalidated
        var initialResult = await validation.GetInvalidatedContent(source);
        Assert.True(initialResult.CacheFullyInvalidated);
        initialResult.UpdateCache();

        // Second call validates against the fresh cache
        await validation.GetInvalidatedContent(source);
    }

    [Fact]
    public async Task GetInvalidatedContent_NoCache_FullyInvalidated() {
        var (validation, fileSystem, source) = CreateValidation();
        WriteFile(fileSystem, @"meshes\test.nif", 1, 2, 3);

        var result = await validation.GetInvalidatedContent(source);

        Assert.True(result.CacheFullyInvalidated);
    }

    [Fact]
    public async Task GetInvalidatedContent_UnchangedSource_ReturnsValid() {
        var (validation, fileSystem, source) = CreateValidation();
        WriteFile(fileSystem, @"meshes\test.nif", 1, 2, 3);
        await BuildCacheAndGetValidResult(validation, source);

        var result = await validation.GetInvalidatedContent(source);

        Assert.False(result.CacheFullyInvalidated);
        Assert.Empty(result.InvalidatedContent);
    }

    [Fact]
    public async Task GetInvalidatedContent_ChangedFile_ReturnsPartlyInvalid() {
        var (validation, fileSystem, source) = CreateValidation();
        WriteFile(fileSystem, @"meshes\test.nif", 1, 2, 3);
        await BuildCacheAndGetValidResult(validation, source);

        WriteFile(fileSystem, @"meshes\test.nif", 7, 8, 9);
        var result = await validation.GetInvalidatedContent(source);

        Assert.False(result.CacheFullyInvalidated);
        var invalidated = result.InvalidatedContent.Select(c => c.Path).ToList();
        Assert.Contains(@"meshes\test.nif", invalidated);
    }

    [Fact]
    public async Task GetInvalidatedContent_NewFile_ReturnsPartlyInvalid() {
        var (validation, fileSystem, source) = CreateValidation();
        WriteFile(fileSystem, @"meshes\test.nif", 1, 2, 3);
        await BuildCacheAndGetValidResult(validation, source);

        WriteFile(fileSystem, @"meshes\new.nif", 4, 5, 6);
        var result = await validation.GetInvalidatedContent(source);

        Assert.False(result.CacheFullyInvalidated);
        var invalidated = result.InvalidatedContent.Select(c => c.Path).ToList();
        Assert.Contains(@"meshes\new.nif", invalidated);
    }

    [Fact]
    public async Task GetInvalidatedContent_DeletedFile_ReturnsValid() {
        var (validation, fileSystem, source) = CreateValidation();
        WriteFile(fileSystem, @"meshes\test.nif", 1, 2, 3);
        await BuildCacheAndGetValidResult(validation, source);

        fileSystem.File.Delete(fileSystem.Path.Combine(SourcePath, @"meshes\test.nif"));
        var result = await validation.GetInvalidatedContent(source);

        Assert.False(result.CacheFullyInvalidated);
        var invalidated = result.InvalidatedContent.Select(c => c.Path).ToList();
        Assert.DoesNotContain(@"meshes\test.nif", invalidated);
    }

    [Fact]
    public async Task GetInvalidatedContent_ChangedFileOfOtherExtension_NotInvalidated() {
        var (validation, fileSystem, source) = CreateValidation();
        WriteFile(fileSystem, @"meshes\test.nif", 1, 2, 3);
        await BuildCacheAndGetValidResult(validation, source);

        WriteFile(fileSystem, @"meshes\readme.txt", 1, 1, 1);
        var result = await validation.GetInvalidatedContent(source);

        Assert.False(result.CacheFullyInvalidated);
        Assert.Empty(result.InvalidatedContent);
    }

    [Fact]
    public async Task UpdateCache_AfterPartlyInvalid_NextValidationIsValid() {
        var (validation, fileSystem, source) = CreateValidation();
        WriteFile(fileSystem, @"meshes\test.nif", 1, 2, 3);
        await BuildCacheAndGetValidResult(validation, source);

        WriteFile(fileSystem, @"meshes\test.nif", 7, 8, 9);
        var result = await validation.GetInvalidatedContent(source);
        Assert.NotEmpty(result.InvalidatedContent);
        result.UpdateCache();

        var nextResult = await validation.GetInvalidatedContent(source);
        Assert.False(nextResult.CacheFullyInvalidated);
        Assert.Empty(nextResult.InvalidatedContent);
    }

    [Fact]
    public async Task UpdateCache_CalledTwice_OnlyUpdatesOnce() {
        var (validation, fileSystem, source) = CreateValidation();
        WriteFile(fileSystem, @"meshes\test.nif", 1, 2, 3);
        await BuildCacheAndGetValidResult(validation, source);

        WriteFile(fileSystem, @"meshes\test.nif", 7, 8, 9);
        var result = await validation.GetInvalidatedContent(source);
        result.UpdateCache();
        // A second call must be a no-op, not throw or corrupt state
        result.UpdateCache();

        var nextResult = await validation.GetInvalidatedContent(source);
        Assert.Empty(nextResult.InvalidatedContent);
    }
}
