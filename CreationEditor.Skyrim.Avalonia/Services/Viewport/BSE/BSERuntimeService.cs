using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reactive.Subjects;
using CreationEditor.Avalonia.Services.Viewport;
using CreationEditor.Avalonia.Services.Viewport.BSE;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Serilog;
namespace CreationEditor.Skyrim.Avalonia.Services.Viewport.BSE;

public sealed class BSERuntimeService : IViewportRuntimeService, IDisposable {
    private sealed record WorldspaceRuntimeSettings(FormKey Worldspace, P2Int Origin, Dictionary<P2Int, ExteriorCellRuntimeSettings> LoadedCells);
    private sealed record InteriorCellRuntimeSettings(List<Interop.ReferenceLoad> References);
    private sealed record ExteriorCellRuntimeSettings(List<Interop.ReferenceLoad> References, Vector2 LocalOffset);

    private Interop.SelectCallback? _selectCallback;
    private readonly DisposableBucket _disposableDropoff = new();
    private readonly ILogger _logger;
    private readonly ILinkCacheProvider _linkCacheProvider;

    private const double UnloadCellGridDistance = 10;

    private FormKey _interiorCellKey = FormKey.Null;
    private InteriorCellRuntimeSettings? _interiorCell;

    private WorldspaceRuntimeSettings? _worldspaceRuntimeSettings;

    private readonly Subject<IList<FormKey>> _selectedReferences = new();
    public IObservable<IList<FormKey>> SelectedReferences => _selectedReferences;

    public BSERuntimeService(
        ILogger logger,
        ILinkCacheProvider linkCacheProvider,
        IViewportFactory viewportFactory) {
        _logger = logger;
        _linkCacheProvider = linkCacheProvider;

        viewportFactory.ViewportInitialized
            .Subscribe(_ => {
                SetupSelectionCallback();
            });
    }

    private void SetupSelectionCallback() {
        // Needs to be saved in variable to avoid garbage collection
        _selectCallback = (count, formKeyStrings) => {
            var convertedFormKeys = new List<FormKey>();

            foreach (var formKeyString in formKeyStrings.ToStringArray((int) count)) {
                var formKey = FormKey.TryFactory(formKeyString);
                if (formKey.HasValue) {
                    convertedFormKeys.Add(formKey.Value);
                } else {
                    _logger.Here().Error("Couldn't convert select {FormKey}", formKeyString);
                }
            }

            _logger.Here().Verbose("Selected {Count} references: {Refs}", count, string.Join(", ", convertedFormKeys));

            _selectedReferences.OnNext(convertedFormKeys);
        };

        Interop.addSelectCallback(_selectCallback);
    }

    public void LoadInteriorCell(ICellGetter cell) {
        if (_interiorCellKey != cell.FormKey) UnloadEverything();

        var placedObjects = cell.GetAllPlacedObjects(_linkCacheProvider.LinkCache);
        var cellReferences = LoadCellReferences(placedObjects, P2Int.Origin);
        _interiorCell = new InteriorCellRuntimeSettings(cellReferences);

        _worldspaceRuntimeSettings = null;
    }

    public void LoadExteriorCell(FormKey worldspaceFormKey, ICellGetter cell) {
        var origin = cell.Grid?.Point ?? P2Int.Origin;
        var editorCellOrigin = origin;

        if (_worldspaceRuntimeSettings is null
         || _worldspaceRuntimeSettings.Worldspace != worldspaceFormKey) {
            // Worldspace not loaded - unload everything
            UnloadEverything();

            _worldspaceRuntimeSettings = new WorldspaceRuntimeSettings(worldspaceFormKey, origin, new Dictionary<P2Int, ExteriorCellRuntimeSettings>());
        } else {
            // Worldspace loaded

            // Return if our cell is already loaded
            if (_worldspaceRuntimeSettings.LoadedCells.ContainsKey(origin)) return;

            // Unload cells outside range
            foreach (var (grid, references) in _worldspaceRuntimeSettings.LoadedCells) {
                if (UnloadCellGridDistance < grid.Distance(origin)) continue;

                UnloadCell(references);
                _worldspaceRuntimeSettings.LoadedCells.Remove(grid);
            }

            // Transform to actual editor cell origin
            editorCellOrigin -= _worldspaceRuntimeSettings.Origin;
        }

        var placedObjects = cell.GetAllPlacedObjects(_linkCacheProvider.LinkCache);
        var loadedReferences = LoadCellReferences(placedObjects, editorCellOrigin);
        if (cell.Landscape is not null) LoadLandscape(cell.Landscape, editorCellOrigin);
        var localOffset = new Vector2(editorCellOrigin.X, editorCellOrigin.Y);
        _worldspaceRuntimeSettings.LoadedCells.Add(origin, new ExteriorCellRuntimeSettings(loadedReferences, localOffset));

        _interiorCellKey = FormKey.Null;
    }

