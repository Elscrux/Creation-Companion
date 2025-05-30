using System.IO.Hashing;
using System.Text;
using Avalonia.Platform;
using CreationEditor;
using CreationEditor.Avalonia;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Skyrim;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Noggog;
using Serilog;
using SkiaSharp;
using Color = System.Drawing.Color;
using Water = Mutagen.Bethesda.Skyrim.Water;
namespace WaterPlugin.Services;

public sealed class WaterGradientGenerator(
    ILogger logger,
    IDataSourceService dataSourceService,
    IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
    RecordController<ISkyrimMod, ISkyrimModGetter> recordController) {
    private const string DefaultWater = @"Data\Textures\Water\DefaultWater.dds";
    private const string RiverFlow = @"Data\Textures\Water\RiverFlow.dds";

    private static readonly Uri Uri = new("avares://WaterPlugin/Assets/FlowTemplate.dds");

    public void Generate(
        FormKey worldspaceFormKey,
        Dictionary<Color, IWaterGetter> waterPresets,
        P2Int bottomLeftGrid,
        P2Int topRightGrid,
        float reflectivityAmount,
        float reflectionMagnitude,
        string shallowWaterMapPath,
        string deepWaterMapPath,
        string reflectionMapPath,
        string presetMapPath) {
        using var flowMapTemplate = AssetLoader.Open(Uri);
        var flowMapBytes = new byte[flowMapTemplate.Length];
        var readBytes = flowMapTemplate.Read(flowMapBytes);
        if (readBytes != flowMapBytes.Length) {
            logger.Here().Error("Failed to read flow map template");
            return;
        }

        var worldspace = recordController
            .GetOrAddOverride<Worldspace, IWorldspaceGetter>(
                new FormLinkInformation(worldspaceFormKey, typeof(IWorldspaceGetter)),
                editorEnvironment.ActiveMod);

        var shallowMap = SKBitmap.Decode(shallowWaterMapPath);
        var deepMap = SKBitmap.Decode(deepWaterMapPath);
        var reflectionMap = SKBitmap.Decode(reflectionMapPath);
        var presetMap = SKBitmap.Decode(presetMapPath);

        var gridSize = shallowMap.Width / (topRightGrid.X - bottomLeftGrid.X);

        // Create water for each cell
        foreach (var cellGetter in editorEnvironment.LinkCache.EnumerateAllCells(worldspaceFormKey)) {
            if (cellGetter.Grid is null) continue;

            var x = cellGetter.Grid.Point.X - bottomLeftGrid.X;
            var y = topRightGrid.Y - cellGetter.Grid.Point.Y;

            // Exclude cells outside the specified range
            if (x < 0 || y < 0 || x >= topRightGrid.X - bottomLeftGrid.X || y >= topRightGrid.Y - bottomLeftGrid.Y) continue;

            var shallowColor = GetAverageColor(shallowMap, x, y, gridSize);
            if (shallowColor is null) continue;

            var deepColor = GetAverageColor(deepMap, x, y, gridSize);
            if (deepColor is null) continue;

            var reflectionColor = GetAverageColor(reflectionMap, x, y, gridSize);
            if (reflectionColor is null) continue;

            var preset = GetAverageColor(presetMap, x, y, gridSize);
            if (preset is null) continue;

            var cell = recordController.GetOrAddOverride<Cell, ICellGetter>(cellGetter);

            var waterWeights = GetWaterWeights(preset.Value, waterPresets);
            var water = GetWaterType(shallowColor.Value, deepColor.Value, reflectionColor.Value, waterWeights);
            cell.Water.SetTo(water.FormKey);

            // Add flow file
            var fileSystem = dataSourceService.ActiveDataSource.FileSystem;
            var directoryPath = fileSystem.Path.Combine(
                dataSourceService.ActiveDataSource.Path,
                "Textures",
                "Water",
                editorEnvironment.ActiveMod.ModKey.FileName);
            var filePath = fileSystem.Path.Combine(directoryPath,
                $"Flow.{cellGetter.Grid.Point.X}.{cellGetter.Grid.Point.Y}.dds");

            if (!fileSystem.File.Exists(filePath)) {
                fileSystem.Directory.CreateDirectory(directoryPath);
                var fileSystemStream = fileSystem.File.Create(filePath);
                fileSystemStream.Write(flowMapBytes, 0, flowMapBytes.Length);
            }
        }

        IWaterGetter GetWaterType(SKColor shallowColor, SKColor deepColor, SKColor reflectionColor, Dictionary<IWaterGetter, float> waterWeights) {
            var shallow = shallowColor.ToSystemDrawingColor();
            var deep = deepColor.ToSystemDrawingColor();
            var reflection = reflectionColor.ToSystemDrawingColor();

            var hashAlgorithm = new XxHash3();
            var weightsStr = string.Join("_", waterWeights.Select(x => x.Key.FormKey.ToString() + x.Value));
            hashAlgorithm.Append(Encoding.UTF8.GetBytes(weightsStr));
            var hash = hashAlgorithm.GetCurrentHash();

            var editorId = $"{worldspace.EditorID}Water"
              + "_S" + shallow.ToHexString(ColorExt.IncludeAlpha.Never)[1..]
              + "_D" + deep.ToHexString(ColorExt.IncludeAlpha.Never)[1..]
              + "_R" + reflection.ToHexString(ColorExt.IncludeAlpha.Never)[1..]
              + "_P" + hash.ToHexString();

            if (editorEnvironment.LinkCache.TryResolve<IWaterGetter>(editorId, out var existingWater)) return existingWater;

            var water = recordController.CreateRecord<Water, IWaterGetter>();
            water.EditorID = editorId;
            water.Flags = Water.Flag.EnableFlowmap;
            water.ImageSpace = new FormLinkNullable<IImageSpaceGetter>(Update.ImageSpace.UnderwaterImageSpace.FormKey);
            water.ShallowColor = shallow;
            water.DeepColor = deep;
            water.ReflectionColor = reflection;
            water.WaterReflectivity = reflectivityAmount;
            water.WaterReflectionMagnitude = reflectionMagnitude;
            water.Opacity = GetWeightedPropertyByte(waterWeights, p => p.Opacity);
            water.WaterFresnel = GetWeightedProperty(waterWeights, p => p.WaterFresnel);
            water.DisplacementStartingSize = GetWeightedProperty(waterWeights, p => p.DisplacementStartingSize);
            water.DisplacementFoce = GetWeightedProperty(waterWeights, p => p.DisplacementFoce);
            water.DisplacementVelocity = GetWeightedProperty(waterWeights, p => p.DisplacementVelocity);
            water.DisplacementFalloff = GetWeightedProperty(waterWeights, p => p.DisplacementFalloff);
            water.DisplacementDampner = GetWeightedProperty(waterWeights, p => p.DisplacementDampner);
            water.WaterRefractionMagnitude = GetWeightedProperty(waterWeights, p => p.WaterRefractionMagnitude);
            water.FogAboveWaterAmount = GetWeightedProperty(waterWeights, p => p.FogAboveWaterAmount);
            water.FogAboveWaterDistanceNearPlane = GetWeightedProperty(waterWeights, p => p.FogAboveWaterDistanceNearPlane);
            water.FogAboveWaterDistanceFarPlane = GetWeightedProperty(waterWeights, p => p.FogAboveWaterDistanceFarPlane);
            water.FogUnderWaterAmount = GetWeightedProperty(waterWeights, p => p.FogUnderWaterAmount);
            water.FogUnderWaterDistanceNearPlane = GetWeightedProperty(waterWeights, p => p.FogUnderWaterDistanceNearPlane);
            water.FogUnderWaterDistanceFarPlane = GetWeightedProperty(waterWeights, p => p.FogUnderWaterDistanceFarPlane);
            water.SpecularPower = GetWeightedProperty(waterWeights, p => p.SpecularPower);
            water.SpecularRadius = GetWeightedProperty(waterWeights, p => p.SpecularRadius);
            water.SpecularBrightness = GetWeightedProperty(waterWeights, p => p.SpecularBrightness);
            water.SpecularSunPower = GetWeightedProperty(waterWeights, p => p.SpecularSunPower);
            water.SpecularSunSparkleMagnitude = GetWeightedProperty(waterWeights, p => p.SpecularSunSparkleMagnitude);
            water.SpecularSunSpecularMagnitude = GetWeightedProperty(waterWeights, p => p.SpecularSunSpecularMagnitude);
            water.SpecularSunSparklePower = GetWeightedProperty(waterWeights, p => p.SpecularSunSparklePower);
            water.DepthSpecularLighting = GetWeightedProperty(waterWeights, p => p.DepthSpecularLighting);
            water.DepthReflections = GetWeightedProperty(waterWeights, p => p.DepthReflections);
            water.DepthRefraction = GetWeightedProperty(waterWeights, p => p.DepthRefraction);
            water.DepthNormals = GetWeightedProperty(waterWeights, p => p.DepthNormals);
            water.NoiseLayerOneWindDirection = GetWeightedProperty(waterWeights, p => p.NoiseLayerOneWindDirection);
            water.NoiseFalloff = GetWeightedProperty(waterWeights, p => p.NoiseFalloff);
            water.NoiseLayerTwoWindDirection = GetWeightedProperty(waterWeights, p => p.NoiseLayerTwoWindDirection);
            water.NoiseLayerThreeWindDirection = GetWeightedProperty(waterWeights, p => p.NoiseLayerThreeWindDirection);
            water.NoiseLayerOneWindSpeed = GetWeightedProperty(waterWeights, p => p.NoiseLayerOneWindSpeed);
            water.NoiseLayerTwoWindSpeed = GetWeightedProperty(waterWeights, p => p.NoiseLayerTwoWindSpeed);
            water.NoiseLayerThreeWindSpeed = GetWeightedProperty(waterWeights, p => p.NoiseLayerThreeWindSpeed);
            water.NoiseFlowmapScale = GetWeightedProperty(waterWeights, p => p.NoiseFlowmapScale);
            water.NoiseLayerOneUvScale = GetWeightedProperty(waterWeights, p => p.NoiseLayerOneUvScale);
            water.NoiseLayerTwoUvScale = GetWeightedProperty(waterWeights, p => p.NoiseLayerTwoUvScale);
            water.NoiseLayerThreeUvScale = GetWeightedProperty(waterWeights, p => p.NoiseLayerThreeUvScale);
            water.NoiseLayerOneAmplitudeScale = GetWeightedProperty(waterWeights, p => p.NoiseLayerOneAmplitudeScale);
            water.NoiseLayerTwoAmplitudeScale = GetWeightedProperty(waterWeights, p => p.NoiseLayerTwoAmplitudeScale);
            water.NoiseLayerThreeAmplitudeScale = GetWeightedProperty(waterWeights, p => p.NoiseLayerThreeAmplitudeScale);
            water.AngularVelocity = new P3Float();
            water.LinearVelocity = new P3Float();
            water.NoiseLayerOneTexture = new AssetLink<SkyrimTextureAssetType>(DefaultWater);
            water.NoiseLayerTwoTexture = new AssetLink<SkyrimTextureAssetType>(DefaultWater);
            water.NoiseLayerThreeTexture = new AssetLink<SkyrimTextureAssetType>(DefaultWater);
            water.FlowNormalsNoiseTexture = new AssetLink<SkyrimTextureAssetType>(RiverFlow);

            return water;

            float GetWeightedProperty(Dictionary<IWaterGetter, float> weights, Func<IWaterGetter, float> propertyGetter) {
                var total = weights.Values.Sum();
                return weights.Sum(pair => pair.Value / total * propertyGetter(pair.Key));
            }
        }
    }

    private static SKColor? GetAverageColor(SKBitmap bitmap, int x, int y, int gridSize) {
        var colors = GetPixelsAt(bitmap, x, y, gridSize)
            .Where(color => color.Alpha > 0 && !color.Equals(SKColors.White))
            .ToList();

        if (colors.Count == 0) return null;

        return colors.GetAverageColor();
    }

    private static IEnumerable<SKColor> GetPixelsAt(SKBitmap bitmap, int x, int y, int gridSize) =>
        Enumerable.Range(0, gridSize)
            .SelectMany(gridX => Enumerable.Range(0, gridSize)
                .Select(gridY => bitmap.GetPixel(x * gridSize + gridX, y * gridSize + gridY))
            );

    private static byte GetWeightedPropertyByte(Dictionary<IWaterGetter, float> waterWeights, Func<IWaterGetter, byte> propertyGetter) {
        return (byte) waterWeights.Sum(pair => pair.Value * propertyGetter(pair.Key));
    }

    private static Dictionary<IWaterGetter, float> GetWaterWeights(SKColor color, Dictionary<Color, IWaterGetter> waterPresets) {
        // Calculate the similarity score for each color
        var similarities = waterPresets.Select(x => CalculateSimilarity(color.ToSystemDrawingColor(), x.Key)).ToList();

        if (similarities.Exists(similarity => Math.Abs(similarity - 1) < 0.001)) {
            // If there is an exact match, set the similarity score to 1 for that color
            similarities = similarities.Select(similarity => Math.Abs(similarity - 1) < 0.001 ? 1.0 : 0.0).ToList();
        } else {
            // Normalize the similarity scores so they sum up to 1
            var total = similarities.Sum();
            if (total > 0) {
                similarities = similarities.Select(similarity => similarity / total).ToList();
            }
        }

        return similarities.Select((similarity, index) => (similarity, waterPresets.ElementAt(index).Value))
            .ToDictionary(x => x.Value, x => (float) x.similarity);
    }

    private static double CalculateSimilarity(Color c1, Color c2) {
        if (c1 == c2) return 1;

        // Calculate the Euclidean distance between two colors in RGB space
        double rDiff = c1.R - c2.R;
        double gDiff = c1.G - c2.G;
        double bDiff = c1.B - c2.B;
        var distance = Math.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);

        // If the distance is greater than 350, the colors are considered completely different
        if (distance > 350) return 0;

        const double maxDistance = 441.6729559300637; // sqrt(255^2 + 255^2 + 255^2)

        // Convert distance to similarity (1 = exact match, 0 = completely different)
        var similarity = 1 - distance / maxDistance;

        return similarity;
    }
}
