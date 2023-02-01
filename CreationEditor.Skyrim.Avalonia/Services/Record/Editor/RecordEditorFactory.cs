using Autofac;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels.Record.Editor;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Location = Mutagen.Bethesda.Skyrim.Location;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Editor;

public class SkyrimRecordEditorFactory : IRecordEditorFactory {
    private readonly IComponentContext _componentContext;

    public SkyrimRecordEditorFactory(
        IComponentContext componentContext) {
        _componentContext = componentContext;
    }

    public Control? FromType(IMajorRecord record) {
        return record switch {
            Npc npc => _componentContext.ResolveOptional<IRecordEditorVM<Npc, INpcGetter>>()?.CreateControl(npc),
            ActionRecord actionRecord => _componentContext.ResolveOptional<IRecordEditorVM<ActionRecord, IActionRecordGetter>>()?.CreateControl(actionRecord),
            BodyPartData bodyPartData => _componentContext.ResolveOptional<IRecordEditorVM<BodyPartData, IBodyPartDataGetter>>()?.CreateControl(bodyPartData),
            LeveledNpc leveledNpc => _componentContext.ResolveOptional<IRecordEditorVM<LeveledNpc, ILeveledNpcGetter>>()?.CreateControl(leveledNpc),
            Perk perk => _componentContext.ResolveOptional<IRecordEditorVM<Perk, IPerkGetter>>()?.CreateControl(perk),
            TalkingActivator talkingActivator => _componentContext.ResolveOptional<IRecordEditorVM<TalkingActivator, ITalkingActivatorGetter>>()?.CreateControl(talkingActivator),

            AcousticSpace acousticSpace => _componentContext.ResolveOptional<IRecordEditorVM<AcousticSpace, IAcousticSpaceGetter>>()?.CreateControl(acousticSpace),
            MusicTrack musicTrack => _componentContext.ResolveOptional<IRecordEditorVM<MusicTrack, IMusicTrackGetter>>()?.CreateControl(musicTrack),
            MusicType musicType => _componentContext.ResolveOptional<IRecordEditorVM<MusicType, IMusicTypeGetter>>()?.CreateControl(musicType),
            ReverbParameters reverbParameters => _componentContext.ResolveOptional<IRecordEditorVM<ReverbParameters, IReverbParametersGetter>>()?.CreateControl(reverbParameters),
            SoundCategory soundCategory => _componentContext.ResolveOptional<IRecordEditorVM<SoundCategory, ISoundCategoryGetter>>()?.CreateControl(soundCategory),
            SoundDescriptor soundDescriptor => _componentContext.ResolveOptional<IRecordEditorVM<SoundDescriptor, ISoundDescriptorGetter>>()?.CreateControl(soundDescriptor),
            SoundMarker soundMarker => _componentContext.ResolveOptional<IRecordEditorVM<SoundMarker, ISoundMarkerGetter>>()?.CreateControl(soundMarker),
            SoundOutputModel soundOutputModel => _componentContext.ResolveOptional<IRecordEditorVM<SoundOutputModel, ISoundOutputModelGetter>>()?.CreateControl(soundOutputModel),

            AssociationType associationType => _componentContext.ResolveOptional<IRecordEditorVM<AssociationType, IAssociationTypeGetter>>()?.CreateControl(associationType),
            Class @class => _componentContext.ResolveOptional<IRecordEditorVM<Class, IClassGetter>>()?.CreateControl(@class),
            EquipType equipType => _componentContext.ResolveOptional<IRecordEditorVM<EquipType, IEquipTypeGetter>>()?.CreateControl(equipType),
            Faction faction => _componentContext.ResolveOptional<IRecordEditorVM<Faction, IFactionGetter>>()?.CreateControl(faction),
            HeadPart headPart => _componentContext.ResolveOptional<IRecordEditorVM<HeadPart, IHeadPartGetter>>()?.CreateControl(headPart),
            MovementType movementType => _componentContext.ResolveOptional<IRecordEditorVM<MovementType, IMovementTypeGetter>>()?.CreateControl(movementType),
            Package package => _componentContext.ResolveOptional<IRecordEditorVM<Package, IPackageGetter>>()?.CreateControl(package),
            Quest quest => _componentContext.ResolveOptional<IRecordEditorVM<Quest, IQuestGetter>>()?.CreateControl(quest),
            Race race => _componentContext.ResolveOptional<IRecordEditorVM<Race, IRaceGetter>>()?.CreateControl(race),
            Relationship relationship => _componentContext.ResolveOptional<IRecordEditorVM<Relationship, IRelationshipGetter>>()?.CreateControl(relationship),
            StoryManagerEventNode storyManagerEventNode => _componentContext.ResolveOptional<IRecordEditorVM<StoryManagerEventNode, IStoryManagerEventNodeGetter>>()?.CreateControl(storyManagerEventNode),
            VoiceType voiceType => _componentContext.ResolveOptional<IRecordEditorVM<VoiceType, IVoiceTypeGetter>>()?.CreateControl(voiceType),

            Ammunition ammunition => _componentContext.ResolveOptional<IRecordEditorVM<Ammunition, IAmmunitionGetter>>()?.CreateControl(ammunition),
            Armor armor => _componentContext.ResolveOptional<IRecordEditorVM<Armor, IArmorGetter>>()?.CreateControl(armor),
            ArmorAddon armorAddon => _componentContext.ResolveOptional<IRecordEditorVM<ArmorAddon, IArmorAddonGetter>>()?.CreateControl(armorAddon),
            Book book => _componentContext.ResolveOptional<IRecordEditorVM<Book, IBookGetter>>()?.CreateControl(book),
            ConstructibleObject constructibleObject => _componentContext.ResolveOptional<IRecordEditorVM<ConstructibleObject, IConstructibleObjectGetter>>()?.CreateControl(constructibleObject),
            Ingredient ingredient => _componentContext.ResolveOptional<IRecordEditorVM<Ingredient, IIngredientGetter>>()?.CreateControl(ingredient),
            Key key => _componentContext.ResolveOptional<IRecordEditorVM<Key, IKeyGetter>>()?.CreateControl(key),
            LeveledItem leveledItem => _componentContext.ResolveOptional<IRecordEditorVM<LeveledItem, ILeveledItemGetter>>()?.CreateControl(leveledItem),
            MiscItem miscItem => _componentContext.ResolveOptional<IRecordEditorVM<MiscItem, IMiscItemGetter>>()?.CreateControl(miscItem),
            Outfit outfit => _componentContext.ResolveOptional<IRecordEditorVM<Outfit, IOutfitGetter>>()?.CreateControl(outfit),
            SoulGem soulGem => _componentContext.ResolveOptional<IRecordEditorVM<SoulGem, ISoulGemGetter>>()?.CreateControl(soulGem),
            Weapon weapon => _componentContext.ResolveOptional<IRecordEditorVM<Weapon, IWeaponGetter>>()?.CreateControl(weapon),

            DualCastData dualCastData => _componentContext.ResolveOptional<IRecordEditorVM<DualCastData, IDualCastDataGetter>>()?.CreateControl(dualCastData),
            ObjectEffect objectEffect => _componentContext.ResolveOptional<IRecordEditorVM<ObjectEffect, IObjectEffectGetter>>()?.CreateControl(objectEffect),
            LeveledSpell leveledSpell => _componentContext.ResolveOptional<IRecordEditorVM<LeveledSpell, ILeveledSpellGetter>>()?.CreateControl(leveledSpell),
            MagicEffect magicEffect => _componentContext.ResolveOptional<IRecordEditorVM<MagicEffect, IMagicEffectGetter>>()?.CreateControl(magicEffect),
            Ingestible ingestible => _componentContext.ResolveOptional<IRecordEditorVM<Ingestible, IIngestibleGetter>>()?.CreateControl(ingestible),
            Scroll scroll => _componentContext.ResolveOptional<IRecordEditorVM<Scroll, IScrollGetter>>()?.CreateControl(scroll),
            Spell spell => _componentContext.ResolveOptional<IRecordEditorVM<Spell, ISpellGetter>>()?.CreateControl(spell),
            WordOfPower wordOfPower => _componentContext.ResolveOptional<IRecordEditorVM<WordOfPower, IWordOfPowerGetter>>()?.CreateControl(wordOfPower),

            AnimatedObject animatedObject => _componentContext.ResolveOptional<IRecordEditorVM<AnimatedObject, IAnimatedObjectGetter>>()?.CreateControl(animatedObject),
            ArtObject artObject => _componentContext.ResolveOptional<IRecordEditorVM<ArtObject, IArtObjectGetter>>()?.CreateControl(artObject),
            CollisionLayer collisionLayer => _componentContext.ResolveOptional<IRecordEditorVM<CollisionLayer, ICollisionLayerGetter>>()?.CreateControl(collisionLayer),
            FormList formList => _componentContext.ResolveOptional<IRecordEditorVM<FormList, IFormListGetter>>()?.CreateControl(formList),
            Global global => _componentContext.ResolveOptional<IRecordEditorVM<Global, IGlobalGetter>>()?.CreateControl(global),
            IdleMarker idleMarker => _componentContext.ResolveOptional<IRecordEditorVM<IdleMarker, IIdleMarkerGetter>>()?.CreateControl(idleMarker),
            Keyword keyword => _componentContext.ResolveOptional<IRecordEditorVM<Keyword, IKeywordGetter>>()?.CreateControl(keyword),
            LandscapeTexture landscapeTexture => _componentContext.ResolveOptional<IRecordEditorVM<LandscapeTexture, ILandscapeTextureGetter>>()?.CreateControl(landscapeTexture),
            LoadScreen loadScreen => _componentContext.ResolveOptional<IRecordEditorVM<LoadScreen, ILoadScreenGetter>>()?.CreateControl(loadScreen),
            MaterialObject materialObject => _componentContext.ResolveOptional<IRecordEditorVM<MaterialObject, IMaterialObjectGetter>>()?.CreateControl(materialObject),
            Message message => _componentContext.ResolveOptional<IRecordEditorVM<Message, IMessageGetter>>()?.CreateControl(message),
            TextureSet textureSet => _componentContext.ResolveOptional<IRecordEditorVM<TextureSet, ITextureSetGetter>>()?.CreateControl(textureSet),

            AddonNode addonNode => _componentContext.ResolveOptional<IRecordEditorVM<AddonNode, IAddonNodeGetter>>()?.CreateControl(addonNode),
            CameraShot cameraShot => _componentContext.ResolveOptional<IRecordEditorVM<CameraShot, ICameraShotGetter>>()?.CreateControl(cameraShot),
            Debris debris => _componentContext.ResolveOptional<IRecordEditorVM<Debris, IDebrisGetter>>()?.CreateControl(debris),
            EffectShader effectShader => _componentContext.ResolveOptional<IRecordEditorVM<EffectShader, IEffectShaderGetter>>()?.CreateControl(effectShader),
            Explosion explosion => _componentContext.ResolveOptional<IRecordEditorVM<Explosion, IExplosionGetter>>()?.CreateControl(explosion),
            Footstep footstep => _componentContext.ResolveOptional<IRecordEditorVM<Footstep, IFootstepGetter>>()?.CreateControl(footstep),
            FootstepSet footstepSet => _componentContext.ResolveOptional<IRecordEditorVM<FootstepSet, IFootstepSetGetter>>()?.CreateControl(footstepSet),
            Hazard hazard => _componentContext.ResolveOptional<IRecordEditorVM<Hazard, IHazardGetter>>()?.CreateControl(hazard),
            ImageSpace imageSpace => _componentContext.ResolveOptional<IRecordEditorVM<ImageSpace, IImageSpaceGetter>>()?.CreateControl(imageSpace),
            ImageSpaceAdapter imageSpaceAdapter => _componentContext.ResolveOptional<IRecordEditorVM<ImageSpaceAdapter, IImageSpaceAdapterGetter>>()?.CreateControl(imageSpaceAdapter),
            ImpactDataSet impactDataSet => _componentContext.ResolveOptional<IRecordEditorVM<ImpactDataSet, IImpactDataSetGetter>>()?.CreateControl(impactDataSet),
            LensFlare lensFlare => _componentContext.ResolveOptional<IRecordEditorVM<LensFlare, ILensFlareGetter>>()?.CreateControl(lensFlare),
            MaterialType materialType => _componentContext.ResolveOptional<IRecordEditorVM<MaterialType, IMaterialTypeGetter>>()?.CreateControl(materialType),
            Projectile projectile => _componentContext.ResolveOptional<IRecordEditorVM<Projectile, IProjectileGetter>>()?.CreateControl(projectile),
            VolumetricLighting volumetricLighting => _componentContext.ResolveOptional<IRecordEditorVM<VolumetricLighting, IVolumetricLightingGetter>>()?.CreateControl(volumetricLighting),

            Climate climate => _componentContext.ResolveOptional<IRecordEditorVM<Climate, IClimateGetter>>()?.CreateControl(climate),
            EncounterZone encounterZone => _componentContext.ResolveOptional<IRecordEditorVM<EncounterZone, IEncounterZoneGetter>>()?.CreateControl(encounterZone),
            LightingTemplate lightingTemplate => _componentContext.ResolveOptional<IRecordEditorVM<LightingTemplate, ILightingTemplateGetter>>()?.CreateControl(lightingTemplate),
            Location location => _componentContext.ResolveOptional<IRecordEditorVM<Location, ILocationGetter>>()?.CreateControl(location),
            LocationReferenceType locationReferenceType => _componentContext.ResolveOptional<IRecordEditorVM<LocationReferenceType, ILocationReferenceTypeGetter>>()?.CreateControl(locationReferenceType),
            ShaderParticleGeometry shaderParticleGeometry => _componentContext.ResolveOptional<IRecordEditorVM<ShaderParticleGeometry, IShaderParticleGeometryGetter>>()?.CreateControl(shaderParticleGeometry),
            VisualEffect visualEffect => _componentContext.ResolveOptional<IRecordEditorVM<VisualEffect, IVisualEffectGetter>>()?.CreateControl(visualEffect),
            Water water => _componentContext.ResolveOptional<IRecordEditorVM<Water, IWaterGetter>>()?.CreateControl(water),
            Weather weather => _componentContext.ResolveOptional<IRecordEditorVM<Weather, IWeatherGetter>>()?.CreateControl(weather),

            Activator activator => _componentContext.ResolveOptional<IRecordEditorVM<Activator, IActivatorGetter>>()?.CreateControl(activator),
            Container container => _componentContext.ResolveOptional<IRecordEditorVM<Container, IContainerGetter>>()?.CreateControl(container),
            Door door => _componentContext.ResolveOptional<IRecordEditorVM<Door, IDoorGetter>>()?.CreateControl(door),
            Flora flora => _componentContext.ResolveOptional<IRecordEditorVM<Flora, IFloraGetter>>()?.CreateControl(flora),
            Furniture furniture => _componentContext.ResolveOptional<IRecordEditorVM<Furniture, IFurnitureGetter>>()?.CreateControl(furniture),
            Grass grass => _componentContext.ResolveOptional<IRecordEditorVM<Grass, IGrassGetter>>()?.CreateControl(grass),
            Light light => _componentContext.ResolveOptional<IRecordEditorVM<Light, ILightGetter>>()?.CreateControl(light),
            MoveableStatic moveableStatic => _componentContext.ResolveOptional<IRecordEditorVM<MoveableStatic, IMoveableStaticGetter>>()?.CreateControl(moveableStatic),
            Static @static => _componentContext.ResolveOptional<IRecordEditorVM<Static, IStaticGetter>>()?.CreateControl(@static),
            Tree tree => _componentContext.ResolveOptional<IRecordEditorVM<Tree, ITreeGetter>>()?.CreateControl(tree),

            _ => null
        };
    }
}
