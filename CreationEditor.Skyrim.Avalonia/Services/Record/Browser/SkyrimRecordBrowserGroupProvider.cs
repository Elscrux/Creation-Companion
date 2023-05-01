using System.Collections.Generic;
using Autofac;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Browser;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser;

public sealed class SkyrimRecordBrowserGroupProvider : IRecordBrowserGroupProvider {
    private readonly ILifetimeScope _lifetimeScope;

    public SkyrimRecordBrowserGroupProvider(
        ILifetimeScope lifetimeScope) {
        _lifetimeScope = lifetimeScope;
    }

    public List<RecordTypeGroup> GetRecordGroups() {
        return new List<RecordTypeGroup> {
            new("Actors",
                new List<RecordTypeListing> {
                    new(_lifetimeScope, INpcGetter.StaticRegistration),
                    new(_lifetimeScope, IActionRecordGetter.StaticRegistration),
                    new(_lifetimeScope, IBodyPartDataGetter.StaticRegistration),
                    new(_lifetimeScope, ILeveledNpcGetter.StaticRegistration),
                    new(_lifetimeScope, IPerkGetter.StaticRegistration),
                    new(_lifetimeScope, ITalkingActivatorGetter.StaticRegistration),
                }),
            new("Audio",
                new List<RecordTypeListing> {
                    new(_lifetimeScope, IAcousticSpaceGetter.StaticRegistration),
                    new(_lifetimeScope, IMusicTrackGetter.StaticRegistration),
                    new(_lifetimeScope, IMusicTypeGetter.StaticRegistration),
                    new(_lifetimeScope, IReverbParametersGetter.StaticRegistration),
                    new(_lifetimeScope, ISoundCategoryGetter.StaticRegistration),
                    new(_lifetimeScope, ISoundDescriptorGetter.StaticRegistration),
                    new(_lifetimeScope, ISoundMarkerGetter.StaticRegistration),
                    new(_lifetimeScope, ISoundOutputModelGetter.StaticRegistration),
                }),
            new("Character",
                new List<RecordTypeListing> {
                    new(_lifetimeScope, IAssociationTypeGetter.StaticRegistration),
                    new(_lifetimeScope, IClassGetter.StaticRegistration),
                    new(_lifetimeScope, IEquipTypeGetter.StaticRegistration),
                    new(_lifetimeScope, IFactionGetter.StaticRegistration),
                    new(_lifetimeScope, IHeadPartGetter.StaticRegistration),
                    new(_lifetimeScope, IMovementTypeGetter.StaticRegistration),
                    new(_lifetimeScope, IPackageGetter.StaticRegistration),
                    new(_lifetimeScope, IQuestGetter.StaticRegistration),
                    new(_lifetimeScope, IRaceGetter.StaticRegistration),
                    new(_lifetimeScope, IRelationshipGetter.StaticRegistration),
                    new(_lifetimeScope, IStoryManagerEventNodeGetter.StaticRegistration, "Story Manager"),
                    new(_lifetimeScope, IVoiceTypeGetter.StaticRegistration),
                }),
            new("Items",
                new List<RecordTypeListing> {
                    new(_lifetimeScope, IAmmunitionGetter.StaticRegistration),
                    new(_lifetimeScope, IArmorGetter.StaticRegistration),
                    new(_lifetimeScope, IArmorAddonGetter.StaticRegistration),
                    new(_lifetimeScope, IBookGetter.StaticRegistration),
                    new(_lifetimeScope, IConstructibleObjectGetter.StaticRegistration),
                    new(_lifetimeScope, IIngredientGetter.StaticRegistration),
                    new(_lifetimeScope, IKeyGetter.StaticRegistration),
                    new(_lifetimeScope, ILeveledItemGetter.StaticRegistration),
                    new(_lifetimeScope, IMiscItemGetter.StaticRegistration),
                    new(_lifetimeScope, IOutfitGetter.StaticRegistration),
                    new(_lifetimeScope, ISoulGemGetter.StaticRegistration),
                    new(_lifetimeScope, IWeaponGetter.StaticRegistration),
                }),
            new("Magic",
                new List<RecordTypeListing> {
                    new(_lifetimeScope, IDualCastDataGetter.StaticRegistration),
                    new(_lifetimeScope, IObjectEffectGetter.StaticRegistration, "Enchantment"),
                    new(_lifetimeScope, ILeveledSpellGetter.StaticRegistration),
                    new(_lifetimeScope, IMagicEffectGetter.StaticRegistration),
                    new(_lifetimeScope, IIngestibleGetter.StaticRegistration),
                    new(_lifetimeScope, IScrollGetter.StaticRegistration),
                    new(_lifetimeScope, IShoutGetter.StaticRegistration),
                    new(_lifetimeScope, ISpellGetter.StaticRegistration),
                    new(_lifetimeScope, IWordOfPowerGetter.StaticRegistration),
                }),
            new("Miscellaneous",
                new List<RecordTypeListing> {
                    new(_lifetimeScope, IAnimatedObjectGetter.StaticRegistration),
                    new(_lifetimeScope, IArtObjectGetter.StaticRegistration),
                    new(_lifetimeScope, ICollisionLayerGetter.StaticRegistration),
                    new(_lifetimeScope, IColorRecordGetter.StaticRegistration, "Color"),
                    new(_lifetimeScope, ICombatStyleGetter.StaticRegistration),
                    new(_lifetimeScope, IFormListGetter.StaticRegistration),
                    new(_lifetimeScope, IGlobalGetter.StaticRegistration),
                    new(_lifetimeScope, IKeywordGetter.StaticRegistration),
                    new(_lifetimeScope, ILandscapeTextureGetter.StaticRegistration),
                    new(_lifetimeScope, ILoadScreenGetter.StaticRegistration),
                    new(_lifetimeScope, IMaterialObjectGetter.StaticRegistration),
                    new(_lifetimeScope, IMessageGetter.StaticRegistration),
                    new(_lifetimeScope, ITextureSetGetter.StaticRegistration),
                }),
            new("Special Effects",
                new List<RecordTypeListing> {
                    new(_lifetimeScope, IAddonNodeGetter.StaticRegistration),
                    new(_lifetimeScope, ICameraShotGetter.StaticRegistration),
                    new(_lifetimeScope, IDebrisGetter.StaticRegistration),
                    new(_lifetimeScope, IEffectShaderGetter.StaticRegistration),
                    new(_lifetimeScope, IExplosionGetter.StaticRegistration),
                    new(_lifetimeScope, IFootstepGetter.StaticRegistration),
                    new(_lifetimeScope, IFootstepSetGetter.StaticRegistration),
                    new(_lifetimeScope, IHazardGetter.StaticRegistration),
                    new(_lifetimeScope, IImageSpaceGetter.StaticRegistration, "Imagespace"),
                    new(_lifetimeScope, IImageSpaceAdapterGetter.StaticRegistration, "Imagespace Modifier"),
                    new(_lifetimeScope, IImpactGetter.StaticRegistration),
                    new(_lifetimeScope, IImpactDataSetGetter.StaticRegistration),
                    new(_lifetimeScope, IMaterialTypeGetter.StaticRegistration),
                    new(_lifetimeScope, IProjectileGetter.StaticRegistration),
                    new(_lifetimeScope, IVolumetricLightingGetter.StaticRegistration),
                }),
            new("World Data",
                new List<RecordTypeListing> {
                    new(_lifetimeScope, IClimateGetter.StaticRegistration),
                    new(_lifetimeScope, IEncounterZoneGetter.StaticRegistration),
                    new(_lifetimeScope, ILightingTemplateGetter.StaticRegistration),
                    new(_lifetimeScope, ILocationGetter.StaticRegistration),
                    new(_lifetimeScope, ILocationReferenceTypeGetter.StaticRegistration),
                    new(_lifetimeScope, IShaderParticleGeometryGetter.StaticRegistration),
                    new(_lifetimeScope, IVisualEffectGetter.StaticRegistration),
                    new(_lifetimeScope, IWaterGetter.StaticRegistration),
                    new(_lifetimeScope, IWeatherGetter.StaticRegistration),
                }),
            new("World Objects",
                new List<RecordTypeListing> {
                    new(_lifetimeScope, IActivatorGetter.StaticRegistration),
                    new(_lifetimeScope, IContainerGetter.StaticRegistration),
                    new(_lifetimeScope, IDoorGetter.StaticRegistration),
                    new(_lifetimeScope, IFloraGetter.StaticRegistration),
                    new(_lifetimeScope, IFurnitureGetter.StaticRegistration),
                    new(_lifetimeScope, IIdleMarkerGetter.StaticRegistration),
                    new(_lifetimeScope, IGrassGetter.StaticRegistration),
                    new(_lifetimeScope, ILightGetter.StaticRegistration),
                    new(_lifetimeScope, IMoveableStaticGetter.StaticRegistration),
                    new(_lifetimeScope, IStaticGetter.StaticRegistration),
                    new(_lifetimeScope, ITreeGetter.StaticRegistration),
                }),
        };
    }
}
