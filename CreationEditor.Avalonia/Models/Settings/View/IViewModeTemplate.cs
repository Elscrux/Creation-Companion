using Avalonia;
namespace CreationEditor.Avalonia.Models.Settings.View;

public interface IViewModeTemplate {
    public ViewMode ViewMode { get; }
    public Dictionary<string, object> All => Spacings.Select(kv => new KeyValuePair<string, object>(kv.Key, kv.Value))
        .Concat(FontSizes.Select(kv => new KeyValuePair<string, object>(kv.Key, kv.Value)))
        .Concat(MaxHeights.Select(kv => new KeyValuePair<string, object>(kv.Key, kv.Value)))
        .ToDictionary(kv => kv.Key, kv => kv.Value);

    public Dictionary<string, Thickness> Spacings { get; }
    public Dictionary<string, double> FontSizes { get; }
    public Dictionary<string, double> MaxHeights { get; }
}
