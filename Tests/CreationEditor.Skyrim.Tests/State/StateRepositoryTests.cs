using System.IO.Abstractions.TestingHelpers;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Serialization.Json;
using CreationEditor.Services.State;
using Newtonsoft.Json.Serialization;
using Serilog.Core;
namespace CreationEditor.Skyrim.Tests.State;

public sealed class NamedGuidStateIdentifierTests {
    [Fact]
    public void AsFileName_WithName_UsesSeparator() {
        var id = new NamedGuid(new Guid("12345678-1234-1234-1234-123456789abc"), "DataSource");

        var fileName = new NamedGuidStateIdentifier().AsFileName(id);

        Assert.Equal($"DataSource~{id.Id}", fileName);
    }

    [Fact]
    public void AsFileName_WithoutName_IsPlainGuid() {
        var id = new NamedGuid(new Guid("12345678-1234-1234-1234-123456789abc"), "");

        var fileName = new NamedGuidStateIdentifier().AsFileName(id);

        Assert.Equal(id.Id.ToString(), fileName);
    }

    [Fact]
    public void Parse_RoundTripsNamedIdentifier() {
        var identifier = new NamedGuidStateIdentifier();
        var id = new NamedGuid(new Guid("12345678-1234-1234-1234-123456789abc"), "DataSource");

        var parsed = identifier.Parse(identifier.AsFileName(id));

        Assert.Equal(id, parsed);
    }

    [Fact]
    public void Parse_RoundTripsUnnamedIdentifier() {
        var identifier = new NamedGuidStateIdentifier();
        var id = new NamedGuid(new Guid("12345678-1234-1234-1234-123456789abc"), "");

        var parsed = identifier.Parse(identifier.AsFileName(id));

        Assert.Equal(id, parsed);
    }
}

public sealed class JsonStateRepositoryTests {
    private static (MockFileSystem FileSystem, JsonStateRepository<DataSourceMemento, DataSourceMemento, NamedGuid> Repository) CreateRepository(
        string stateId = "TestState") {
        var fileSystem = new MockFileSystem();
        var repository = new JsonStateRepository<DataSourceMemento, DataSourceMemento, NamedGuid>(
            new JsonSerializerSettingsProvider(new DefaultContractResolver()),
            new NamedGuidStateIdentifier(),
            Logger.None,
            fileSystem,
            stateId);
        return (fileSystem, repository);
    }

    private static readonly string SomeSourcePath = Path.Combine(Path.GetTempPath(), "SomeSource");

    private static DataSourceMemento CreateMemento(string? path = null) => new(DataSourceType.FileSystem, "SomeSource", path ?? SomeSourcePath, false);

    [Fact]
    public void SaveThenLoad_RoundTripsState() {
        var (_, repository) = CreateRepository();
        var memento = CreateMemento();
        var id = new NamedGuid(Guid.NewGuid(), "SomeSource");

        Assert.True(repository.Save(memento, id));

        var loaded = repository.Load(id);
        Assert.Equal(memento, loaded);
    }

    [Fact]
    public void Load_MissingState_ReturnsNull() {
        var (_, repository) = CreateRepository();

        Assert.Null(repository.Load(new NamedGuid(Guid.NewGuid(), "Missing")));
    }

    [Fact]
    public void LoadAll_ReturnsAllSavedStates() {
        var (_, repository) = CreateRepository();
        var memento1 = CreateMemento(Path.Combine(Path.GetTempPath(), "Source1"));
        var memento2 = CreateMemento(Path.Combine(Path.GetTempPath(), "Source2"));
        repository.Save(memento1, new NamedGuid(Guid.NewGuid(), "Source1"));
        repository.Save(memento2, new NamedGuid(Guid.NewGuid(), "Source2"));

        var all = repository.LoadAll().ToList();

        Assert.Equal(2, all.Count);
        Assert.Contains(memento1, all);
        Assert.Contains(memento2, all);
    }

    [Fact]
    public void LoadAllWithIdentifier_ParsesIdentifiers() {
        var (_, repository) = CreateRepository();
        var memento = CreateMemento();
        var id = new NamedGuid(Guid.NewGuid(), "SomeSource");
        repository.Save(memento, id);

        var withIdentifier = repository.LoadAllWithIdentifier();

        var pair = Assert.Single(withIdentifier);
        Assert.Equal(id, pair.Key);
        Assert.Equal(memento, pair.Value);
    }