    private void UnloadEverything() {
        if (_worldspaceRuntimeSettings is not null) {
            var referenceLists = new List<List<Interop.ReferenceLoad>>();

            foreach (var (_, cell) in _worldspaceRuntimeSettings.LoadedCells) {
                referenceLists.Add(cell.References);
            }

            if (referenceLists.Count > 0) {
                var unloadReferences = referenceLists
                    .SelectMany(x => x)
                    .Select(x => x.FormKey)
                    .ToArray();

                Interop.deleteReferences(Convert.ToUInt32(unloadReferences.Length), unloadReferences);
            }

            _worldspaceRuntimeSettings = null;
        } else if (_interiorCell is not null) {
            UnloadCell(_interiorCell);

            _interiorCell = null;
        }
    }

    private void UnloadCell(ExteriorCellRuntimeSettings cell) {
        Interop.deleteReferences(Convert.ToUInt32(cell.References.Count), cell.References.Select(x => x.FormKey).ToArray());
    }

    private void UnloadCell(InteriorCellRuntimeSettings cell) {
        Interop.deleteReferences(Convert.ToUInt32(cell.References.Count), cell.References.Select(x => x.FormKey).ToArray());
    }

    private void LoadLandscape(ILandscapeGetter landscape, P2Int originToLoad) {
        const int cellPointSize = 33;
        const int normalsOffset = cellPointSize * cellPointSize;
        const int vertexColorOffset = normalsOffset * 4;
        const int landscapeBufferSize = normalsOffset * 7;

        if (landscape.VertexHeightMap is null) throw new ArgumentException(nameof(landscape.VertexHeightMap));
        if (landscape.VertexNormals is null) throw new ArgumentException(nameof(landscape.VertexNormals));
        if (landscape.VertexColors is null) throw new ArgumentException(nameof(landscape.VertexColors));

        var floatBuffer = new float[landscapeBufferSize];

        var counter = 0;
        foreach (var x in landscape.VertexHeightMap.HeightMap) {
            var height = (sbyte) x.Value;
            floatBuffer[counter] = height * 8 + landscape.VertexHeightMap.Offset;
            counter++;
        }

        foreach (var x in landscape.VertexNormals) {
            var xValue = (sbyte) x.Value.X;
            var yValue = (sbyte) x.Value.Y;
            var zValue = (sbyte) x.Value.Z;

            floatBuffer[counter] = xValue / 127f;
            counter++;
            floatBuffer[counter] = yValue / 127f;
            counter++;
            floatBuffer[counter] = zValue / 127f;
            counter++;
        }

        foreach (var x in landscape.VertexColors) {
            floatBuffer[counter] = x.Value.X / 255f;
            counter++;
            floatBuffer[counter] = x.Value.Y / 255f;
            counter++;
            floatBuffer[counter] = x.Value.Z / 255f;
            counter++;
        }

        var cornerSets = new Interop.CornerSets();
        var unmanagedMemory = new DisposableBucket();

        foreach (var grouping in landscape.Layers.GroupBy(x => x.Header?.Quadrant)) {
            ref var quadrant = ref cornerSets.BottomLeft;
            switch (grouping.Key) {
                case Quadrant.BottomLeft:
                    quadrant = ref cornerSets.BottomLeft;
                    break;
                case Quadrant.BottomRight:
                    quadrant = ref cornerSets.BottomRight;
                    break;
                case Quadrant.TopLeft:
                    quadrant = ref cornerSets.TopLeft;
                    break;
                case Quadrant.TopRight:
                    quadrant = ref cornerSets.TopRight;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var layers = grouping.ToArray();
            var alphaLayersCount = (byte) layers.Count(x => x is IAlphaLayerGetter);
            var alphaLayers = new Interop.AlphaLayer[alphaLayersCount];

            var alphaLayerCounter = 0;
            foreach (var layerRecord in layers) {
                if (layerRecord.Header is null) continue;

                var landscapeTextureRecord = layerRecord.Header.Texture.TryResolve(_linkCacheProvider.LinkCache);
                var textureSetRecord = landscapeTextureRecord?.TextureSet.TryResolve(_linkCacheProvider.LinkCache);

                if (layerRecord is IAlphaLayerGetter alphaLayerRecord) {
                    var alphaLayer = new Interop.AlphaLayer();
                    if (textureSetRecord is not null) {
                        alphaLayer.TextureSet = GetTextureSet(textureSetRecord);
                    }

                    if (alphaLayerRecord.AlphaLayerData.HasValue) {
                        var memoryData = alphaLayerRecord.AlphaLayerData.Value;
                        const int dataSize = sizeof(ushort) * 2 + sizeof(float);

                        var alphaDataLength = Convert.ToUInt16(memoryData.Length / dataSize);
                        var alphaData = new Interop.AlphaData[alphaDataLength];

                        for (var i = 0; i < alphaDataLength; i++) {
                            alphaData[i].Position = BitConverter.ToUInt16(memoryData[..sizeof(ushort)]);
                            alphaData[i].Opacity = BitConverter.ToSingle(memoryData.Slice(sizeof(ushort) * 2, sizeof(float)));

                            // Move the span to the next data set
                            memoryData = memoryData[dataSize..];
                        }

                        unmanagedMemory.Add(alphaData.ToUnmanagedMemory(out var pointer));
                        alphaLayer.Data = pointer;
                        alphaLayer.DataLength = alphaDataLength;
                    }

                    alphaLayers[alphaLayerCounter] = alphaLayer;
                    alphaLayerCounter++;
                } else {
                    if (textureSetRecord is not null) {
                        quadrant.BaseLayer = new Interop.BaseLayer {
                            TextureSet = GetTextureSet(textureSetRecord)
                        };
                    }
                }
            }

            unmanagedMemory.Add(alphaLayers.ToUnmanagedMemory(out var alphaLayersPointer));
            quadrant.AlphaLayers = alphaLayersPointer;
            quadrant.AlphaLayersLength = alphaLayersCount;
        }

        var landscapeInfo = new Interop.TerrainInfo {
            X = originToLoad.X,
            Y = originToLoad.Y,
            PointSize = cellPointSize,
            PositionBegin = 0,
            NormalBegin = normalsOffset,
            ColorBegin = vertexColorOffset,
            CornerSets = cornerSets,
        };

        // todo: pass multiple terrains, WARNING normal begin and color begin will be dynamic in that case!
        Interop.loadTerrain(1, new[] { landscapeInfo }, floatBuffer);

        unmanagedMemory.Dispose();
    }

    public static Interop.TextureSet GetTextureSet(ITextureSetGetter textureSet) {
        return new Interop.TextureSet {
            Diffuse = textureSet.Diffuse?.DataRelativePath,
            Normal = textureSet.NormalOrGloss?.DataRelativePath,
            Specular = textureSet.BacklightMaskOrSpecular?.DataRelativePath,
            EnvironmentMask = textureSet.Environment?.DataRelativePath,
            Height = textureSet.Height?.DataRelativePath,
            Environment = textureSet.Environment?.DataRelativePath,
            Multilayer = textureSet.Multilayer?.DataRelativePath,
            Emissive = textureSet.GlowOrDetailMap?.DataRelativePath,
        };
    }

    private List<Interop.ReferenceLoad> LoadCellReferences(IEnumerable<IPlacedGetter> placedRecords, P2Int gridPoint) {
        var origin = new P3Float(gridPoint.X, gridPoint.Y, 0);
        var refs = new List<Interop.ReferenceLoad>();

        foreach (var placed in placedRecords) {
            switch (placed) {
                case IPlacedArrowGetter:
                case IPlacedBarrierGetter:
                case IPlacedBeamGetter:
                case IPlacedConeGetter:
                case IPlacedFlameGetter:
                case IPlacedHazardGetter:
                case IPlacedMissileGetter:
                case IPlacedTrapGetter:
                case IAPlacedTrapGetter:
                case IPlacedNpcGetter:
                    break;
                case IPlacedObjectGetter placedObject:
                    var placement = placedObject.Placement;
                    if (placement is null) continue;

                    var relativePosition = placement.Position - origin;

                    var baseObject = placedObject.Base.Resolve(_linkCacheProvider.LinkCache);

                    var model = baseObject switch {
                        IModeledGetter modeled => modeled.Model?.File.DataRelativePath,
                        IArmorGetter armor => armor.WorldModel?.Male?.Model?.File.DataRelativePath,
                        ISoundMarkerGetter => null,
                        ISpellGetter => null,
                        ITextureSetGetter => null,
                        IAcousticSpaceGetter => null,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    if (model is null) continue;

                    var scale = placedObject.Scale ?? 1;

                    refs.Add(new Interop.ReferenceLoad {
                        FormKey = placedObject.FormKey.ToString(),
                        Path = model.ToLower(),
                        Transform = new Interop.ReferenceTransform {
                            Translation = relativePosition,
                            Scale = new P3Float(scale, scale, scale),
                            Rotations = placement.Rotation
                        }
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(placed));
            }
        }

        Interop.loadReferences(Convert.ToUInt32(refs.Count), refs.ToArray());

        return refs;
    }

    public void Dispose() => _disposableDropoff.Dispose();
}
