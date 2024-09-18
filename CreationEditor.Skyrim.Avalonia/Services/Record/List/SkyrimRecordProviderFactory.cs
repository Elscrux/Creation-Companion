using Autofac;
using CreationEditor.Avalonia.Services.Record.Provider;
using CreationEditor.Services.Filter;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Activator = Mutagen.Bethesda.Skyrim.Activator;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.List;

public sealed class SkyrimRecordProviderFactory(
    IComponentContext componentContext,
    Func<IEnumerable<IFormLinkIdentifier>, IRecordBrowserSettings?, RecordIdentifiersProvider> recordIdentifiersProviderFactory,
    IRecordBrowserSettings defaultRecordBrowserSettings)
    : IRecordProviderFactory {

    public IRecordProvider FromIdentifiers(IEnumerable<IFormLinkIdentifier> identifiers, IRecordBrowserSettings? browserSettings = null) {
        browserSettings ??= defaultRecordBrowserSettings;
        return recordIdentifiersProviderFactory(identifiers, browserSettings);
    }

    public IRecordProvider FromType(Type type, IRecordBrowserSettings? browserSettings = null) {
        browserSettings ??= defaultRecordBrowserSettings;
        var browserSettingsParam = TypedParameter.From(browserSettings);

        return type.Name switch {
            nameof(INpcGetter)
                => componentContext.Resolve<RecordProvider<Npc, INpcGetter>>(browserSettingsParam),
            nameof(IActionRecordGetter)
                => componentContext.Resolve<RecordProvider<ActionRecord, IActionRecordGetter>>(browserSettingsParam),
            nameof(IBodyPartDataGetter)
                => componentContext.Resolve<RecordProvider<BodyPartData, IBodyPartDataGetter>>(browserSettingsParam),
            nameof(ILeveledNpcGetter)
                => componentContext.Resolve<RecordProvider<LeveledNpc, ILeveledNpcGetter>>(browserSettingsParam),
            nameof(IPerkGetter)
                => componentContext.Resolve<RecordProvider<Perk, IPerkGetter>>(browserSettingsParam),
            nameof(ITalkingActivatorGetter)
                => componentContext.Resolve<RecordProvider<TalkingActivator, ITalkingActivatorGetter>>(browserSettingsParam),

            nameof(IAcousticSpaceGetter)
                => componentContext.Resolve<RecordProvider<AcousticSpace, IAcousticSpaceGetter>>(browserSettingsParam),
            nameof(IMusicTrackGetter)
                => componentContext.Resolve<RecordProvider<MusicTrack, IMusicTrackGetter>>(browserSettingsParam),
            nameof(IMusicTypeGetter)
                => componentContext.Resolve<RecordProvider<MusicType, IMusicTypeGetter>>(browserSettingsParam),
            nameof(IReverbParametersGetter)
                => componentContext.Resolve<RecordProvider<ReverbParameters, IReverbParametersGetter>>(browserSettingsParam),
            nameof(ISoundCategoryGetter)
                => componentContext.Resolve<RecordProvider<SoundCategory, ISoundCategoryGetter>>(browserSettingsParam),
            nameof(ISoundDescriptorGetter)
                => componentContext.Resolve<RecordProvider<SoundDescriptor, ISoundDescriptorGetter>>(browserSettingsParam),
            nameof(ISoundMarkerGetter)
                => componentContext.Resolve<RecordProvider<SoundMarker, ISoundMarkerGetter>>(browserSettingsParam),
            nameof(ISoundOutputModelGetter)
                => componentContext.Resolve<RecordProvider<SoundOutputModel, ISoundOutputModelGetter>>(browserSettingsParam),

            nameof(IAssociationTypeGetter)
                => componentContext.Resolve<RecordProvider<AssociationType, IAssociationTypeGetter>>(browserSettingsParam),
            nameof(IClassGetter)
                => componentContext.Resolve<RecordProvider<Class, IClassGetter>>(browserSettingsParam),
            nameof(IEquipTypeGetter)
                => componentContext.Resolve<RecordProvider<EquipType, IEquipTypeGetter>>(browserSettingsParam),
            nameof(IFactionGetter)
                => componentContext.Resolve<RecordProvider<Faction, IFactionGetter>>(browserSettingsParam),
            nameof(IHeadPartGetter)
                => componentContext.Resolve<RecordProvider<HeadPart, IHeadPartGetter>>(browserSettingsParam),
            nameof(IMovementTypeGetter)
                => componentContext.Resolve<RecordProvider<MovementType, IMovementTypeGetter>>(browserSettingsParam),
            nameof(IPackageGetter)
                => componentContext.Resolve<RecordProvider<Package, IPackageGetter>>(browserSettingsParam),
            nameof(IQuestGetter)
                => componentContext.Resolve<RecordProvider<Quest, IQuestGetter>>(browserSettingsParam),
            nameof(IRaceGetter)
                => componentContext.Resolve<RecordProvider<Race, IRaceGetter>>(browserSettingsParam),
            nameof(IRelationshipGetter)
                => componentContext.Resolve<RecordProvider<Relationship, IRelationshipGetter>>(browserSettingsParam),
            nameof(IStoryManagerEventNodeGetter)
                => componentContext.Resolve<RecordProvider<StoryManagerEventNode, IStoryManagerEventNodeGetter>>(browserSettingsParam),
            nameof(IVoiceTypeGetter)
                => componentContext.Resolve<RecordProvider<VoiceType, IVoiceTypeGetter>>(browserSettingsParam),

            nameof(IAmmunitionGetter)
                => componentContext.Resolve<RecordProvider<Ammunition, IAmmunitionGetter>>(browserSettingsParam),
            nameof(IArmorGetter)
                => componentContext.Resolve<RecordProvider<Armor, IArmorGetter>>(browserSettingsParam),
            nameof(IArmorAddonGetter)
                => componentContext.Resolve<RecordProvider<ArmorAddon, IArmorAddonGetter>>(browserSettingsParam),
            nameof(IBookGetter)
                => componentContext.Resolve<RecordProvider<Book, IBookGetter>>(browserSettingsParam),
            nameof(IConstructibleObjectGetter)
                => componentContext.Resolve<RecordProvider<ConstructibleObject, IConstructibleObjectGetter>>(browserSettingsParam),
            nameof(IIngredientGetter)
                => componentContext.Resolve<RecordProvider<Ingredient, IIngredientGetter>>(browserSettingsParam),
            nameof(IKeyGetter)
                => componentContext.Resolve<RecordProvider<Key, IKeyGetter>>(browserSettingsParam),
            nameof(ILeveledItemGetter)
                => componentContext.Resolve<RecordProvider<LeveledItem, ILeveledItemGetter>>(browserSettingsParam),
            nameof(IMiscItemGetter)
                => componentContext.Resolve<RecordProvider<MiscItem, IMiscItemGetter>>(browserSettingsParam),
            nameof(IOutfitGetter)
                => componentContext.Resolve<RecordProvider<Outfit, IOutfitGetter>>(browserSettingsParam),
            nameof(ISoulGemGetter)
                => componentContext.Resolve<RecordProvider<SoulGem, ISoulGemGetter>>(browserSettingsParam),
            nameof(IWeaponGetter)
                => componentContext.Resolve<RecordProvider<Weapon, IWeaponGetter>>(browserSettingsParam),

            nameof(IDualCastDataGetter)
                => componentContext.Resolve<RecordProvider<DualCastData, IDualCastDataGetter>>(browserSettingsParam),
            nameof(IObjectEffectGetter)
                => componentContext.Resolve<RecordProvider<ObjectEffect, IObjectEffectGetter>>(browserSettingsParam),
            nameof(ILeveledSpellGetter)
                => componentContext.Resolve<RecordProvider<LeveledSpell, ILeveledSpellGetter>>(browserSettingsParam),
            nameof(IMagicEffectGetter)
                => componentContext.Resolve<RecordProvider<MagicEffect, IMagicEffectGetter>>(browserSettingsParam),
            nameof(IIngestibleGetter)
                => componentContext.Resolve<RecordProvider<Ingestible, IIngestibleGetter>>(browserSettingsParam),
            nameof(IScrollGetter)
                => componentContext.Resolve<RecordProvider<Scroll, IScrollGetter>>(browserSettingsParam),
            nameof(IShoutGetter)
                => componentContext.Resolve<RecordProvider<Shout, IShoutGetter>>(browserSettingsParam),
            nameof(ISpellGetter)
                => componentContext.Resolve<RecordProvider<Spell, ISpellGetter>>(browserSettingsParam),
            nameof(IWordOfPowerGetter)
                => componentContext.Resolve<RecordProvider<WordOfPower, IWordOfPowerGetter>>(browserSettingsParam),

            nameof(IAnimatedObjectGetter)
                => componentContext.Resolve<RecordProvider<AnimatedObject, IAnimatedObjectGetter>>(browserSettingsParam),
            nameof(IArtObjectGetter)
                => componentContext.Resolve<RecordProvider<ArtObject, IArtObjectGetter>>(browserSettingsParam),
            nameof(ICollisionLayerGetter)
                => componentContext.Resolve<RecordProvider<CollisionLayer, ICollisionLayerGetter>>(browserSettingsParam),
            nameof(IColorRecordGetter)
                => componentContext.Resolve<RecordProvider<ColorRecord, IColorRecordGetter>>(browserSettingsParam),
            nameof(ICombatStyleGetter)
                => componentContext.Resolve<RecordProvider<CombatStyle, ICombatStyleGetter>>(browserSettingsParam),
            nameof(IFormListGetter)
                => componentContext.Resolve<RecordProvider<FormList, IFormListGetter>>(browserSettingsParam),
            nameof(IGlobalGetter)
                => componentContext.Resolve<RecordProvider<Global, IGlobalGetter>>(browserSettingsParam),
            nameof(IKeywordGetter)
                => componentContext.Resolve<RecordProvider<Keyword, IKeywordGetter>>(browserSettingsParam),
            nameof(ILandscapeTextureGetter)
                => componentContext.Resolve<RecordProvider<LandscapeTexture, ILandscapeTextureGetter>>(browserSettingsParam),
            nameof(ILoadScreenGetter)
                => componentContext.Resolve<RecordProvider<LoadScreen, ILoadScreenGetter>>(browserSettingsParam),
            nameof(IMaterialObjectGetter)
                => componentContext.Resolve<RecordProvider<MaterialObject, IMaterialObjectGetter>>(browserSettingsParam),
            nameof(IMessageGetter)
                => componentContext.Resolve<RecordProvider<Message, IMessageGetter>>(browserSettingsParam),
            nameof(ITextureSetGetter)
                => componentContext.Resolve<RecordProvider<TextureSet, ITextureSetGetter>>(browserSettingsParam),

            nameof(IAddonNodeGetter)
                => componentContext.Resolve<RecordProvider<AddonNode, IAddonNodeGetter>>(browserSettingsParam),
            nameof(ICameraShotGetter)
                => componentContext.Resolve<RecordProvider<CameraShot, ICameraShotGetter>>(browserSettingsParam),
            nameof(IDebrisGetter)
                => componentContext.Resolve<RecordProvider<Debris, IDebrisGetter>>(browserSettingsParam),
            nameof(IEffectShaderGetter)
                => componentContext.Resolve<RecordProvider<EffectShader, IEffectShaderGetter>>(browserSettingsParam),
            nameof(IExplosionGetter)
                => componentContext.Resolve<RecordProvider<Explosion, IExplosionGetter>>(browserSettingsParam),
            nameof(IFootstepGetter)
                => componentContext.Resolve<RecordProvider<Footstep, IFootstepGetter>>(browserSettingsParam),
            nameof(IFootstepSetGetter)
                => componentContext.Resolve<RecordProvider<FootstepSet, IFootstepSetGetter>>(browserSettingsParam),
            nameof(IHazardGetter)
                => componentContext.Resolve<RecordProvider<Hazard, IHazardGetter>>(browserSettingsParam),
            nameof(IImageSpaceGetter)
                => componentContext.Resolve<RecordProvider<ImageSpace, IImageSpaceGetter>>(browserSettingsParam),
            nameof(IImageSpaceAdapterGetter)
                => componentContext.Resolve<RecordProvider<ImageSpaceAdapter, IImageSpaceAdapterGetter>>(browserSettingsParam),
            nameof(IImpactGetter)
                => componentContext.Resolve<RecordProvider<Impact, IImpactGetter>>(browserSettingsParam),
            nameof(IImpactDataSetGetter)
                => componentContext.Resolve<RecordProvider<ImpactDataSet, IImpactDataSetGetter>>(browserSettingsParam),
            nameof(IMaterialTypeGetter)
                => componentContext.Resolve<RecordProvider<MaterialType, IMaterialTypeGetter>>(browserSettingsParam),
            nameof(IProjectileGetter)
                => componentContext.Resolve<RecordProvider<Projectile, IProjectileGetter>>(browserSettingsParam),
            nameof(IVolumetricLightingGetter)
                => componentContext.Resolve<RecordProvider<VolumetricLighting, IVolumetricLightingGetter>>(browserSettingsParam),

            nameof(IClimateGetter)
                => componentContext.Resolve<RecordProvider<Climate, IClimateGetter>>(browserSettingsParam),
            nameof(IEncounterZoneGetter)
                => componentContext.Resolve<RecordProvider<EncounterZone, IEncounterZoneGetter>>(browserSettingsParam),
            nameof(ILightingTemplateGetter)
                => componentContext.Resolve<RecordProvider<LightingTemplate, ILightingTemplateGetter>>(browserSettingsParam),
            nameof(ILocationGetter)
                => componentContext.Resolve<RecordProvider<Location, ILocationGetter>>(browserSettingsParam),
            nameof(ILocationReferenceTypeGetter)
                => componentContext.Resolve<RecordProvider<LocationReferenceType, ILocationReferenceTypeGetter>>(browserSettingsParam),
            nameof(IShaderParticleGeometryGetter)
                => componentContext.Resolve<RecordProvider<ShaderParticleGeometry, IShaderParticleGeometryGetter>>(browserSettingsParam),
            nameof(IVisualEffectGetter)
                => componentContext.Resolve<RecordProvider<VisualEffect, IVisualEffectGetter>>(browserSettingsParam),
            nameof(IWaterGetter)
                => componentContext.Resolve<RecordProvider<Water, IWaterGetter>>(browserSettingsParam),
            nameof(IWeatherGetter)
                => componentContext.Resolve<RecordProvider<Weather, IWeatherGetter>>(browserSettingsParam),

            nameof(IActivatorGetter)
                => componentContext.Resolve<RecordProvider<Activator, IActivatorGetter>>(browserSettingsParam),
            nameof(IContainerGetter)
                => componentContext.Resolve<RecordProvider<Container, IContainerGetter>>(browserSettingsParam),
            nameof(IDoorGetter)
                => componentContext.Resolve<RecordProvider<Door, IDoorGetter>>(browserSettingsParam),
            nameof(IFloraGetter)
                => componentContext.Resolve<RecordProvider<Flora, IFloraGetter>>(browserSettingsParam),
            nameof(IFurnitureGetter)
                => componentContext.Resolve<RecordProvider<Furniture, IFurnitureGetter>>(browserSettingsParam),
            nameof(IIdleMarkerGetter)
                => componentContext.Resolve<RecordProvider<IdleMarker, IIdleMarkerGetter>>(browserSettingsParam),
            nameof(IGrassGetter)
                => componentContext.Resolve<RecordProvider<Grass, IGrassGetter>>(browserSettingsParam),
            nameof(ILightGetter)
                => componentContext.Resolve<RecordProvider<Light, ILightGetter>>(browserSettingsParam),
            nameof(IMoveableStaticGetter)
                => componentContext.Resolve<RecordProvider<MoveableStatic, IMoveableStaticGetter>>(browserSettingsParam),
            nameof(IStaticGetter)
                => componentContext.Resolve<RecordProvider<Static, IStaticGetter>>(browserSettingsParam),
            nameof(ITreeGetter)
                => componentContext.Resolve<RecordProvider<Tree, ITreeGetter>>(browserSettingsParam),

            _ => componentContext.Resolve<RecordTypeProvider>(TypedParameter.From(type.AsEnumerable()), browserSettingsParam),
        };
    }
}
