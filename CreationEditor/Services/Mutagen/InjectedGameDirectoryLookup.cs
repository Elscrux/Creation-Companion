using System.IO.Abstractions;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments.DI;
using Noggog;
namespace CreationEditor.Services.Mutagen;

public sealed class InjectedGameDirectoryLookup(IFileSystem fileSystem, string gameDirectory) : IGameDirectoryLookup, IDataDirectoryLookup {
    IEnumerable<DirectoryPath> IGameDirectoryLookup.GetAll(GameRelease release) => [gameDirectory];
    bool IGameDirectoryLookup.TryGet(GameRelease release, out DirectoryPath path) {
        path = gameDirectory;
        return true;
    }
    DirectoryPath IGameDirectoryLookup.Get(GameRelease release) => gameDirectory;
    public DirectoryPath? TryGet(GameRelease release) => gameDirectory;
    bool IDataDirectoryLookup.TryGet(GameRelease release, out DirectoryPath path) {
        path = fileSystem.Path.Combine(gameDirectory, "Data");
        return true;
    }
    DirectoryPath IDataDirectoryLookup.Get(GameRelease release) => fileSystem.Path.Combine(gameDirectory, "Data");
    IEnumerable<DirectoryPath> IDataDirectoryLookup.GetAll(GameRelease release) => [fileSystem.Path.Combine(gameDirectory, "Data")];
}
