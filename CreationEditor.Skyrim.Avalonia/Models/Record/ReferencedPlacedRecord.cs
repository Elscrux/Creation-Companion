using System;
using CreationEditor.Services.Mutagen.References.Record;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.Models.Record;

public sealed class ReferencedPlacedRecord : ReactiveObject, IReferencedRecord<IPlacedGetter> {
    private readonly IReferencedRecord<IPlacedGetter> _referencedRecord;

    public IMajorRecordIdentifier? Base { get; }

    public ReferencedPlacedRecord(IReferencedRecord<IPlacedGetter> referencedRecord, ILinkCache linkCache) {
        _referencedRecord = referencedRecord;

        Base = _referencedRecord.Record switch {
            IPlacedObjectGetter placedObject => placedObject.Base.TryResolve(linkCache),
            IPlacedNpcGetter placedNpc => placedNpc.Base.TryResolve(linkCache),
            IPlacedArrowGetter placedArrow => placedArrow.Projectile.TryResolve(linkCache),
            IPlacedBarrierGetter placedBarrier => placedBarrier.Projectile.TryResolve(linkCache),
            IPlacedBeamGetter placedBeam => placedBeam.Projectile.TryResolve(linkCache),
            IPlacedConeGetter placedCone => placedCone.Projectile.TryResolve(linkCache),
            IPlacedFlameGetter placedFlame => placedFlame.Projectile.TryResolve(linkCache),
            IPlacedHazardGetter placedHazard => placedHazard.Hazard.TryResolve(linkCache),
            IPlacedMissileGetter placedMissile => placedMissile.Projectile.TryResolve(linkCache),
            IPlacedTrapGetter placedTrap => placedTrap.Projectile.TryResolve(linkCache),
            _ => throw new ArgumentOutOfRangeException(nameof(_referencedRecord.Record))
        };
    }

    public IObservableCollection<IFormLinkIdentifier> References => _referencedRecord.References;
    public IPlacedGetter Record {
        get => _referencedRecord.Record;
        set => _referencedRecord.Record = value;
    }
}
