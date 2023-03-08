using System;
using System.Collections.Generic;
using Autofac;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Avalonia.Views.Record;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Activator = Mutagen.Bethesda.Skyrim.Activator;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.List;

public sealed class SkyrimRecordListFactory : IRecordListFactory {
    private readonly IComponentContext _componentContext;
    private readonly IRecordBrowserSettingsVM _defaultRecordBrowserSettingsVM;

    public SkyrimRecordListFactory(
        IComponentContext componentContext,
        IRecordBrowserSettingsVM defaultRecordBrowserSettingsVM) {
        _componentContext = componentContext;
        _defaultRecordBrowserSettingsVM = defaultRecordBrowserSettingsVM;
    }

    public RecordList FromIdentifiers(IEnumerable<IFormLinkIdentifier> identifiers, IRecordBrowserSettingsVM? browserSettings = null) {
        browserSettings ??= _defaultRecordBrowserSettingsVM;
        var browserSettingsParam = TypedParameter.From(browserSettings);
        var identifiersParam = TypedParameter.From(identifiers);

        var extraColumnsBuilder = _componentContext.Resolve<IExtraColumnsBuilder>();
        var columns = extraColumnsBuilder
            .AddRecordType<IMajorRecordGetter>()
            .AddColumnType<TypeExtraColumns>()
            .Build();

        var recordProvider = _componentContext.Resolve<RecordIdentifiersProvider>(identifiersParam, browserSettingsParam);

        var providerParam = TypedParameter.From<IRecordProvider>(recordProvider);
        var recordListVM = _componentContext.Resolve<IRecordListVM>(providerParam);
        var recordList = new RecordList(columns) { DataContext = recordListVM };

        return recordList;
    }

