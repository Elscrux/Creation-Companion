using Avalonia;
namespace CreationEditor.Avalonia.Models.Settings.View;

public interface IViewModeTemplate {
    ViewMode ViewMode { get; }
    Dictionary<string, object> All => Spacings.Select(kv => new KeyValuePair<string, object>(kv.Key, kv.Value))
        .Concat(FontSizes.Select(kv => new KeyValuePair<string, object>(kv.Key, kv.Value)))
        .Concat(MaxHeights.Select(kv => new KeyValuePair<string, object>(kv.Key, kv.Value)))
        .ToDictionary(kv => kv.Key, kv => kv.Value);

    Dictionary<string, Thickness> Spacings { get; }
    Dictionary<string, double> FontSizes { get; }
    Dictionary<string, double> MaxHeights { get; }
}
