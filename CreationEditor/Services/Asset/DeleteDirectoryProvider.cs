using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
namespace CreationEditor.Services.Asset;

public sealed class DeleteDirectoryProvider(
    IFileSystem fileSystem,
    IDataDirectoryProvider dataDirectoryProvider
) : IDeleteDirectoryProvider {

    private const string DeleteDirectoryName = "_DELETE_";

    public string DeleteDirectory => fileSystem.Path.Combine(dataDirectoryProvider.Path, DeleteDirectoryName);
}
