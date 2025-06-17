using System.Numerics;
using System.Reactive.Subjects;
using CreationEditor.Avalonia.Services.Viewport;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ProjectBSE.Interop;
using Serilog;
using static ProjectBSE.Interop.Interop;
using Quadrant = Mutagen.Bethesda.Plugins.Records.Quadrant;
namespace CreationEditor.Skyrim.Avalonia.Services.Viewport.BSE;

public sealed class BSERuntimeService : IViewportRuntimeService, IDisposable {
    private sealed record WorldspaceRuntimeSettings(FormKey Worldspace, P2Int Origin, Dictionary<P2Int, ExteriorCellRuntimeSettings> LoadedCells);
    private sealed record InteriorCellRuntimeSettings(List<ReferenceLoad> References);
    private sealed record ExteriorCellRuntimeSettings(List<ReferenceLoad> References, Vector2 LocalOffset);

    private readonly DisposableBucket _disposableDropoff = new();
    private readonly ILogger _logger;
    private readonly ILinkCacheProvider _linkCacheProvider;

    private const double UnloadCellGridDistance = 10;

    private FormKey _interiorCellKey = FormKey.Null;
    private InteriorCellRuntimeSettings? _interiorCell;

    private WorldspaceRuntimeSettings? _worldspaceRuntimeSettings;

    private readonly Subject<IReadOnlyList<FormKey>> _selectedReferences = new();
    public IObservable<IReadOnlyList<FormKey>> SelectedReferences => _selectedReferences;

    public BSERuntimeService(
        ILogger logger,
        ILinkCacheProvider linkCacheProvider,
        IViewportFactory viewportFactory) {
        _logger = logger;
        _linkCacheProvider = linkCacheProvider;

        viewportFactory.ViewportInitialized
            .Subscribe(_ => {
                UpdateKeybindings(new Dictionary<IOFunction, IOFunctionBinding> {
                    { IOFunction.FreeForward, new IOFunctionBinding { Key = 'w', Type = IOFunctionBindingType.Keyboard } },
                    { IOFunction.FreeLeft, new IOFunctionBinding { Key = 'a', Type = IOFunctionBindingType.Keyboard } },
                    { IOFunction.FreeBackwards, new IOFunctionBinding { Key = 's', Type = IOFunctionBindingType.Keyboard } },
                    { IOFunction.FreeRight, new IOFunctionBinding { Key = 'd', Type = IOFunctionBindingType.Keyboard } },
                    { IOFunction.FreeDown, new IOFunctionBinding { Key = 'q', Type = IOFunctionBindingType.Keyboard } },
                    { IOFunction.FreeUp, new IOFunctionBinding { Key = 'e', Type = IOFunctionBindingType.Keyboard } },
                    { IOFunction.FreeSpeedAdd, new IOFunctionBinding { Key = 1, Type = IOFunctionBindingType.Scroll } },
                    { IOFunction.FreeSpeedReduce, new IOFunctionBinding { Key = -1, Type = IOFunctionBindingType.Scroll } },
                    { IOFunction.FreeReset, new IOFunctionBinding { Key = '-', Type = IOFunctionBindingType.Keyboard } },
                    { IOFunction.RotatingForward, new IOFunctionBinding { Key = 1, Type = IOFunctionBindingType.Scroll } },
                    { IOFunction.RotatingBackwards, new IOFunctionBinding { Key = -1, Type = IOFunctionBindingType.Scroll } },
                    { IOFunction.RotatingUp, new IOFunctionBinding { Key = 'q', Type = IOFunctionBindingType.Keyboard } },
                    { IOFunction.RotatingDown, new IOFunctionBinding { Key = 'e', Type = IOFunctionBindingType.Keyboard } },
                    { IOFunction.RotatingReset, new IOFunctionBinding { Key = '-', Type = IOFunctionBindingType.Keyboard } },
                    { IOFunction.RotatingSpeedAdd, new IOFunctionBinding { Key = '+', Type = IOFunctionBindingType.Keyboard } },
                    { IOFunction.RotatingSpeedReduce, new IOFunctionBinding { Key = '-', Type = IOFunctionBindingType.Keyboard } },
                    { IOFunction.MoveCamera, new IOFunctionBinding { Key = 0, Type = IOFunctionBindingType.Mouse } },
                    { IOFunction.Select, new IOFunctionBinding { Key = 1, Type = IOFunctionBindingType.Mouse } },
                    { IOFunction.MultiSelectModifier, new IOFunctionBinding { Key = 'y', Type = IOFunctionBindingType.Keyboard } },// How to set control key?
                });
                SetupSelectionCallback();
            });
    }

    private void SetupSelectionCallback() {
        // Needs to be saved in variable to avoid garbage collection
        AddSelectCallback(formKeys => {
            _logger.Here().Verbose("Selected {Count} references: {Refs}", formKeys.Length, string.Join(", ", formKeys));

            _selectedReferences.OnNext(formKeys);
        });
    }

    public void LoadInteriorCell(ICellGetter cell) {
        if (_interiorCellKey != cell.FormKey) UnloadEverything();

        var placedObjects = cell.GetAllPlaced(_linkCacheProvider.LinkCache);
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

            _worldspaceRuntimeSettings = new WorldspaceRuntimeSettings(
                worldspaceFormKey,
                origin,
                new Dictionary<P2Int, ExteriorCellRuntimeSettings>());
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

        var placedObjects = cell.GetAllPlaced(_linkCacheProvider.LinkCache);
        var loadedReferences = LoadCellReferences(placedObjects, editorCellOrigin);
        if (cell.Landscape is not null) LoadLandscape(cell.Landscape, editorCellOrigin);
        var localOffset = new Vector2(editorCellOrigin.X, editorCellOrigin.Y);
        _worldspaceRuntimeSettings.LoadedCells.Add(origin, new ExteriorCellRuntimeSettings(loadedReferences, localOffset));

        _interiorCellKey = FormKey.Null;
    }

