using System;
using System.Collections.Generic;
using Autofac;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Services.Filter;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Activator = Mutagen.Bethesda.Skyrim.Activator;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.List;

public sealed class SkyrimRecordListFactory : IRecordListFactory {
    private readonly Func<IRecordProvider, IRecordListVM> _recordListVMFactory;
    private readonly Func<IEnumerable<IFormLinkIdentifier>, IRecordBrowserSettings?, RecordIdentifiersProvider> _recordIdentifiersProviderFactory;
    private readonly IRecordBrowserSettings _defaultRecordBrowserSettings;
    private readonly ILifetimeScope _lifetimeScope;

    public SkyrimRecordListFactory(
        ILifetimeScope lifetimeScope,
        Func<IRecordProvider, IRecordListVM> recordListVMFactory,
        Func<IEnumerable<IFormLinkIdentifier>, IRecordBrowserSettings?, RecordIdentifiersProvider> recordIdentifiersProviderFactory,
        IRecordBrowserSettings defaultRecordBrowserSettings) {
        _lifetimeScope = lifetimeScope;
        _recordListVMFactory = recordListVMFactory;
        _recordIdentifiersProviderFactory = recordIdentifiersProviderFactory;
        _defaultRecordBrowserSettings = defaultRecordBrowserSettings;
    }

    public IRecordListVM FromIdentifiers(IEnumerable<IFormLinkIdentifier> identifiers, IRecordBrowserSettings? browserSettings = null) {
        browserSettings ??= _defaultRecordBrowserSettings;
        var recordProvider = _recordIdentifiersProviderFactory(identifiers, browserSettings);
        var recordListVM = _recordListVMFactory(recordProvider);
        return recordListVM;
    }

