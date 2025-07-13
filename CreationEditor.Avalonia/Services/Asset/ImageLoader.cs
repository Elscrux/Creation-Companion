using Avalonia.Media;
using Avalonia.Media.Imaging;
using CreationEditor.Services.DataSource;
using Serilog;
namespace CreationEditor.Avalonia.Services.Asset;

public class ImageLoader(
    ILogger logger,
    IDataSourceService dataSourceService)
    : IImageLoader {

    public IImage? LoadImage(string path) {
        // Get file path
        if (!dataSourceService.TryGetFileLink(path, out var link)) {
            logger.Here().Warning("Failed to get file link for {Path}", path);
            return null;
        }

        // Get stream
        var stream = link.ReadFile();
        if (stream is null) return null;

        // Create bitmap
        var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);

        memoryStream.Seek(0, SeekOrigin.Begin);

        return new Bitmap(memoryStream);
    }
}
