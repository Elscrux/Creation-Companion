using System.Collections.Generic;
using Autofac;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Browser;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser;

public class SkyrimRecordBrowserGroupProvider : IRecordBrowserGroupProvider {
    private readonly IComponentContext _componentContext;

    public SkyrimRecordBrowserGroupProvider(
        IComponentContext componentContext) {
        _componentContext = componentContext;

    }

    public List<RecordTypeGroup> GetRecordGroups() {
        return new List<RecordTypeGroup> {
            new("Actors",
                new List<RecordTypeListing> {
                    new(_componentContext, INpcGetter.StaticRegistration),
                    new(_componentContext, IActionRecordGetter.StaticRegistration),
                    new(_componentContext, IBodyPartDataGetter.StaticRegistration),
                    new(_componentContext, ILeveledNpcGetter.StaticRegistration),
                    new(_componentContext, IPerkGetter.StaticRegistration),
                    new(_componentContext, ITalkingActivatorGetter.StaticRegistration),
                }),
            new("Audio",
                new List<RecordTypeListing> {
                    new(_componentContext, IAcousticSpaceGetter.StaticRegistration),
                    new(_componentContext, IMusicTrackGetter.StaticRegistration),
                    new(_componentContext, IMusicTypeGetter.StaticRegistration),
                    new(_componentContext, IReverbParametersGetter.StaticRegistration),
                    new(_componentContext, ISoundCategoryGetter.StaticRegistration),
                    new(_componentContext, ISoundDescriptorGetter.StaticRegistration),
                    new(_componentContext, ISoundMarkerGetter.StaticRegistration),
                    new(_componentContext, ISoundOutputModelGetter.StaticRegistration),
                }),
            new("Character",
                new List<RecordTypeListing> {
                    new(_componentContext, IAssociationTypeGetter.StaticRegistration),
                    new(_componentContext, IClassGetter.StaticRegistration),
                    new(_componentContext, IEquipTypeGetter.StaticRegistration),
                    new(_componentContext, IFactionGetter.StaticRegistration),
                    new(_componentContext, IHeadPartGetter.StaticRegistration),
                    new(_componentContext, IMovementTypeGetter.StaticRegistration),
                    new(_componentContext, IPackageGetter.StaticRegistration),
                    new(_componentContext, IQuestGetter.StaticRegistration),
                    new(_componentContext, IRaceGetter.StaticRegistration),
                    new(_componentContext, IRelationshipGetter.StaticRegistration),
                    new(_componentContext, IStoryManagerEventNodeGetter.StaticRegistration, "Story Manager"),
                    new(_componentContext, IVoiceTypeGetter.StaticRegistration),
                }),
            new("Items",
                new List<RecordTypeListing> {
                    new(_componentContext, IAmmunitionGetter.StaticRegistration),
                    new(_componentContext, IArmorGetter.StaticRegistration),
                    new(_componentContext, IArmorAddonGetter.StaticRegistration),
                    new(_componentContext, IBookGetter.StaticRegistration),
                    new(_componentContext, IConstructibleObjectGetter.StaticRegistration),
                    new(_componentContext, IIngredientGetter.StaticRegistration),
                    new(_componentContext, IKeyGetter.StaticRegistration),
                    new(_componentContext, ILeveledItemGetter.StaticRegistration),
                    new(_componentContext, IMiscItemGetter.StaticRegistration),
                    new(_componentContext, IOutfitGetter.StaticRegistration),
                    new(_componentContext, ISoulGemGetter.StaticRegistration),
                    new(_componentContext, IWeaponGetter.StaticRegistration),
                }),
            new("Magic",
                new List<RecordTypeListing> {
                    new(_componentContext, IDualCastDataGetter.StaticRegistration),
                    new(_componentContext, IObjectEffectGetter.StaticRegistration, "Enchantment"),
                    new(_componentContext, ILeveledSpellGetter.StaticRegistration),
                    new(_componentContext, IMagicEffectGetter.StaticRegistration),
                    new(_componentContext, IIngestibleGetter.StaticRegistration),
                    new(_componentContext, IScrollGetter.StaticRegistration),
                    new(_componentContext, IShoutGetter.StaticRegistration),
                    new(_componentContext, ISpellGetter.StaticRegistration),
                    new(_componentContext, IWordOfPowerGetter.StaticRegistration),
                }),
            new("Miscellaneous",
                new List<RecordTypeListing> {
                    new(_componentContext, IAnimatedObjectGetter.StaticRegistration),
                    new(_componentContext, IArtObjectGetter.StaticRegistration),
                    new(_componentContext, ICollisionLayerGetter.StaticRegistration),
                    new(_componentContext, IColorRecordGetter.StaticRegistration, "Color"),
                    new(_componentContext, ICombatStyleGetter.StaticRegistration),
                    new(_componentContext, IFormListGetter.StaticRegistration),
                    new(_componentContext, IGlobalGetter.StaticRegistration),
                    new(_componentContext, IKeywordGetter.StaticRegistration),
                    new(_componentContext, ILandscapeTextureGetter.StaticRegistration),
                    new(_componentContext, ILoadScreenGetter.StaticRegistration),
                    new(_componentContext, IMaterialObjectGetter.StaticRegistration),
                    new(_componentContext, IMessageGetter.StaticRegistration),
                    new(_componentContext, ITextureSetGetter.StaticRegistration),
                }),
            new("Special Effects",
                new List<RecordTypeListing> {
                    new(_componentContext, IAddonNodeGetter.StaticRegistration),
                    new(_componentContext, ICameraShotGetter.StaticRegistration),
                    new(_componentContext, IDebrisGetter.StaticRegistration),
                    new(_componentContext, IEffectShaderGetter.StaticRegistration),
                    new(_componentContext, IExplosionGetter.StaticRegistration),
                    new(_componentContext, IFootstepGetter.StaticRegistration),
                    new(_componentContext, IFootstepSetGetter.StaticRegistration),
                    new(_componentContext, IHazardGetter.StaticRegistration),
                    new(_componentContext, IImageSpaceGetter.StaticRegistration, "Imagespace"),
                    new(_componentContext, IImageSpaceAdapterGetter.StaticRegistration, "Imagespace Modifier"),
                    new(_componentContext, IImpactGetter.StaticRegistration),
                    new(_componentContext, IImpactDataSetGetter.StaticRegistration),
                    new(_componentContext, IMaterialTypeGetter.StaticRegistration),
                    new(_componentContext, IProjectileGetter.StaticRegistration),
                    new(_componentContext, IVolumetricLightingGetter.StaticRegistration),
                }),
            new("World Data",
                new List<RecordTypeListing> {
                    new(_componentContext, IClimateGetter.StaticRegistration),
                    new(_componentContext, IEncounterZoneGetter.StaticRegistration),
                    new(_componentContext, ILightingTemplateGetter.StaticRegistration),
                    new(_componentContext, ILocationGetter.StaticRegistration),
                    new(_componentContext, ILocationReferenceTypeGetter.StaticRegistration),
                    new(_componentContext, IShaderParticleGeometryGetter.StaticRegistration),
                    new(_componentContext, IVisualEffectGetter.StaticRegistration),
                    new(_componentContext, IWaterGetter.StaticRegistration),
                    new(_componentContext, IWeatherGetter.StaticRegistration),
                }),
            new("World Objects",
                new List<RecordTypeListing> {
                    new(_componentContext, IActivatorGetter.StaticRegistration),
                    new(_componentContext, IContainerGetter.StaticRegistration),
                    new(_componentContext, IDoorGetter.StaticRegistration),
                    new(_componentContext, IFloraGetter.StaticRegistration),
                    new(_componentContext, IFurnitureGetter.StaticRegistration),
                    new(_componentContext, IIdleMarkerGetter.StaticRegistration),
                    new(_componentContext, IGrassGetter.StaticRegistration),
                    new(_componentContext, ILightGetter.StaticRegistration),
                    new(_componentContext, IMoveableStaticGetter.StaticRegistration),
                    new(_componentContext, IStaticGetter.StaticRegistration),
                    new(_componentContext, ITreeGetter.StaticRegistration),
                }),
        };
    }
}
