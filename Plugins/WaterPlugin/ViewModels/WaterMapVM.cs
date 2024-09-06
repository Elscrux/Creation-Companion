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
using ReactiveUI.Fody.Helpers;
using Serilog;
using WaterPlugin.Services;
namespace WaterPlugin.ViewModels;

public sealed class WaterMapVM : ViewModel {
    private readonly WaterGradientGenerator _generator;
    private readonly ILogger _logger;
    public ILinkCacheProvider LinkCacheProvider { get; }

    [Reactive] public FormKey WorldspaceFormKey { get; set; }

    public ImageSelectionVM ShallowWaterMap { get; } = new();
    public ImageSelectionVM DeepWaterMap { get; } = new();
    public ImageSelectionVM ReflectionMap { get; } = new();
    public ImageSelectionVM PresetMap { get; } = new();

    [Reactive] public int TopCell { get; set; } = 64;
    [Reactive] public int BottomCell { get; set; } = -64;
    [Reactive] public int LeftCell { get; set; } = -64;
    [Reactive] public int RightCell { get; set; } = 64;

    public ObservableCollectionExtended<PresetVM> Presets { get; } = [];

    public ReactiveCommand<Unit, Unit> Generate { get; }
    [Reactive] public float ReflectivityAmount { get; set; } = 0.3f;
    [Reactive] public float ReflectionMagnitude { get; set; } = 0.3f;

    public WaterMapVM(
        WaterGradientGenerator generator,
        ILinkCacheProvider linkCacheProvider,
        ILogger logger) {
        _generator = generator;
        _logger = logger;
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
            new P2Int(-65, -70),
            new P2Int(96, 64),
            ReflectivityAmount,
            ReflectionMagnitude,
            ShallowWaterMap.FilePath!,
            DeepWaterMap.FilePath!,
            ReflectionMap.FilePath!,
            PresetMap.FilePath!);
    }
}

public sealed class PresetVM : ViewModel {
    [Reactive] public Color Color { get; set; } = Colors.White;
    [Reactive] public FormKey WaterType { get; set; }
}

public sealed class ImageSelectionVM : ViewModel {
    [Reactive] public Bitmap? Source { get; set; }
    [Reactive] public string? FilePath { get; set; }

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
