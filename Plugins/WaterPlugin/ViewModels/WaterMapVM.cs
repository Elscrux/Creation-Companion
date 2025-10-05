using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CreationEditor;
using CreationEditor.Avalonia;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.Environment;
using DynamicData;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
using WaterPlugin.Services;
namespace WaterPlugin.ViewModels;

public sealed partial class WaterMapVM : ViewModel {
    private readonly WaterGradientGenerator _generator;
    private readonly ILogger _logger;
    public ILinkCacheProvider LinkCacheProvider { get; }

    [Reactive] public partial FormKey WorldspaceFormKey { get; set; }

    public ImageSelectionVM ShallowWaterMap { get; } = new();
    public ImageSelectionVM DeepWaterMap { get; } = new();
    public ImageSelectionVM ReflectionMap { get; } = new();
    public ImageSelectionVM PresetMap { get; } = new();

    [Reactive] public partial int TopCell { get; set; }
    [Reactive] public partial int BottomCell { get; set; }
    [Reactive] public partial int LeftCell { get; set; }
    [Reactive] public partial int RightCell { get; set; }

    public ObservableCollectionExtended<PresetVM> Presets { get; } = [];

    public ReactiveCommand<Unit, Unit> Generate { get; }
    [Reactive] public partial float ReflectivityAmount { get; set; }
    [Reactive] public partial float ReflectionMagnitude { get; set; }

    public WaterMapVM(
        WaterGradientGenerator generator,
        ILinkCacheProvider linkCacheProvider,
        ILogger logger) {
        _generator = generator;
        _logger = logger;
        TopCell = 64;
        BottomCell = -64;
        LeftCell = -64;
        RightCell = 64;
        ReflectivityAmount = 0.3f;
        ReflectionMagnitude = 0.4f;
        LinkCacheProvider = linkCacheProvider;

        // A worldspace must be selected
        var worldspaceValid = this.WhenAnyValue(x => x.WorldspaceFormKey).Select(x => !x.IsNull);

        // All images must be loaded
        var imagesValid = this.WhenAnyValue(
                x => x.ShallowWaterMap.Source,
                x => x.DeepWaterMap.Source,
                x => x.ReflectionMap.Source,
                x => x.PresetMap.Source)
            .Select(x => x.Item1 is not null && x.Item2 is not null && x.Item3 is not null && x.Item4 is not null);

        var presetsValid = Presets.ToObservableChangeSet()
            .AutoRefresh(x => x.Color)
            .AutoRefresh(x => x.WaterType)
            .ToCollection()
            .Select(presets =>
                // All presets have a proper water type
                presets.All(x => !x.WaterType.IsNull)
                // No duplicate colors or water types
             && presets.DistinctBy(x => x.Color).Count() == presets.Count
             && presets.DistinctBy(x => x.WaterType).Count() == presets.Count);

        var canGenerate = worldspaceValid.CombineLatest(imagesValid, presetsValid, (x, y, z) => x && y && z);
        Generate = ReactiveCommand.CreateRunInBackground(GenerateWaterMap, canGenerate);
    }

    private void GenerateWaterMap() {
        var waterPresets = new Dictionary<System.Drawing.Color, IWaterGetter>();
        foreach (var preset in Presets) {
            if (LinkCacheProvider.LinkCache.TryResolve<IWaterGetter>(preset.WaterType, out var waterGetter)) {
                waterPresets.Add(preset.Color.ToSystemDrawingsColor(), waterGetter);
                continue;
            }

            _logger.Here().Error("Failed to resolve water type {WaterType}", preset.WaterType);
            return;
        }

        _generator.Generate(
            WorldspaceFormKey,
            waterPresets,
            new P2Int(LeftCell, BottomCell),
            new P2Int(RightCell, TopCell),
            ReflectivityAmount,
            ReflectionMagnitude,
            ShallowWaterMap.FilePath!,
            DeepWaterMap.FilePath!,
            ReflectionMap.FilePath!,
            PresetMap.FilePath!);
    }
}

public sealed partial class PresetVM : ViewModel {
    [Reactive] public partial Color Color { get; set; }
    [Reactive] public partial FormKey WaterType { get; set; }

    public PresetVM() {
        Color = Colors.White;
    }
}

public sealed partial class ImageSelectionVM : ViewModel {
    [Reactive] public partial Bitmap? Source { get; set; }
    [Reactive] public partial string? FilePath { get; set; }

    public ImageSelectionVM() {
        this.WhenAnyValue(x => x.FilePath)
            .Subscribe(_ => UpdateImage());
    }

    private void UpdateImage() {
        if (FilePath is null) {
            Source = null;
            return;
        }

        Source = new Bitmap(FilePath);
    }
}