    public RecordList FromType(Type type, IRecordBrowserSettingsVM? browserSettings = null) {
        browserSettings ??= _defaultRecordBrowserSettingsVM;
        var browserSettingsParam = TypedParameter.From(browserSettings);

        IRecordProvider recordProvider = type.Name switch {
            nameof(INpcGetter) => _componentContext.Resolve<RecordProvider<Npc, INpcGetter>>(browserSettingsParam),
            nameof(IActionRecordGetter) => _componentContext.Resolve<RecordProvider<ActionRecord, IActionRecordGetter>>(browserSettingsParam),
            nameof(IBodyPartDataGetter) => _componentContext.Resolve<RecordProvider<BodyPartData, IBodyPartDataGetter>>(browserSettingsParam),
            nameof(ILeveledNpcGetter) => _componentContext.Resolve<RecordProvider<LeveledNpc, ILeveledNpcGetter>>(browserSettingsParam),
            nameof(IPerkGetter) => _componentContext.Resolve<RecordProvider<Perk, IPerkGetter>>(browserSettingsParam),
            nameof(ITalkingActivatorGetter) => _componentContext.Resolve<RecordProvider<TalkingActivator, ITalkingActivatorGetter>>(browserSettingsParam),

            nameof(IAcousticSpaceGetter) => _componentContext.Resolve<RecordProvider<AcousticSpace, IAcousticSpaceGetter>>(browserSettingsParam),
            nameof(IMusicTrackGetter) => _componentContext.Resolve<RecordProvider<MusicTrack, IMusicTrackGetter>>(browserSettingsParam),
            nameof(IMusicTypeGetter) => _componentContext.Resolve<RecordProvider<MusicType, IMusicTypeGetter>>(browserSettingsParam),
            nameof(IReverbParametersGetter) => _componentContext.Resolve<RecordProvider<ReverbParameters, IReverbParametersGetter>>(browserSettingsParam),
            nameof(ISoundCategoryGetter) => _componentContext.Resolve<RecordProvider<SoundCategory, ISoundCategoryGetter>>(browserSettingsParam),
            nameof(ISoundDescriptorGetter) => _componentContext.Resolve<RecordProvider<SoundDescriptor, ISoundDescriptorGetter>>(browserSettingsParam),
            nameof(ISoundMarkerGetter) => _componentContext.Resolve<RecordProvider<SoundMarker, ISoundMarkerGetter>>(browserSettingsParam),
            nameof(ISoundOutputModelGetter) => _componentContext.Resolve<RecordProvider<SoundOutputModel, ISoundOutputModelGetter>>(browserSettingsParam),

            nameof(IAssociationTypeGetter) => _componentContext.Resolve<RecordProvider<AssociationType, IAssociationTypeGetter>>(browserSettingsParam),
            nameof(IClassGetter) => _componentContext.Resolve<RecordProvider<Class, IClassGetter>>(browserSettingsParam),
            nameof(IEquipTypeGetter) => _componentContext.Resolve<RecordProvider<EquipType, IEquipTypeGetter>>(browserSettingsParam),
            nameof(IFactionGetter) => _componentContext.Resolve<RecordProvider<Faction, IFactionGetter>>(browserSettingsParam),
            nameof(IHeadPartGetter) => _componentContext.Resolve<RecordProvider<HeadPart, IHeadPartGetter>>(browserSettingsParam),
            nameof(IMovementTypeGetter) => _componentContext.Resolve<RecordProvider<MovementType, IMovementTypeGetter>>(browserSettingsParam),
            nameof(IPackageGetter) => _componentContext.Resolve<RecordProvider<Package, IPackageGetter>>(browserSettingsParam),
            nameof(IQuestGetter) => _componentContext.Resolve<RecordProvider<Quest, IQuestGetter>>(browserSettingsParam),
            nameof(IRaceGetter) => _componentContext.Resolve<RecordProvider<Race, IRaceGetter>>(browserSettingsParam),
            nameof(IRelationshipGetter) => _componentContext.Resolve<RecordProvider<Relationship, IRelationshipGetter>>(browserSettingsParam),
            nameof(IStoryManagerEventNodeGetter) => _componentContext.Resolve<RecordProvider<StoryManagerEventNode, IStoryManagerEventNodeGetter>>(browserSettingsParam),
            nameof(IVoiceTypeGetter) => _componentContext.Resolve<RecordProvider<VoiceType, IVoiceTypeGetter>>(browserSettingsParam),

            nameof(IAmmunitionGetter) => _componentContext.Resolve<RecordProvider<Ammunition, IAmmunitionGetter>>(browserSettingsParam),
            nameof(IArmorGetter) => _componentContext.Resolve<RecordProvider<Armor, IArmorGetter>>(browserSettingsParam),
            nameof(IArmorAddonGetter) => _componentContext.Resolve<RecordProvider<ArmorAddon, IArmorAddonGetter>>(browserSettingsParam),
            nameof(IBookGetter) => _componentContext.Resolve<RecordProvider<Book, IBookGetter>>(browserSettingsParam),
            nameof(IConstructibleObjectGetter) => _componentContext.Resolve<RecordProvider<ConstructibleObject, IConstructibleObjectGetter>>(browserSettingsParam),
            nameof(IIngredientGetter) => _componentContext.Resolve<RecordProvider<Ingredient, IIngredientGetter>>(browserSettingsParam),
            nameof(IKeyGetter) => _componentContext.Resolve<RecordProvider<Key, IKeyGetter>>(browserSettingsParam),
            nameof(ILeveledItemGetter) => _componentContext.Resolve<RecordProvider<LeveledItem, ILeveledItemGetter>>(browserSettingsParam),
            nameof(IMiscItemGetter) => _componentContext.Resolve<RecordProvider<MiscItem, IMiscItemGetter>>(browserSettingsParam),
            nameof(IOutfitGetter) => _componentContext.Resolve<RecordProvider<Outfit, IOutfitGetter>>(browserSettingsParam),
            nameof(ISoulGemGetter) => _componentContext.Resolve<RecordProvider<SoulGem, ISoulGemGetter>>(browserSettingsParam),
            nameof(IWeaponGetter) => _componentContext.Resolve<RecordProvider<Weapon, IWeaponGetter>>(browserSettingsParam),

            nameof(IDualCastDataGetter) => _componentContext.Resolve<RecordProvider<DualCastData, IDualCastDataGetter>>(browserSettingsParam),
            nameof(IObjectEffectGetter) => _componentContext.Resolve<RecordProvider<ObjectEffect, IObjectEffectGetter>>(browserSettingsParam),
            nameof(ILeveledSpellGetter) => _componentContext.Resolve<RecordProvider<LeveledSpell, ILeveledSpellGetter>>(browserSettingsParam),
            nameof(IMagicEffectGetter) => _componentContext.Resolve<RecordProvider<MagicEffect, IMagicEffectGetter>>(browserSettingsParam),
            nameof(IIngestibleGetter) => _componentContext.Resolve<RecordProvider<Ingestible, IIngestibleGetter>>(browserSettingsParam),
            nameof(IScrollGetter) => _componentContext.Resolve<RecordProvider<Scroll, IScrollGetter>>(browserSettingsParam),
            nameof(IShoutGetter) => _componentContext.Resolve<RecordProvider<Shout, IShoutGetter>>(browserSettingsParam),
            nameof(ISpellGetter) => _componentContext.Resolve<RecordProvider<Spell, ISpellGetter>>(browserSettingsParam),
            nameof(IWordOfPowerGetter) => _componentContext.Resolve<RecordProvider<WordOfPower, IWordOfPowerGetter>>(browserSettingsParam),

            nameof(IAnimatedObjectGetter) => _componentContext.Resolve<RecordProvider<AnimatedObject, IAnimatedObjectGetter>>(browserSettingsParam),
            nameof(IArtObjectGetter) => _componentContext.Resolve<RecordProvider<ArtObject, IArtObjectGetter>>(browserSettingsParam),
            nameof(ICollisionLayerGetter) => _componentContext.Resolve<RecordProvider<CollisionLayer, ICollisionLayerGetter>>(browserSettingsParam),
            nameof(IColorRecordGetter) => _componentContext.Resolve<RecordProvider<ColorRecord, IColorRecordGetter>>(browserSettingsParam),
            nameof(ICombatStyleGetter) => _componentContext.Resolve<RecordProvider<CombatStyle, ICombatStyleGetter>>(browserSettingsParam),
            nameof(IFormListGetter) => _componentContext.Resolve<RecordProvider<FormList, IFormListGetter>>(browserSettingsParam),
            nameof(IGlobalGetter) => _componentContext.Resolve<RecordProvider<Global, IGlobalGetter>>(browserSettingsParam),
            nameof(IKeywordGetter) => _componentContext.Resolve<RecordProvider<Keyword, IKeywordGetter>>(browserSettingsParam),
            nameof(ILandscapeTextureGetter) => _componentContext.Resolve<RecordProvider<LandscapeTexture, ILandscapeTextureGetter>>(browserSettingsParam),
            nameof(ILoadScreenGetter) => _componentContext.Resolve<RecordProvider<LoadScreen, ILoadScreenGetter>>(browserSettingsParam),
            nameof(IMaterialObjectGetter) => _componentContext.Resolve<RecordProvider<MaterialObject, IMaterialObjectGetter>>(browserSettingsParam),
            nameof(IMessageGetter) => _componentContext.Resolve<RecordProvider<Message, IMessageGetter>>(browserSettingsParam),
            nameof(ITextureSetGetter) => _componentContext.Resolve<RecordProvider<TextureSet, ITextureSetGetter>>(browserSettingsParam),

            nameof(IAddonNodeGetter) => _componentContext.Resolve<RecordProvider<AddonNode, IAddonNodeGetter>>(browserSettingsParam),
            nameof(ICameraShotGetter) => _componentContext.Resolve<RecordProvider<CameraShot, ICameraShotGetter>>(browserSettingsParam),
            nameof(IDebrisGetter) => _componentContext.Resolve<RecordProvider<Debris, IDebrisGetter>>(browserSettingsParam),
            nameof(IEffectShaderGetter) => _componentContext.Resolve<RecordProvider<EffectShader, IEffectShaderGetter>>(browserSettingsParam),
            nameof(IExplosionGetter) => _componentContext.Resolve<RecordProvider<Explosion, IExplosionGetter>>(browserSettingsParam),
            nameof(IFootstepGetter) => _componentContext.Resolve<RecordProvider<Footstep, IFootstepGetter>>(browserSettingsParam),
            nameof(IFootstepSetGetter) => _componentContext.Resolve<RecordProvider<FootstepSet, IFootstepSetGetter>>(browserSettingsParam),
            nameof(IHazardGetter) => _componentContext.Resolve<RecordProvider<Hazard, IHazardGetter>>(browserSettingsParam),
            nameof(IImageSpaceGetter) => _componentContext.Resolve<RecordProvider<ImageSpace, IImageSpaceGetter>>(browserSettingsParam),
            nameof(IImageSpaceAdapterGetter) => _componentContext.Resolve<RecordProvider<ImageSpaceAdapter, IImageSpaceAdapterGetter>>(browserSettingsParam),
            nameof(IImpactGetter) => _componentContext.Resolve<RecordProvider<Impact, IImpactGetter>>(browserSettingsParam),
            nameof(IImpactDataSetGetter) => _componentContext.Resolve<RecordProvider<ImpactDataSet, IImpactDataSetGetter>>(browserSettingsParam),
            nameof(IMaterialTypeGetter) => _componentContext.Resolve<RecordProvider<MaterialType, IMaterialTypeGetter>>(browserSettingsParam),
            nameof(IProjectileGetter) => _componentContext.Resolve<RecordProvider<Projectile, IProjectileGetter>>(browserSettingsParam),
            nameof(IVolumetricLightingGetter) => _componentContext.Resolve<RecordProvider<VolumetricLighting, IVolumetricLightingGetter>>(browserSettingsParam),

            nameof(IClimateGetter) => _componentContext.Resolve<RecordProvider<Climate, IClimateGetter>>(browserSettingsParam),
            nameof(IEncounterZoneGetter) => _componentContext.Resolve<RecordProvider<EncounterZone, IEncounterZoneGetter>>(browserSettingsParam),
            nameof(ILightingTemplateGetter) => _componentContext.Resolve<RecordProvider<LightingTemplate, ILightingTemplateGetter>>(browserSettingsParam),
            nameof(ILocationGetter) => _componentContext.Resolve<RecordProvider<Location, ILocationGetter>>(browserSettingsParam),
            nameof(ILocationReferenceTypeGetter) => _componentContext.Resolve<RecordProvider<LocationReferenceType, ILocationReferenceTypeGetter>>(browserSettingsParam),
            nameof(IShaderParticleGeometryGetter) => _componentContext.Resolve<RecordProvider<ShaderParticleGeometry, IShaderParticleGeometryGetter>>(browserSettingsParam),
            nameof(IVisualEffectGetter) => _componentContext.Resolve<RecordProvider<VisualEffect, IVisualEffectGetter>>(browserSettingsParam),
            nameof(IWaterGetter) => _componentContext.Resolve<RecordProvider<Water, IWaterGetter>>(browserSettingsParam),
            nameof(IWeatherGetter) => _componentContext.Resolve<RecordProvider<Weather, IWeatherGetter>>(browserSettingsParam),

            nameof(IActivatorGetter) => _componentContext.Resolve<RecordProvider<Activator, IActivatorGetter>>(browserSettingsParam),
            nameof(IContainerGetter) => _componentContext.Resolve<RecordProvider<Container, IContainerGetter>>(browserSettingsParam),
            nameof(IDoorGetter) => _componentContext.Resolve<RecordProvider<Door, IDoorGetter>>(browserSettingsParam),
            nameof(IFloraGetter) => _componentContext.Resolve<RecordProvider<Flora, IFloraGetter>>(browserSettingsParam),
            nameof(IFurnitureGetter) => _componentContext.Resolve<RecordProvider<Furniture, IFurnitureGetter>>(browserSettingsParam),
            nameof(IIdleMarkerGetter) => _componentContext.Resolve<RecordProvider<IdleMarker, IIdleMarkerGetter>>(browserSettingsParam),
            nameof(IGrassGetter) => _componentContext.Resolve<RecordProvider<Grass, IGrassGetter>>(browserSettingsParam),
            nameof(ILightGetter) => _componentContext.Resolve<RecordProvider<Light, ILightGetter>>(browserSettingsParam),
            nameof(IMoveableStaticGetter) => _componentContext.Resolve<RecordProvider<MoveableStatic, IMoveableStaticGetter>>(browserSettingsParam),
            nameof(IStaticGetter) => _componentContext.Resolve<RecordProvider<Static, IStaticGetter>>(browserSettingsParam),
            nameof(ITreeGetter) => _componentContext.Resolve<RecordProvider<Tree, ITreeGetter>>(browserSettingsParam),

            _ => _componentContext.Resolve<RecordTypeProvider>(TypedParameter.From(type.AsEnumerable()), browserSettingsParam)
        };

        var recordListVM = _componentContext.Resolve<IRecordListVM>(TypedParameter.From(recordProvider));

        var extraColumnsBuilder = _componentContext.Resolve<IExtraColumnsBuilder>();
        var columns = extraColumnsBuilder
            .AddRecordType(type)
            .Build();

        var recordList = new RecordList(columns) { DataContext = recordListVM };

        return recordList;
    }
}