    private void UnloadEverything() {
        if (_worldspaceRuntimeSettings is not null) {
            var referenceLists = new List<List<ReferenceLoad>>();

            foreach (var (_, cell) in _worldspaceRuntimeSettings.LoadedCells) {
                referenceLists.Add(cell.References);
            }

            if (referenceLists.Count > 0) {
                var unloadReferences = referenceLists
                    .SelectMany(x => x)
                    .Select(x => x.FormKey)
                    .ToArray();

                DeleteReferences(unloadReferences);
            }

            _worldspaceRuntimeSettings = null;
        } else if (_interiorCell is not null) {
            UnloadCell(_interiorCell);

            _interiorCell = null;
        }
    }

    private void UnloadCell(ExteriorCellRuntimeSettings cell) {
        DeleteReferences(cell.References.Select(x => x.FormKey).ToArray());
    }

    private void UnloadCell(InteriorCellRuntimeSettings cell) {
        DeleteReferences(cell.References.Select(x => x.FormKey).ToArray());
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

        foreach (var value in landscape.VertexNormals.Values()) {
            var xValue = (sbyte) value.X;
            var yValue = (sbyte) value.Y;
            var zValue = (sbyte) value.Z;

            floatBuffer[counter] = xValue / 127f;
            counter++;
            floatBuffer[counter] = yValue / 127f;
            counter++;
            floatBuffer[counter] = zValue / 127f;
            counter++;
        }

        foreach (var value in landscape.VertexColors.Values()) {
            floatBuffer[counter] = value.X / 255f;
            counter++;
            floatBuffer[counter] = value.Y / 255f;
            counter++;
            floatBuffer[counter] = value.Z / 255f;
            counter++;
        }

        var cornerSets = new CornerSets();
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
                    throw new InvalidOperationException();
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
                    if (alphaLayerRecord.AlphaLayerData is null) {
                        alphaLayers[alphaLayerCounter] = new Interop.AlphaLayer();
                    } else {
                        var alphaData = alphaLayerRecord.AlphaLayerData
                            .Select(x => new AlphaData {
                                Opacity = x.Opacity,
                                Position = x.Position,
                            })
                            .ToArray();

                        var alphaLayer = new Interop.AlphaLayer {
                            Data = alphaData,
                            DataLength = (ushort) alphaData.Length,
                        };
                        if (textureSetRecord is not null) {
                            alphaLayer.TextureSet = GetTextureSet(textureSetRecord);
                        }

                        alphaLayers[alphaLayerCounter] = alphaLayer;
                    }

                    alphaLayerCounter++;
                } else {
                    if (textureSetRecord is not null) {
                        quadrant.BaseLayer = new Interop.BaseLayer {
                            TextureSet = GetTextureSet(textureSetRecord),
                        };
                    }
                }
            }

            quadrant.AlphaLayers = alphaLayers;
        }

        var landscapeInfo = new TerrainInfo {
            X = originToLoad.X,
            Y = originToLoad.Y,
            PointSize = cellPointSize,
            PositionBegin = 0,
            NormalBegin = normalsOffset,
            ColorBegin = vertexColorOffset,
            CornerSets = cornerSets,
        };

        // todo: pass multiple terrains, WARNING normal begin and color begin will be dynamic in that case!
        LoadTerrain([landscapeInfo], floatBuffer);

        unmanagedMemory.Dispose();
    }

    public static Interop.TextureSet GetTextureSet(ITextureSetGetter textureSet) {
        return new Interop.TextureSet {
            Diffuse = textureSet.Diffuse?.DataRelativePath.Path,
            Normal = textureSet.NormalOrGloss?.DataRelativePath.Path,
            Specular = textureSet.BacklightMaskOrSpecular?.DataRelativePath.Path,
            EnvironmentMask = textureSet.Environment?.DataRelativePath.Path,
            Height = textureSet.Height?.DataRelativePath.Path,
            Environment = textureSet.Environment?.DataRelativePath.Path,
            Multilayer = textureSet.Multilayer?.DataRelativePath.Path,
            Emissive = textureSet.GlowOrDetailMap?.DataRelativePath.Path,
        };
    }

    private List<ReferenceLoad> LoadCellReferences(IEnumerable<IPlacedGetter> placedRecords, P2Int gridPoint) {
        var origin = new P3Float(gridPoint.X, gridPoint.Y, 0);
        var refs = new List<ReferenceLoad>();

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
                        _ => throw new InvalidOperationException(),
                    };

                    if (!model.HasValue) continue;

                    var scale = placedObject.Scale ?? 1;

                    refs.Add(new ReferenceLoad {
                        FormKey = placedObject.FormKey,
                        Path = model.Value.Path.ToLower(),
                        Transform = new ReferenceTransform {
                            Translation = relativePosition,
                            Scale = new P3Float(scale, scale, scale),
                            Rotations = placement.Rotation,
                        },
                    });
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        LoadReferences(refs.ToArray());

        return refs;
    }

    public void Dispose() => _disposableDropoff.Dispose();
}
