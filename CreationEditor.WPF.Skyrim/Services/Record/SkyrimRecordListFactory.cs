using System;
using Autofac;
using CreationEditor.WPF.Models.Record.Browser;
using CreationEditor.WPF.Services.Record;
using CreationEditor.WPF.ViewModels.Record;
using CreationEditor.WPF.Views.Record;
using Mutagen.Bethesda.Skyrim;
using Activator = Mutagen.Bethesda.Skyrim.Activator;
namespace CreationEditor.WPF.Skyrim.Services.Record;

public class SkyrimRecordListFactory : IRecordListFactory {
    private readonly ILifetimeScope _lifetimeScope;
    private readonly IRecordBrowserSettings _defaultRecordBrowserSettings;
    private readonly IExtraColumnProvider _extraColumnProvider;

    public SkyrimRecordListFactory(
        ILifetimeScope lifetimeScope,
        IRecordBrowserSettings defaultRecordBrowserSettings,
        IExtraColumnProvider extraColumnProvider) {
        _lifetimeScope = lifetimeScope;
        _defaultRecordBrowserSettings = defaultRecordBrowserSettings;
        _extraColumnProvider = extraColumnProvider;
    }
    
    public RecordList FromType(Type type, IRecordBrowserSettings? browserSettings = null) {
        browserSettings ??= _defaultRecordBrowserSettings;
        var browserSettingsParam = TypedParameter.From(browserSettings);

        var recordList = new RecordList();
        
        _extraColumnProvider.GetColumns(type)
            .ForEach(recordList.AddColumn);
        
        recordList.DataContext = type.Name switch {
            nameof(INpcGetter) => _lifetimeScope.Resolve<RecordListVM<Npc, INpcGetter>>(browserSettingsParam),
            nameof(IActionRecordGetter) => _lifetimeScope.Resolve<RecordListVM<ActionRecord, IActionRecordGetter>>(browserSettingsParam),
            nameof(IBodyPartDataGetter) => _lifetimeScope.Resolve<RecordListVM<BodyPartData, IBodyPartDataGetter>>(browserSettingsParam),
            nameof(ILeveledNpcGetter) => _lifetimeScope.Resolve<RecordListVM<LeveledNpc, ILeveledNpcGetter>>(browserSettingsParam),
            nameof(IPerkGetter) => _lifetimeScope.Resolve<RecordListVM<Perk, IPerkGetter>>(browserSettingsParam),
            nameof(ITalkingActivatorGetter) => _lifetimeScope.Resolve<RecordListVM<TalkingActivator, ITalkingActivatorGetter>>(browserSettingsParam),
                
            nameof(IAcousticSpaceGetter) => _lifetimeScope.Resolve<RecordListVM<AcousticSpace, IAcousticSpaceGetter>>(browserSettingsParam),
            nameof(IMusicTrackGetter) => _lifetimeScope.Resolve<RecordListVM<MusicTrack, IMusicTrackGetter>>(browserSettingsParam),
            nameof(IMusicTypeGetter) => _lifetimeScope.Resolve<RecordListVM<MusicType, IMusicTypeGetter>>(browserSettingsParam),
            nameof(IReverbParametersGetter) => _lifetimeScope.Resolve<RecordListVM<ReverbParameters, IReverbParametersGetter>>(browserSettingsParam),
            nameof(ISoundCategoryGetter) => _lifetimeScope.Resolve<RecordListVM<SoundCategory, ISoundCategoryGetter>>(browserSettingsParam),
            nameof(ISoundDescriptorGetter) => _lifetimeScope.Resolve<RecordListVM<SoundDescriptor, ISoundDescriptorGetter>>(browserSettingsParam),
            nameof(ISoundMarkerGetter) => _lifetimeScope.Resolve<RecordListVM<SoundMarker, ISoundMarkerGetter>>(browserSettingsParam),
            nameof(ISoundOutputModelGetter) => _lifetimeScope.Resolve<RecordListVM<SoundOutputModel, ISoundOutputModelGetter>>(browserSettingsParam),

            nameof(IAssociationTypeGetter) => _lifetimeScope.Resolve<RecordListVM<AssociationType, IAssociationTypeGetter>>(browserSettingsParam),
            nameof(IClassGetter) => _lifetimeScope.Resolve<RecordListVM<Class, IClassGetter>>(browserSettingsParam),
            nameof(IEquipTypeGetter) => _lifetimeScope.Resolve<RecordListVM<EquipType, IEquipTypeGetter>>(browserSettingsParam),
            nameof(IFactionGetter) => _lifetimeScope.Resolve<RecordListVM<Faction, IFactionGetter>>(browserSettingsParam),
            nameof(IHeadPartGetter) => _lifetimeScope.Resolve<RecordListVM<HeadPart, IHeadPartGetter>>(browserSettingsParam),
            nameof(IMovementTypeGetter) => _lifetimeScope.Resolve<RecordListVM<MovementType, IMovementTypeGetter>>(browserSettingsParam),
            nameof(IPackageGetter) => _lifetimeScope.Resolve<RecordListVM<Package, IPackageGetter>>(browserSettingsParam),
            nameof(IQuestGetter) => _lifetimeScope.Resolve<RecordListVM<Quest, IQuestGetter>>(browserSettingsParam),
            nameof(IRaceGetter) => _lifetimeScope.Resolve<RecordListVM<Race, IRaceGetter>>(browserSettingsParam),
            nameof(IRelationshipGetter) => _lifetimeScope.Resolve<RecordListVM<Relationship, IRelationshipGetter>>(browserSettingsParam),
            nameof(IStoryManagerEventNodeGetter) => _lifetimeScope.Resolve<RecordListVM<StoryManagerEventNode, IStoryManagerEventNodeGetter>>(browserSettingsParam),
            nameof(IVoiceTypeGetter) => _lifetimeScope.Resolve<RecordListVM<VoiceType, IVoiceTypeGetter>>(browserSettingsParam),

            nameof(IAmmunitionGetter) => _lifetimeScope.Resolve<RecordListVM<Ammunition, IAmmunitionGetter>>(browserSettingsParam),
            nameof(IArmorGetter) => _lifetimeScope.Resolve<RecordListVM<Armor, IArmorGetter>>(browserSettingsParam),
            nameof(IArmorAddonGetter) => _lifetimeScope.Resolve<RecordListVM<ArmorAddon, IArmorAddonGetter>>(browserSettingsParam),
            nameof(IBookGetter) => _lifetimeScope.Resolve<RecordListVM<Book, IBookGetter>>(browserSettingsParam),
            nameof(IConstructibleObjectGetter) => _lifetimeScope.Resolve<RecordListVM<ConstructibleObject, IConstructibleObjectGetter>>(browserSettingsParam),
            nameof(IIngredientGetter) => _lifetimeScope.Resolve<RecordListVM<Ingredient, IIngredientGetter>>(browserSettingsParam),
            nameof(IKeyGetter) => _lifetimeScope.Resolve<RecordListVM<Key, IKeyGetter>>(browserSettingsParam),
            nameof(ILeveledItemGetter) => _lifetimeScope.Resolve<RecordListVM<LeveledItem, ILeveledItemGetter>>(browserSettingsParam),
            nameof(IMiscItemGetter) => _lifetimeScope.Resolve<RecordListVM<MiscItem, IMiscItemGetter>>(browserSettingsParam),
            nameof(IOutfitGetter) => _lifetimeScope.Resolve<RecordListVM<Outfit, IOutfitGetter>>(browserSettingsParam),
            nameof(ISoulGemGetter) => _lifetimeScope.Resolve<RecordListVM<SoulGem, ISoulGemGetter>>(browserSettingsParam),
            nameof(IWeaponGetter) => _lifetimeScope.Resolve<RecordListVM<Weapon, IWeaponGetter>>(browserSettingsParam),
            
            nameof(IDualCastDataGetter) => _lifetimeScope.Resolve<RecordListVM<DualCastData, IDualCastDataGetter>>(browserSettingsParam),
            nameof(IObjectEffectGetter) => _lifetimeScope.Resolve<RecordListVM<ObjectEffect, IObjectEffectGetter>>(browserSettingsParam),
            nameof(ILeveledSpellGetter) => _lifetimeScope.Resolve<RecordListVM<LeveledSpell, ILeveledSpellGetter>>(browserSettingsParam),
            nameof(IMagicEffectGetter) => _lifetimeScope.Resolve<RecordListVM<MagicEffect, IMagicEffectGetter>>(browserSettingsParam),
            nameof(IIngestibleGetter) => _lifetimeScope.Resolve<RecordListVM<Ingestible, IIngestibleGetter>>(browserSettingsParam),
            nameof(IScrollGetter) => _lifetimeScope.Resolve<RecordListVM<Scroll, IScrollGetter>>(browserSettingsParam),
            nameof(ISpellGetter) => _lifetimeScope.Resolve<RecordListVM<Spell, ISpellGetter>>(browserSettingsParam),
            nameof(IWordOfPowerGetter) => _lifetimeScope.Resolve<RecordListVM<WordOfPower, IWordOfPowerGetter>>(browserSettingsParam),
            
            nameof(IAnimatedObjectGetter) => _lifetimeScope.Resolve<RecordListVM<AnimatedObject, IAnimatedObjectGetter>>(browserSettingsParam),
            nameof(IArtObjectGetter) => _lifetimeScope.Resolve<RecordListVM<ArtObject, IArtObjectGetter>>(browserSettingsParam),
            nameof(ICollisionLayerGetter) => _lifetimeScope.Resolve<RecordListVM<CollisionLayer, ICollisionLayerGetter>>(browserSettingsParam),
            nameof(IFormListGetter) => _lifetimeScope.Resolve<RecordListVM<FormList, IFormListGetter>>(browserSettingsParam),
            nameof(IGlobalGetter) => _lifetimeScope.Resolve<RecordListVM<Global, IGlobalGetter>>(browserSettingsParam),
            nameof(IIdleMarkerGetter) => _lifetimeScope.Resolve<RecordListVM<IdleMarker, IIdleMarkerGetter>>(browserSettingsParam),
            nameof(IKeywordGetter) => _lifetimeScope.Resolve<RecordListVM<Keyword, IKeywordGetter>>(browserSettingsParam),
            nameof(ILandscapeTextureGetter) => _lifetimeScope.Resolve<RecordListVM<LandscapeTexture, ILandscapeTextureGetter>>(browserSettingsParam),
            nameof(ILoadScreenGetter) => _lifetimeScope.Resolve<RecordListVM<LoadScreen, ILoadScreenGetter>>(browserSettingsParam),
            nameof(IMaterialObjectGetter) => _lifetimeScope.Resolve<RecordListVM<MaterialObject, IMaterialObjectGetter>>(browserSettingsParam),
            nameof(IMessageGetter) => _lifetimeScope.Resolve<RecordListVM<Message, IMessageGetter>>(browserSettingsParam),
            nameof(ITextureSetGetter) => _lifetimeScope.Resolve<RecordListVM<TextureSet, ITextureSetGetter>>(browserSettingsParam),
            
            nameof(IAddonNodeGetter) => _lifetimeScope.Resolve<RecordListVM<AddonNode, IAddonNodeGetter>>(browserSettingsParam),
            nameof(ICameraShotGetter) => _lifetimeScope.Resolve<RecordListVM<CameraShot, ICameraShotGetter>>(browserSettingsParam),
            nameof(IDebrisGetter) => _lifetimeScope.Resolve<RecordListVM<Debris, IDebrisGetter>>(browserSettingsParam),
            nameof(IEffectShaderGetter) => _lifetimeScope.Resolve<RecordListVM<EffectShader, IEffectShaderGetter>>(browserSettingsParam),
            nameof(IExplosionGetter) => _lifetimeScope.Resolve<RecordListVM<Explosion, IExplosionGetter>>(browserSettingsParam),
            nameof(IFootstepGetter) => _lifetimeScope.Resolve<RecordListVM<Footstep, IFootstepGetter>>(browserSettingsParam),
            nameof(IFootstepSetGetter) => _lifetimeScope.Resolve<RecordListVM<FootstepSet, IFootstepSetGetter>>(browserSettingsParam),
            nameof(IHazardGetter) => _lifetimeScope.Resolve<RecordListVM<Hazard, IHazardGetter>>(browserSettingsParam),
            nameof(IImageSpaceGetter) => _lifetimeScope.Resolve<RecordListVM<ImageSpace, IImageSpaceGetter>>(browserSettingsParam),
            nameof(IImageSpaceAdapterGetter) => _lifetimeScope.Resolve<RecordListVM<ImageSpaceAdapter, IImageSpaceAdapterGetter>>(browserSettingsParam),
            nameof(IImpactDataSetGetter) => _lifetimeScope.Resolve<RecordListVM<ImpactDataSet, IImpactDataSetGetter>>(browserSettingsParam),
            nameof(ILensFlareGetter) => _lifetimeScope.Resolve<RecordListVM<LensFlare, ILensFlareGetter>>(browserSettingsParam),
            nameof(IMaterialTypeGetter) => _lifetimeScope.Resolve<RecordListVM<MaterialType, IMaterialTypeGetter>>(browserSettingsParam),
            nameof(IProjectileGetter) => _lifetimeScope.Resolve<RecordListVM<Projectile, IProjectileGetter>>(browserSettingsParam),
            nameof(IVolumetricLightingGetter) => _lifetimeScope.Resolve<RecordListVM<VolumetricLighting, IVolumetricLightingGetter>>(browserSettingsParam),
            
            nameof(IClimateGetter) => _lifetimeScope.Resolve<RecordListVM<Climate, IClimateGetter>>(browserSettingsParam),
            nameof(IEncounterZoneGetter) => _lifetimeScope.Resolve<RecordListVM<EncounterZone, IEncounterZoneGetter>>(browserSettingsParam),
            nameof(ILightingTemplateGetter) => _lifetimeScope.Resolve<RecordListVM<LightingTemplate, ILightingTemplateGetter>>(browserSettingsParam),
            nameof(ILocationGetter) => _lifetimeScope.Resolve<RecordListVM<Location, ILocationGetter>>(browserSettingsParam),
            nameof(ILocationReferenceTypeGetter) => _lifetimeScope.Resolve<RecordListVM<LocationReferenceType, ILocationReferenceTypeGetter>>(browserSettingsParam),
            nameof(IShaderParticleGeometryGetter) => _lifetimeScope.Resolve<RecordListVM<ShaderParticleGeometry, IShaderParticleGeometryGetter>>(browserSettingsParam),
            nameof(IVisualEffectGetter) => _lifetimeScope.Resolve<RecordListVM<VisualEffect, IVisualEffectGetter>>(browserSettingsParam),
            nameof(IWaterGetter) => _lifetimeScope.Resolve<RecordListVM<Water, IWaterGetter>>(browserSettingsParam),
            nameof(IWeatherGetter) => _lifetimeScope.Resolve<RecordListVM<Weather, IWeatherGetter>>(browserSettingsParam),
            
            nameof(IActivatorGetter) => _lifetimeScope.Resolve<RecordListVM<Activator, IActivatorGetter>>(browserSettingsParam),
            nameof(IContainerGetter) => _lifetimeScope.Resolve<RecordListVM<Container, IContainerGetter>>(browserSettingsParam),
            nameof(IDoorGetter) => _lifetimeScope.Resolve<RecordListVM<Door, IDoorGetter>>(browserSettingsParam),
            nameof(IFloraGetter) => _lifetimeScope.Resolve<RecordListVM<Flora, IFloraGetter>>(browserSettingsParam),
            nameof(IFurnitureGetter) => _lifetimeScope.Resolve<RecordListVM<Furniture, IFurnitureGetter>>(browserSettingsParam),
            nameof(IGrassGetter) => _lifetimeScope.Resolve<RecordListVM<Grass, IGrassGetter>>(browserSettingsParam),
            nameof(ILightGetter) => _lifetimeScope.Resolve<RecordListVM<Light, ILightGetter>>(browserSettingsParam),
            nameof(IMoveableStaticGetter) => _lifetimeScope.Resolve<RecordListVM<MoveableStatic, IMoveableStaticGetter>>(browserSettingsParam),
            nameof(IStaticGetter) => _lifetimeScope.Resolve<RecordListVM<Static, IStaticGetter>>(browserSettingsParam),
            nameof(ITreeGetter) => _lifetimeScope.Resolve<RecordListVM<Tree, ITreeGetter>>(browserSettingsParam),
            
            _ => _lifetimeScope.Resolve<RecordListVMReadOnly>(TypedParameter.From(type), browserSettingsParam)
        };
        
        return recordList;
    }
}
