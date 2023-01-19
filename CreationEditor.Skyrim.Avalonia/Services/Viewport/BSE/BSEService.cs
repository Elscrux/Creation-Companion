using System;
using System.Collections.Generic;
using CreationEditor.Avalonia.Services.Viewport.BSE;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.Services.Viewport.BSE;

public interface IViewportRuntimeService {
    public void Load(IEnumerable<IPlacedGetter> placedRecords, P2Int gridPoint);
    public void Load(IEnumerable<IPlacedGetter> placedRecords, P3Float origin);
}

public class BSERuntimeService : IViewportRuntimeService {
    private readonly IEditorEnvironment _editorEnvironment;

    public Dictionary<FormKey, Interop.ReferenceLoad> Loaded { get; } = new();

    public BSERuntimeService(
        IEditorEnvironment editorEnvironment) {
        _editorEnvironment = editorEnvironment;
    }
    
    public void Load(IEnumerable<IPlacedGetter> placedRecords, P2Int gridPoint) {
        Load(placedRecords, new P3Float(gridPoint.X, gridPoint.Y, 0));
    }
    
    public void Load(IEnumerable<IPlacedGetter> placedRecords, P3Float origin) {
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
                    if (placement == null) return;
        
                    var relativePosition = placement.Position - origin;
        
                    var baseObject = placedObject.Base.Resolve(_editorEnvironment.LinkCache);
        
                    var model = baseObject switch {
                        IModeledGetter modeled => modeled.Model?.File.DataRelativePath,
                        IArmorGetter armor => armor.WorldModel.Male.Model?.File.DataRelativePath,
                        ISoundMarkerGetter soundMarker => null,
                        ISpellGetter spell => null,
                        ITextureSetGetter textureSet => null,
                        IAcousticSpaceGetter acousticSpaceGetter => null,
                        _ => throw new ArgumentOutOfRangeException(nameof(baseObject))
                    };
        
                    if (model == null) continue;
        
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
    }
}
