using System.IO.Abstractions;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CreationEditor.Services.Asset;
namespace CreationEditor.Avalonia.Services.Asset;

public class ImageLoader(
    IFileSystem fileSystem,
    IDataDirectoryService dataDirectoryService,
    IAssetProvider assetProvider)
    : IImageLoader {

    public IImage? LoadImage(string path) {
        // Get file path
        var fullPath = fileSystem.Path.Combine(dataDirectoryService.Path, path);
        var assetFile = assetProvider.GetAssetFile(fullPath);
        if (assetFile is null) return null;

        // Get stream
        var stream = assetProvider.GetAssetFileStream(assetFile);
        if (stream is null) return null;

        // Create bitmap
        var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);

        memoryStream.Seek(0, SeekOrigin.Begin);

        return new Bitmap(memoryStream);
    }
}