    [Fact]
    public void Update_AppliesFunctionAndPersists() {
        var (_, repository) = CreateRepository();
        var id = new NamedGuid(Guid.NewGuid(), "SomeSource");
        repository.Save(CreateMemento(), id);

        repository.Update(existing => existing! with { IsReadyOnly = true }, id);

        var loaded = repository.Load(id);
        Assert.True(loaded!.IsReadyOnly);
    }

    [Fact]
    public void Delete_RemovesState() {
        var (_, repository) = CreateRepository();
        var id = new NamedGuid(Guid.NewGuid(), "SomeSource");
        repository.Save(CreateMemento(), id);

        repository.Delete(id);

        Assert.Null(repository.Load(id));
        Assert.Equal(0, repository.Count());
    }

    [Fact]
    public void Count_ReflectsSavedStates() {
        var (_, repository) = CreateRepository();
        Assert.Equal(0, repository.Count());

        repository.Save(CreateMemento(), new NamedGuid(Guid.NewGuid(), "Source1"));
        repository.Save(CreateMemento(), new NamedGuid(Guid.NewGuid(), "Source2"));

        Assert.Equal(2, repository.Count());
    }

    [Fact]
    public void SeparateRepositoryInstances_SharePersistedState() {
        var (fileSystem, repository) = CreateRepository();
        var id = new NamedGuid(Guid.NewGuid(), "SomeSource");
        repository.Save(CreateMemento(), id);

        // A new repository over the same state id reads the persisted state
        var secondRepository = new JsonStateRepository<DataSourceMemento, DataSourceMemento, NamedGuid>(
            new JsonSerializerSettingsProvider(new DefaultContractResolver()),
            new NamedGuidStateIdentifier(),
            Logger.None,
            fileSystem,
            "TestState");

        Assert.Equal(CreateMemento(), secondRepository.Load(id));
    }
}

public sealed class CachedStateRepositoryTests {
    private static (MockFileSystem FileSystem, IStateRepository<DataSourceMemento, DataSourceMemento, NamedGuid> Repository) CreateRepository() {
        var fileSystem = new MockFileSystem();
        StateRepositoryFactory<DataSourceMemento, DataSourceMemento, NamedGuid> factory = null!;
        factory = new StateRepositoryFactory<DataSourceMemento, DataSourceMemento, NamedGuid>(
            stateIds => new JsonStateRepository<DataSourceMemento, DataSourceMemento, NamedGuid>(
                new JsonSerializerSettingsProvider(new DefaultContractResolver()),
                new NamedGuidStateIdentifier(),
                Logger.None,
                fileSystem,
                stateIds),
            stateIds => new CachedStateRepository<DataSourceMemento, DataSourceMemento, NamedGuid>(stateIds, factory));
        return (fileSystem, factory.CreateCached("TestState"));
    }

    [Fact]
    public void Load_CachesLoadedState() {
        var (_, repository) = CreateRepository();
        var id = new NamedGuid(Guid.NewGuid(), "SomeSource");
        repository.Save(new DataSourceMemento(DataSourceType.FileSystem, "SomeSource", Path.Combine(Path.GetTempPath(), "Source"), false), id);

        var loaded1 = repository.Load(id);
        var loaded2 = repository.Load(id);

        Assert.Equal(loaded1, loaded2);
    }

    [Fact]
    public void Delete_InvalidatesCache() {
        var (_, repository) = CreateRepository();
        var id = new NamedGuid(Guid.NewGuid(), "SomeSource");
        repository.Save(new DataSourceMemento(DataSourceType.FileSystem, "SomeSource", Path.Combine(Path.GetTempPath(), "Source"), false), id);
        Assert.NotNull(repository.Load(id));

        repository.Delete(id);

        Assert.Null(repository.Load(id));
    }

    [Fact]
    public void Save_UpdatesCacheAndPersistence() {
        var (_, repository) = CreateRepository();
        var id = new NamedGuid(Guid.NewGuid(), "SomeSource");

        repository.Save(new DataSourceMemento(DataSourceType.FileSystem, "SomeSource", Path.Combine(Path.GetTempPath(), "Source"), false), id);
        repository.Save(new DataSourceMemento(DataSourceType.FileSystem, "SomeSource", Path.Combine(Path.GetTempPath(), "Source"), true), id);

        Assert.True(repository.Load(id)!.IsReadyOnly);
    }
}
