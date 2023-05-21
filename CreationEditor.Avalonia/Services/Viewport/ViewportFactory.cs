using Avalonia.Controls;
using CreationEditor.Avalonia.Views.Viewport;
using Mutagen.Bethesda.Environments.DI;
namespace CreationEditor.Avalonia.Services.Viewport;

public sealed class BSEViewportFactory : IViewportFactory {
    private readonly IDataDirectoryProvider _dataDirectoryProvider;

    public bool IsMultiInstanceCapable => false;

    public BSEViewportFactory(
        IDataDirectoryProvider dataDirectoryProvider) {
        _dataDirectoryProvider = dataDirectoryProvider;
    }

    public Control CreateViewport() {
        return new ViewportBSE(
            @"E:\TES\Skyrim\vanilla-files\" //_dataDirectoryProvider.Path
        );
    }
}
