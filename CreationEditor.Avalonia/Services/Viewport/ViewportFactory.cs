using System.IO.Abstractions;
using Avalonia.Controls;
using CreationEditor.Avalonia.Views.Viewport;
using Mutagen.Bethesda.Environments.DI;
namespace CreationEditor.Avalonia.Services.Viewport;

public sealed class BSEViewportFactory : IViewportFactory {
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IFileSystem _fileSystem;

    public bool IsMultiInstanceCapable => false;

    public BSEViewportFactory(
        IDataDirectoryProvider dataDirectoryProvider,
        IFileSystem fileSystem) {
        _dataDirectoryProvider = dataDirectoryProvider;
        _fileSystem = fileSystem;
    }

    public Control CreateViewport() {
        var assetDirectory = _dataDirectoryProvider.Path;
        var bsaFileNames = _fileSystem.Directory
            .EnumerateFiles(assetDirectory, "*.bsa")
            .Select(path => _fileSystem.Path.GetFileName(path))
            // .OrderBy() todo order by load order priority (+ exclude non-loaded BSAs? would need ini parsing to check which inis are always loaded)
            .ToArray();

        return new ViewportBSE(
            assetDirectory,
            bsaFileNames
        );
    }
}
