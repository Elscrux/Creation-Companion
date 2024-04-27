using Avalonia.Media;
namespace CreationEditor.Avalonia.Services.Asset;

public interface IImageLoader {
    /// <summary>
    /// Load an image from a path.
    /// </summary>
    /// <param name="path">Data relative path to the image</param>
    /// <returns>Image loaded from the path</returns>
    IImage? LoadImage(string path);
}
