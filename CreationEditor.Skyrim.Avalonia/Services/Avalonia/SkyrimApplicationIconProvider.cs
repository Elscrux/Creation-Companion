using System;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CreationEditor.Avalonia.Services.Avalonia;
namespace CreationEditor.Skyrim.Avalonia.Services.Avalonia;

public sealed class SkyrimApplicationIconProvider : IApplicationIconProvider {
    private const string BasePath = "avares://Creation Companion";
    private const string Path = "Assets/Skyrim.ico";
    private const string SpinningPath = "Assets/Skyrim-Spinning.ico";

    public IImage Icon { get; } = LoadImage(Path);
    public IImage SpinningIcon { get; } = LoadImage(SpinningPath);

    private static IImage LoadImage(string path) {
        using var stream = AssetLoader.Open(new Uri($"{BasePath}/{path}"));
        return new Bitmap(stream);
    }
}