    public IRecordListVM FromType(Type type, IRecordBrowserSettings? browserSettings = null) {
        browserSettings ??= _defaultRecordBrowserSettings;
        var browserSettingsParam = TypedParameter.From(browserSettings);

        var newScope = _lifetimeScope.BeginLifetimeScope();

        IRecordProvider recordProvider = type.Name switch {
            nameof(INpcGetter) => newScope.Resolve<RecordProvider<Npc, INpcGetter>>(browserSettingsParam),
            nameof(IActionRecordGetter) => newScope.Resolve<RecordProvider<ActionRecord, IActionRecordGetter>>(browserSettingsParam),
            nameof(IBodyPartDataGetter) => newScope.Resolve<RecordProvider<BodyPartData, IBodyPartDataGetter>>(browserSettingsParam),
            nameof(ILeveledNpcGetter) => newScope.Resolve<RecordProvider<LeveledNpc, ILeveledNpcGetter>>(browserSettingsParam),
            nameof(IPerkGetter) => newScope.Resolve<RecordProvider<Perk, IPerkGetter>>(browserSettingsParam),
            nameof(ITalkingActivatorGetter) => newScope.Resolve<RecordProvider<TalkingActivator, ITalkingActivatorGetter>>(browserSettingsParam),

            nameof(IAcousticSpaceGetter) => newScope.Resolve<RecordProvider<AcousticSpace, IAcousticSpaceGetter>>(browserSettingsParam),
            nameof(IMusicTrackGetter) => newScope.Resolve<RecordProvider<MusicTrack, IMusicTrackGetter>>(browserSettingsParam),
            nameof(IMusicTypeGetter) => newScope.Resolve<RecordProvider<MusicType, IMusicTypeGetter>>(browserSettingsParam),
            nameof(IReverbParametersGetter) => newScope.Resolve<RecordProvider<ReverbParameters, IReverbParametersGetter>>(browserSettingsParam),
            nameof(ISoundCategoryGetter) => newScope.Resolve<RecordProvider<SoundCategory, ISoundCategoryGetter>>(browserSettingsParam),
            nameof(ISoundDescriptorGetter) => newScope.Resolve<RecordProvider<SoundDescriptor, ISoundDescriptorGetter>>(browserSettingsParam),
            nameof(ISoundMarkerGetter) => newScope.Resolve<RecordProvider<SoundMarker, ISoundMarkerGetter>>(browserSettingsParam),
            nameof(ISoundOutputModelGetter) => newScope.Resolve<RecordProvider<SoundOutputModel, ISoundOutputModelGetter>>(browserSettingsParam),

            nameof(IAssociationTypeGetter) => newScope.Resolve<RecordProvider<AssociationType, IAssociationTypeGetter>>(browserSettingsParam),
            nameof(IClassGetter) => newScope.Resolve<RecordProvider<Class, IClassGetter>>(browserSettingsParam),
            nameof(IEquipTypeGetter) => newScope.Resolve<RecordProvider<EquipType, IEquipTypeGetter>>(browserSettingsParam),
            nameof(IFactionGetter) => newScope.Resolve<RecordProvider<Faction, IFactionGetter>>(browserSettingsParam),
            nameof(IHeadPartGetter) => newScope.Resolve<RecordProvider<HeadPart, IHeadPartGetter>>(browserSettingsParam),
            nameof(IMovementTypeGetter) => newScope.Resolve<RecordProvider<MovementType, IMovementTypeGetter>>(browserSettingsParam),
            nameof(IPackageGetter) => newScope.Resolve<RecordProvider<Package, IPackageGetter>>(browserSettingsParam),
            nameof(IQuestGetter) => newScope.Resolve<RecordProvider<Quest, IQuestGetter>>(browserSettingsParam),
            nameof(IRaceGetter) => newScope.Resolve<RecordProvider<Race, IRaceGetter>>(browserSettingsParam),
            nameof(IRelationshipGetter) => newScope.Resolve<RecordProvider<Relationship, IRelationshipGetter>>(browserSettingsParam),
            nameof(IStoryManagerEventNodeGetter) => newScope.Resolve<RecordProvider<StoryManagerEventNode, IStoryManagerEventNodeGetter>>(browserSettingsParam),
            nameof(IVoiceTypeGetter) => newScope.Resolve<RecordProvider<VoiceType, IVoiceTypeGetter>>(browserSettingsParam),

            nameof(IAmmunitionGetter) => newScope.Resolve<RecordProvider<Ammunition, IAmmunitionGetter>>(browserSettingsParam),
            nameof(IArmorGetter) => newScope.Resolve<RecordProvider<Armor, IArmorGetter>>(browserSettingsParam),
            nameof(IArmorAddonGetter) => newScope.Resolve<RecordProvider<ArmorAddon, IArmorAddonGetter>>(browserSettingsParam),
            nameof(IBookGetter) => newScope.Resolve<RecordProvider<Book, IBookGetter>>(browserSettingsParam),
            nameof(IConstructibleObjectGetter) => newScope.Resolve<RecordProvider<ConstructibleObject, IConstructibleObjectGetter>>(browserSettingsParam),
            nameof(IIngredientGetter) => newScope.Resolve<RecordProvider<Ingredient, IIngredientGetter>>(browserSettingsParam),
            nameof(IKeyGetter) => newScope.Resolve<RecordProvider<Key, IKeyGetter>>(browserSettingsParam),
            nameof(ILeveledItemGetter) => newScope.Resolve<RecordProvider<LeveledItem, ILeveledItemGetter>>(browserSettingsParam),
            nameof(IMiscItemGetter) => newScope.Resolve<RecordProvider<MiscItem, IMiscItemGetter>>(browserSettingsParam),
            nameof(IOutfitGetter) => newScope.Resolve<RecordProvider<Outfit, IOutfitGetter>>(browserSettingsParam),
            nameof(ISoulGemGetter) => newScope.Resolve<RecordProvider<SoulGem, ISoulGemGetter>>(browserSettingsParam),
            nameof(IWeaponGetter) => newScope.Resolve<RecordProvider<Weapon, IWeaponGetter>>(browserSettingsParam),

            nameof(IDualCastDataGetter) => newScope.Resolve<RecordProvider<DualCastData, IDualCastDataGetter>>(browserSettingsParam),
            nameof(IObjectEffectGetter) => newScope.Resolve<RecordProvider<ObjectEffect, IObjectEffectGetter>>(browserSettingsParam),
            nameof(ILeveledSpellGetter) => newScope.Resolve<RecordProvider<LeveledSpell, ILeveledSpellGetter>>(browserSettingsParam),
            nameof(IMagicEffectGetter) => newScope.Resolve<RecordProvider<MagicEffect, IMagicEffectGetter>>(browserSettingsParam),
            nameof(IIngestibleGetter) => newScope.Resolve<RecordProvider<Ingestible, IIngestibleGetter>>(browserSettingsParam),
            nameof(IScrollGetter) => newScope.Resolve<RecordProvider<Scroll, IScrollGetter>>(browserSettingsParam),
            nameof(IShoutGetter) => newScope.Resolve<RecordProvider<Shout, IShoutGetter>>(browserSettingsParam),
            nameof(ISpellGetter) => newScope.Resolve<RecordProvider<Spell, ISpellGetter>>(browserSettingsParam),
            nameof(IWordOfPowerGetter) => newScope.Resolve<RecordProvider<WordOfPower, IWordOfPowerGetter>>(browserSettingsParam),

            nameof(IAnimatedObjectGetter) => newScope.Resolve<RecordProvider<AnimatedObject, IAnimatedObjectGetter>>(browserSettingsParam),
            nameof(IArtObjectGetter) => newScope.Resolve<RecordProvider<ArtObject, IArtObjectGetter>>(browserSettingsParam),
            nameof(ICollisionLayerGetter) => newScope.Resolve<RecordProvider<CollisionLayer, ICollisionLayerGetter>>(browserSettingsParam),
            nameof(IColorRecordGetter) => newScope.Resolve<RecordProvider<ColorRecord, IColorRecordGetter>>(browserSettingsParam),
            nameof(ICombatStyleGetter) => newScope.Resolve<RecordProvider<CombatStyle, ICombatStyleGetter>>(browserSettingsParam),
            nameof(IFormListGetter) => newScope.Resolve<RecordProvider<FormList, IFormListGetter>>(browserSettingsParam),
            nameof(IGlobalGetter) => newScope.Resolve<RecordProvider<Global, IGlobalGetter>>(browserSettingsParam),
            nameof(IKeywordGetter) => newScope.Resolve<RecordProvider<Keyword, IKeywordGetter>>(browserSettingsParam),
            nameof(ILandscapeTextureGetter) => newScope.Resolve<RecordProvider<LandscapeTexture, ILandscapeTextureGetter>>(browserSettingsParam),
            nameof(ILoadScreenGetter) => newScope.Resolve<RecordProvider<LoadScreen, ILoadScreenGetter>>(browserSettingsParam),
            nameof(IMaterialObjectGetter) => newScope.Resolve<RecordProvider<MaterialObject, IMaterialObjectGetter>>(browserSettingsParam),
            nameof(IMessageGetter) => newScope.Resolve<RecordProvider<Message, IMessageGetter>>(browserSettingsParam),
            nameof(ITextureSetGetter) => newScope.Resolve<RecordProvider<TextureSet, ITextureSetGetter>>(browserSettingsParam),

            nameof(IAddonNodeGetter) => newScope.Resolve<RecordProvider<AddonNode, IAddonNodeGetter>>(browserSettingsParam),
            nameof(ICameraShotGetter) => newScope.Resolve<RecordProvider<CameraShot, ICameraShotGetter>>(browserSettingsParam),
            nameof(IDebrisGetter) => newScope.Resolve<RecordProvider<Debris, IDebrisGetter>>(browserSettingsParam),
            nameof(IEffectShaderGetter) => newScope.Resolve<RecordProvider<EffectShader, IEffectShaderGetter>>(browserSettingsParam),
            nameof(IExplosionGetter) => newScope.Resolve<RecordProvider<Explosion, IExplosionGetter>>(browserSettingsParam),
            nameof(IFootstepGetter) => newScope.Resolve<RecordProvider<Footstep, IFootstepGetter>>(browserSettingsParam),
            nameof(IFootstepSetGetter) => newScope.Resolve<RecordProvider<FootstepSet, IFootstepSetGetter>>(browserSettingsParam),
            nameof(IHazardGetter) => newScope.Resolve<RecordProvider<Hazard, IHazardGetter>>(browserSettingsParam),
            nameof(IImageSpaceGetter) => newScope.Resolve<RecordProvider<ImageSpace, IImageSpaceGetter>>(browserSettingsParam),
            nameof(IImageSpaceAdapterGetter) => newScope.Resolve<RecordProvider<ImageSpaceAdapter, IImageSpaceAdapterGetter>>(browserSettingsParam),
            nameof(IImpactGetter) => newScope.Resolve<RecordProvider<Impact, IImpactGetter>>(browserSettingsParam),
            nameof(IImpactDataSetGetter) => newScope.Resolve<RecordProvider<ImpactDataSet, IImpactDataSetGetter>>(browserSettingsParam),
            nameof(IMaterialTypeGetter) => newScope.Resolve<RecordProvider<MaterialType, IMaterialTypeGetter>>(browserSettingsParam),
            nameof(IProjectileGetter) => newScope.Resolve<RecordProvider<Projectile, IProjectileGetter>>(browserSettingsParam),
            nameof(IVolumetricLightingGetter) => newScope.Resolve<RecordProvider<VolumetricLighting, IVolumetricLightingGetter>>(browserSettingsParam),

            nameof(IClimateGetter) => newScope.Resolve<RecordProvider<Climate, IClimateGetter>>(browserSettingsParam),
            nameof(IEncounterZoneGetter) => newScope.Resolve<RecordProvider<EncounterZone, IEncounterZoneGetter>>(browserSettingsParam),
            nameof(ILightingTemplateGetter) => newScope.Resolve<RecordProvider<LightingTemplate, ILightingTemplateGetter>>(browserSettingsParam),
            nameof(ILocationGetter) => newScope.Resolve<RecordProvider<Location, ILocationGetter>>(browserSettingsParam),
            nameof(ILocationReferenceTypeGetter) => newScope.Resolve<RecordProvider<LocationReferenceType, ILocationReferenceTypeGetter>>(browserSettingsParam),
            nameof(IShaderParticleGeometryGetter) => newScope.Resolve<RecordProvider<ShaderParticleGeometry, IShaderParticleGeometryGetter>>(browserSettingsParam),
            nameof(IVisualEffectGetter) => newScope.Resolve<RecordProvider<VisualEffect, IVisualEffectGetter>>(browserSettingsParam),
            nameof(IWaterGetter) => newScope.Resolve<RecordProvider<Water, IWaterGetter>>(browserSettingsParam),
            nameof(IWeatherGetter) => newScope.Resolve<RecordProvider<Weather, IWeatherGetter>>(browserSettingsParam),

            nameof(IActivatorGetter) => newScope.Resolve<RecordProvider<Activator, IActivatorGetter>>(browserSettingsParam),
            nameof(IContainerGetter) => newScope.Resolve<RecordProvider<Container, IContainerGetter>>(browserSettingsParam),
            nameof(IDoorGetter) => newScope.Resolve<RecordProvider<Door, IDoorGetter>>(browserSettingsParam),
            nameof(IFloraGetter) => newScope.Resolve<RecordProvider<Flora, IFloraGetter>>(browserSettingsParam),
            nameof(IFurnitureGetter) => newScope.Resolve<RecordProvider<Furniture, IFurnitureGetter>>(browserSettingsParam),
            nameof(IIdleMarkerGetter) => newScope.Resolve<RecordProvider<IdleMarker, IIdleMarkerGetter>>(browserSettingsParam),
            nameof(IGrassGetter) => newScope.Resolve<RecordProvider<Grass, IGrassGetter>>(browserSettingsParam),
            nameof(ILightGetter) => newScope.Resolve<RecordProvider<Light, ILightGetter>>(browserSettingsParam),
            nameof(IMoveableStaticGetter) => newScope.Resolve<RecordProvider<MoveableStatic, IMoveableStaticGetter>>(browserSettingsParam),
            nameof(IStaticGetter) => newScope.Resolve<RecordProvider<Static, IStaticGetter>>(browserSettingsParam),
            nameof(ITreeGetter) => newScope.Resolve<RecordProvider<Tree, ITreeGetter>>(browserSettingsParam),

            _ => newScope.Resolve<RecordTypeProvider>(TypedParameter.From(type.AsEnumerable()), browserSettingsParam)
        };

        var recordListVM = newScope.Resolve<IRecordListVM>(TypedParameter.From(recordProvider));

        newScope.DisposeWith(recordListVM);

        return recordListVM;
    }
}
