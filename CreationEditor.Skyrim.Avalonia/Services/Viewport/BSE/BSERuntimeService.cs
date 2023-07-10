using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using CreationEditor.Avalonia.Services.Viewport;
using CreationEditor.Avalonia.Services.Viewport.BSE;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Serilog;
namespace CreationEditor.Skyrim.Avalonia.Services.Viewport.BSE;

public sealed class BSERuntimeService : IViewportRuntimeService, IDisposable {
    private sealed record WorldspaceRuntimeSettings(FormKey Worldspace, P2Int Origin, Dictionary<P2Int, List<Interop.ReferenceLoad>> LoadedCells);

    private Interop.SelectCallback? _selectCallback;
    private readonly IDisposableDropoff _disposableDropoff = new DisposableBucket();
    private readonly ILogger _logger;
    private readonly IEditorEnvironment _editorEnvironment;

    private const double UnloadCellGridDistance = 10;

    private FormKey _interiorCell = FormKey.Null;
    private List<Interop.ReferenceLoad>? _interiorCellReferences;

    private WorldspaceRuntimeSettings? _worldspaceRuntimeSettings;

    private readonly Subject<IList<FormKey>> _selectedReferences = new();
    public IObservable<IList<FormKey>> SelectedReferences => _selectedReferences;

    public BSERuntimeService(
        ILogger logger,
        IEditorEnvironment editorEnvironment,
        IViewportFactory viewportFactory) {
        _logger = logger;
        _editorEnvironment = editorEnvironment;

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
        if (_interiorCell != cell.FormKey) UnloadEverything();

        var placedObjects = cell.GetAllPlacedObjects(_editorEnvironment.LinkCache);
        _interiorCellReferences = Load(placedObjects, P2Int.Origin);

        _worldspaceRuntimeSettings = null;
    }

    public void LoadExteriorCell(FormKey worldspaceFormKey, ICellGetter cell) {
        var origin = cell.Grid?.Point ?? P2Int.Origin;
        var originToLoad = origin;

        if (_worldspaceRuntimeSettings is null
         || _worldspaceRuntimeSettings.Worldspace != worldspaceFormKey) {
            // Worldspace not loaded - unload everything
            UnloadEverything();

            _worldspaceRuntimeSettings = new WorldspaceRuntimeSettings(worldspaceFormKey, origin, new Dictionary<P2Int, List<Interop.ReferenceLoad>>());
        } else {
            // Worldspace loaded

            // Return if our cell is already loaded
            if (_worldspaceRuntimeSettings.LoadedCells.ContainsKey(origin)) return;

            // Unload cells outside range
            foreach (var (grid, references) in _worldspaceRuntimeSettings.LoadedCells) {
                if (UnloadCellGridDistance < grid.Distance(origin)) continue;

                Unload(references);
                _worldspaceRuntimeSettings.LoadedCells.Remove(grid);
            }

            // Transform origin
            originToLoad -= _worldspaceRuntimeSettings.Origin;
        }

        var placedObjects = cell.GetAllPlacedObjects(_editorEnvironment.LinkCache);
        var loadedReferences = Load(placedObjects, originToLoad);
        _worldspaceRuntimeSettings.LoadedCells.Add(origin, loadedReferences);

        _interiorCell = FormKey.Null;
    }

    private void UnloadEverything() {
        if (_worldspaceRuntimeSettings is not null) {
            var referenceLists = new List<List<Interop.ReferenceLoad>>();

            foreach (var (_, references) in _worldspaceRuntimeSettings.LoadedCells) {
                referenceLists.Add(references);
            }

            if (referenceLists.Count > 0) {
                var unloadReferences = referenceLists
                    .SelectMany(x => x)
                    .Select(x => x.FormKey)
                    .ToArray();

                Interop.deleteReferences(Convert.ToUInt32(unloadReferences.Length), unloadReferences);
            }

            _worldspaceRuntimeSettings = null;
        } else if (_interiorCellReferences is not null) {
            Unload(_interiorCellReferences);

            _interiorCellReferences = null;
        }
    }

    private void Unload(List<Interop.ReferenceLoad> references) {
        Interop.deleteReferences(Convert.ToUInt32(references.Count), references.Select(x => x.FormKey).ToArray());
    }

    private List<Interop.ReferenceLoad> Load(IEnumerable<IPlacedGetter> placedRecords, P2Int gridPoint) {
        var origin = new P3Float(gridPoint.X, gridPoint.Y, 0);
        var refs = new List<Interop.ReferenceLoad>();

        foreach (var placed in placedRecords) {
            switch (placed) {
                case IPlacedArrowGetter placedArrow:
                case IPlacedBarrierGetter placedBarrier:
                case IPlacedBeamGetter placedBeam:
                case IPlacedConeGetter placedCone:
                case IPlacedFlameGetter placedFlame:
                case IPlacedHazardGetter placedHazard:
                case IPlacedMissileGetter placedMissile:
                case IPlacedTrapGetter placedTrap:
                case IAPlacedTrapGetter aPlacedTrap:
                case IPlacedNpcGetter placedNpc:
                    break;
                case IPlacedObjectGetter placedObject:
                    var placement = placedObject.Placement;
                    if (placement is null) continue;

                    var relativePosition = placement.Position - origin;

                    var baseObject = placedObject.Base.Resolve(_editorEnvironment.LinkCache);

                    var model = baseObject switch {
                        IModeledGetter modeled => modeled.Model?.File.DataRelativePath,
                        IArmorGetter armor => armor.WorldModel?.Male?.Model?.File.DataRelativePath,
                        ISoundMarkerGetter soundMarker => null,
                        ISpellGetter spell => null,
                        ITextureSetGetter textureSet => null,
                        IAcousticSpaceGetter acousticSpaceGetter => null,
                        _ => throw new ArgumentOutOfRangeException(nameof(baseObject))
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
