using System;
using System.Collections.Generic;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Browser;
using Loqui;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser;

public sealed class SkyrimRecordBrowserGroupProvider : IRecordBrowserGroupProvider {
    private readonly Func<ILoquiRegistration, string?, RecordTypeListing> _recordTypeListingFactory;

    public SkyrimRecordBrowserGroupProvider(
        Func<ILoquiRegistration, string?, RecordTypeListing> recordTypeListingFactory) {
        _recordTypeListingFactory = recordTypeListingFactory;
    }

    public List<RecordTypeGroup> GetRecordGroups() {
        return new List<RecordTypeGroup> {
            new("Actors",
                new List<RecordTypeListing> {
                    _recordTypeListingFactory(INpcGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IActionRecordGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IBodyPartDataGetter.StaticRegistration, null),
                    _recordTypeListingFactory(ILeveledNpcGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IPerkGetter.StaticRegistration, null),
                    _recordTypeListingFactory(ITalkingActivatorGetter.StaticRegistration, null),
                }),
            new("Audio",
                new List<RecordTypeListing> {
                    _recordTypeListingFactory(IAcousticSpaceGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IMusicTrackGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IMusicTypeGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IReverbParametersGetter.StaticRegistration, null),
                    _recordTypeListingFactory(ISoundCategoryGetter.StaticRegistration, null),
                    _recordTypeListingFactory(ISoundDescriptorGetter.StaticRegistration, null),
                    _recordTypeListingFactory(ISoundMarkerGetter.StaticRegistration, null),
                    _recordTypeListingFactory(ISoundOutputModelGetter.StaticRegistration, null),
                }),
            new("Character",
                new List<RecordTypeListing> {
                    _recordTypeListingFactory(IAssociationTypeGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IClassGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IEquipTypeGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IFactionGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IHeadPartGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IMovementTypeGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IPackageGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IQuestGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IRaceGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IRelationshipGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IStoryManagerEventNodeGetter.StaticRegistration, "Story Manager"),
                    _recordTypeListingFactory(IVoiceTypeGetter.StaticRegistration, null),
                }),
            new("Items",
                new List<RecordTypeListing> {
                    _recordTypeListingFactory(IAmmunitionGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IArmorGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IArmorAddonGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IBookGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IConstructibleObjectGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IIngredientGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IKeyGetter.StaticRegistration, null),
                    _recordTypeListingFactory(ILeveledItemGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IMiscItemGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IOutfitGetter.StaticRegistration, null),
                    _recordTypeListingFactory(ISoulGemGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IWeaponGetter.StaticRegistration, null),
                }),
            new("Magic",
                new List<RecordTypeListing> {
                    _recordTypeListingFactory(IDualCastDataGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IObjectEffectGetter.StaticRegistration, "Enchantment"),
                    _recordTypeListingFactory(ILeveledSpellGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IMagicEffectGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IIngestibleGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IScrollGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IShoutGetter.StaticRegistration, null),
                    _recordTypeListingFactory(ISpellGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IWordOfPowerGetter.StaticRegistration, null),
                }),
            new("Miscellaneous",
                new List<RecordTypeListing> {
                    _recordTypeListingFactory(IAnimatedObjectGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IArtObjectGetter.StaticRegistration, null),
                    _recordTypeListingFactory(ICollisionLayerGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IColorRecordGetter.StaticRegistration, "Color"),
                    _recordTypeListingFactory(ICombatStyleGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IFormListGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IGlobalGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IKeywordGetter.StaticRegistration, null),
                    _recordTypeListingFactory(ILandscapeTextureGetter.StaticRegistration, null),
                    _recordTypeListingFactory(ILoadScreenGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IMaterialObjectGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IMessageGetter.StaticRegistration, null),
                    _recordTypeListingFactory(ITextureSetGetter.StaticRegistration, null),
                }),
            new("Special Effects",
                new List<RecordTypeListing> {
                    _recordTypeListingFactory(IAddonNodeGetter.StaticRegistration, null),
                    _recordTypeListingFactory(ICameraShotGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IDebrisGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IEffectShaderGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IExplosionGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IFootstepGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IFootstepSetGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IHazardGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IImageSpaceGetter.StaticRegistration, "Imagespace"),
                    _recordTypeListingFactory(IImageSpaceAdapterGetter.StaticRegistration, "Imagespace Modifier"),
                    _recordTypeListingFactory(IImpactGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IImpactDataSetGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IMaterialTypeGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IProjectileGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IVolumetricLightingGetter.StaticRegistration, null),
                }),
            new("World Data",
                new List<RecordTypeListing> {
                    _recordTypeListingFactory(IClimateGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IEncounterZoneGetter.StaticRegistration, null),
                    _recordTypeListingFactory(ILightingTemplateGetter.StaticRegistration, null),
                    _recordTypeListingFactory(ILocationGetter.StaticRegistration, null),
                    _recordTypeListingFactory(ILocationReferenceTypeGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IShaderParticleGeometryGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IVisualEffectGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IWaterGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IWeatherGetter.StaticRegistration, null),
                }),
            new("World Objects",
                new List<RecordTypeListing> {
                    _recordTypeListingFactory(IActivatorGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IContainerGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IDoorGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IFloraGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IFurnitureGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IIdleMarkerGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IGrassGetter.StaticRegistration, null),
                    _recordTypeListingFactory(ILightGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IMoveableStaticGetter.StaticRegistration, null),
                    _recordTypeListingFactory(IStaticGetter.StaticRegistration, null),
                    _recordTypeListingFactory(ITreeGetter.StaticRegistration, null),
                }),
        };
    }
}
