using Autofac;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels.Record.Editor;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Location = Mutagen.Bethesda.Skyrim.Location;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Editor;

public class SkyrimRecordEditorFactory : IRecordEditorFactory {
    private readonly ILifetimeScope _lifetimeScope;

    public SkyrimRecordEditorFactory(
        ILifetimeScope lifetimeScope) {
        _lifetimeScope = lifetimeScope;
    }

    public Control? FromType(IMajorRecord record) {
        return record switch {
            Npc npc => _lifetimeScope.ResolveOptional<IRecordEditorVM<Npc, INpcGetter>>()?.CreateControl(npc),
            ActionRecord actionRecord => _lifetimeScope.ResolveOptional<IRecordEditorVM<ActionRecord, IActionRecordGetter>>()?.CreateControl(actionRecord),
            BodyPartData BodyPartData => _lifetimeScope.ResolveOptional<IRecordEditorVM<BodyPartData, IBodyPartDataGetter>>()?.CreateControl(BodyPartData),
            LeveledNpc LeveledNpc => _lifetimeScope.ResolveOptional<IRecordEditorVM<LeveledNpc, ILeveledNpcGetter>>()?.CreateControl(LeveledNpc),
            Perk Perk => _lifetimeScope.ResolveOptional<IRecordEditorVM<Perk, IPerkGetter>>()?.CreateControl(Perk),
            TalkingActivator TalkingActivator => _lifetimeScope.ResolveOptional<IRecordEditorVM<TalkingActivator, ITalkingActivatorGetter>>()?.CreateControl(TalkingActivator),

            AcousticSpace AcousticSpace => _lifetimeScope.ResolveOptional<IRecordEditorVM<AcousticSpace, IAcousticSpaceGetter>>()?.CreateControl(AcousticSpace),
            MusicTrack MusicTrack => _lifetimeScope.ResolveOptional<IRecordEditorVM<MusicTrack, IMusicTrackGetter>>()?.CreateControl(MusicTrack),
            MusicType MusicType => _lifetimeScope.ResolveOptional<IRecordEditorVM<MusicType, IMusicTypeGetter>>()?.CreateControl(MusicType),
            ReverbParameters ReverbParameters => _lifetimeScope.ResolveOptional<IRecordEditorVM<ReverbParameters, IReverbParametersGetter>>()?.CreateControl(ReverbParameters),
            SoundCategory SoundCategory => _lifetimeScope.ResolveOptional<IRecordEditorVM<SoundCategory, ISoundCategoryGetter>>()?.CreateControl(SoundCategory),
            SoundDescriptor SoundDescriptor => _lifetimeScope.ResolveOptional<IRecordEditorVM<SoundDescriptor, ISoundDescriptorGetter>>()?.CreateControl(SoundDescriptor),
            SoundMarker SoundMarker => _lifetimeScope.ResolveOptional<IRecordEditorVM<SoundMarker, ISoundMarkerGetter>>()?.CreateControl(SoundMarker),
            SoundOutputModel SoundOutputModel => _lifetimeScope.ResolveOptional<IRecordEditorVM<SoundOutputModel, ISoundOutputModelGetter>>()?.CreateControl(SoundOutputModel),

            AssociationType AssociationType => _lifetimeScope.ResolveOptional<IRecordEditorVM<AssociationType, IAssociationTypeGetter>>()?.CreateControl(AssociationType),
            Class Class => _lifetimeScope.ResolveOptional<IRecordEditorVM<Class, IClassGetter>>()?.CreateControl(Class),
            EquipType EquipType => _lifetimeScope.ResolveOptional<IRecordEditorVM<EquipType, IEquipTypeGetter>>()?.CreateControl(EquipType),
            Faction Faction => _lifetimeScope.ResolveOptional<IRecordEditorVM<Faction, IFactionGetter>>()?.CreateControl(Faction),
            HeadPart HeadPart => _lifetimeScope.ResolveOptional<IRecordEditorVM<HeadPart, IHeadPartGetter>>()?.CreateControl(HeadPart),
            MovementType MovementType => _lifetimeScope.ResolveOptional<IRecordEditorVM<MovementType, IMovementTypeGetter>>()?.CreateControl(MovementType),
            Package Package => _lifetimeScope.ResolveOptional<IRecordEditorVM<Package, IPackageGetter>>()?.CreateControl(Package),
            Quest Quest => _lifetimeScope.ResolveOptional<IRecordEditorVM<Quest, IQuestGetter>>()?.CreateControl(Quest),
            Race Race => _lifetimeScope.ResolveOptional<IRecordEditorVM<Race, IRaceGetter>>()?.CreateControl(Race),
            Relationship Relationship => _lifetimeScope.ResolveOptional<IRecordEditorVM<Relationship, IRelationshipGetter>>()?.CreateControl(Relationship),
            StoryManagerEventNode StoryManagerEventNode => _lifetimeScope.ResolveOptional<IRecordEditorVM<StoryManagerEventNode, IStoryManagerEventNodeGetter>>()?.CreateControl(StoryManagerEventNode),
            VoiceType VoiceType => _lifetimeScope.ResolveOptional<IRecordEditorVM<VoiceType, IVoiceTypeGetter>>()?.CreateControl(VoiceType),

            Ammunition Ammunition => _lifetimeScope.ResolveOptional<IRecordEditorVM<Ammunition, IAmmunitionGetter>>()?.CreateControl(Ammunition),
            Armor Armor => _lifetimeScope.ResolveOptional<IRecordEditorVM<Armor, IArmorGetter>>()?.CreateControl(Armor),
            ArmorAddon ArmorAddon => _lifetimeScope.ResolveOptional<IRecordEditorVM<ArmorAddon, IArmorAddonGetter>>()?.CreateControl(ArmorAddon),
            Book Book => _lifetimeScope.ResolveOptional<IRecordEditorVM<Book, IBookGetter>>()?.CreateControl(Book),
            ConstructibleObject ConstructibleObject => _lifetimeScope.ResolveOptional<IRecordEditorVM<ConstructibleObject, IConstructibleObjectGetter>>()?.CreateControl(ConstructibleObject),
            Ingredient Ingredient => _lifetimeScope.ResolveOptional<IRecordEditorVM<Ingredient, IIngredientGetter>>()?.CreateControl(Ingredient),
            Key Key => _lifetimeScope.ResolveOptional<IRecordEditorVM<Key, IKeyGetter>>()?.CreateControl(Key),
            LeveledItem LeveledItem => _lifetimeScope.ResolveOptional<IRecordEditorVM<LeveledItem, ILeveledItemGetter>>()?.CreateControl(LeveledItem),
            MiscItem MiscItem => _lifetimeScope.ResolveOptional<IRecordEditorVM<MiscItem, IMiscItemGetter>>()?.CreateControl(MiscItem),
            Outfit Outfit => _lifetimeScope.ResolveOptional<IRecordEditorVM<Outfit, IOutfitGetter>>()?.CreateControl(Outfit),
            SoulGem SoulGem => _lifetimeScope.ResolveOptional<IRecordEditorVM<SoulGem, ISoulGemGetter>>()?.CreateControl(SoulGem),
            Weapon Weapon => _lifetimeScope.ResolveOptional<IRecordEditorVM<Weapon, IWeaponGetter>>()?.CreateControl(Weapon),

            DualCastData DualCastData => _lifetimeScope.ResolveOptional<IRecordEditorVM<DualCastData, IDualCastDataGetter>>()?.CreateControl(DualCastData),
            ObjectEffect ObjectEffect => _lifetimeScope.ResolveOptional<IRecordEditorVM<ObjectEffect, IObjectEffectGetter>>()?.CreateControl(ObjectEffect),
            LeveledSpell LeveledSpell => _lifetimeScope.ResolveOptional<IRecordEditorVM<LeveledSpell, ILeveledSpellGetter>>()?.CreateControl(LeveledSpell),
            MagicEffect MagicEffect => _lifetimeScope.ResolveOptional<IRecordEditorVM<MagicEffect, IMagicEffectGetter>>()?.CreateControl(MagicEffect),
            Ingestible Ingestible => _lifetimeScope.ResolveOptional<IRecordEditorVM<Ingestible, IIngestibleGetter>>()?.CreateControl(Ingestible),
            Scroll Scroll => _lifetimeScope.ResolveOptional<IRecordEditorVM<Scroll, IScrollGetter>>()?.CreateControl(Scroll),
            Spell Spell => _lifetimeScope.ResolveOptional<IRecordEditorVM<Spell, ISpellGetter>>()?.CreateControl(Spell),
            WordOfPower WordOfPower => _lifetimeScope.ResolveOptional<IRecordEditorVM<WordOfPower, IWordOfPowerGetter>>()?.CreateControl(WordOfPower),

            AnimatedObject AnimatedObject => _lifetimeScope.ResolveOptional<IRecordEditorVM<AnimatedObject, IAnimatedObjectGetter>>()?.CreateControl(AnimatedObject),
            ArtObject ArtObject => _lifetimeScope.ResolveOptional<IRecordEditorVM<ArtObject, IArtObjectGetter>>()?.CreateControl(ArtObject),
            CollisionLayer CollisionLayer => _lifetimeScope.ResolveOptional<IRecordEditorVM<CollisionLayer, ICollisionLayerGetter>>()?.CreateControl(CollisionLayer),
            FormList FormList => _lifetimeScope.ResolveOptional<IRecordEditorVM<FormList, IFormListGetter>>()?.CreateControl(FormList),
            Global Global => _lifetimeScope.ResolveOptional<IRecordEditorVM<Global, IGlobalGetter>>()?.CreateControl(Global),
            IdleMarker IdleMarker => _lifetimeScope.ResolveOptional<IRecordEditorVM<IdleMarker, IIdleMarkerGetter>>()?.CreateControl(IdleMarker),
            Keyword Keyword => _lifetimeScope.ResolveOptional<IRecordEditorVM<Keyword, IKeywordGetter>>()?.CreateControl(Keyword),
            LandscapeTexture LandscapeTexture => _lifetimeScope.ResolveOptional<IRecordEditorVM<LandscapeTexture, ILandscapeTextureGetter>>()?.CreateControl(LandscapeTexture),
            LoadScreen LoadScreen => _lifetimeScope.ResolveOptional<IRecordEditorVM<LoadScreen, ILoadScreenGetter>>()?.CreateControl(LoadScreen),
            MaterialObject MaterialObject => _lifetimeScope.ResolveOptional<IRecordEditorVM<MaterialObject, IMaterialObjectGetter>>()?.CreateControl(MaterialObject),
            Message Message => _lifetimeScope.ResolveOptional<IRecordEditorVM<Message, IMessageGetter>>()?.CreateControl(Message),
            TextureSet TextureSet => _lifetimeScope.ResolveOptional<IRecordEditorVM<TextureSet, ITextureSetGetter>>()?.CreateControl(TextureSet),

            AddonNode AddonNode => _lifetimeScope.ResolveOptional<IRecordEditorVM<AddonNode, IAddonNodeGetter>>()?.CreateControl(AddonNode),
            CameraShot CameraShot => _lifetimeScope.ResolveOptional<IRecordEditorVM<CameraShot, ICameraShotGetter>>()?.CreateControl(CameraShot),
            Debris Debris => _lifetimeScope.ResolveOptional<IRecordEditorVM<Debris, IDebrisGetter>>()?.CreateControl(Debris),
            EffectShader EffectShader => _lifetimeScope.ResolveOptional<IRecordEditorVM<EffectShader, IEffectShaderGetter>>()?.CreateControl(EffectShader),
            Explosion Explosion => _lifetimeScope.ResolveOptional<IRecordEditorVM<Explosion, IExplosionGetter>>()?.CreateControl(Explosion),
            Footstep Footstep => _lifetimeScope.ResolveOptional<IRecordEditorVM<Footstep, IFootstepGetter>>()?.CreateControl(Footstep),
            FootstepSet FootstepSet => _lifetimeScope.ResolveOptional<IRecordEditorVM<FootstepSet, IFootstepSetGetter>>()?.CreateControl(FootstepSet),
            Hazard Hazard => _lifetimeScope.ResolveOptional<IRecordEditorVM<Hazard, IHazardGetter>>()?.CreateControl(Hazard),
            ImageSpace ImageSpace => _lifetimeScope.ResolveOptional<IRecordEditorVM<ImageSpace, IImageSpaceGetter>>()?.CreateControl(ImageSpace),
            ImageSpaceAdapter ImageSpaceAdapter => _lifetimeScope.ResolveOptional<IRecordEditorVM<ImageSpaceAdapter, IImageSpaceAdapterGetter>>()?.CreateControl(ImageSpaceAdapter),
            ImpactDataSet ImpactDataSet => _lifetimeScope.ResolveOptional<IRecordEditorVM<ImpactDataSet, IImpactDataSetGetter>>()?.CreateControl(ImpactDataSet),
            LensFlare LensFlare => _lifetimeScope.ResolveOptional<IRecordEditorVM<LensFlare, ILensFlareGetter>>()?.CreateControl(LensFlare),
            MaterialType MaterialType => _lifetimeScope.ResolveOptional<IRecordEditorVM<MaterialType, IMaterialTypeGetter>>()?.CreateControl(MaterialType),
            Projectile Projectile => _lifetimeScope.ResolveOptional<IRecordEditorVM<Projectile, IProjectileGetter>>()?.CreateControl(Projectile),
            VolumetricLighting VolumetricLighting => _lifetimeScope.ResolveOptional<IRecordEditorVM<VolumetricLighting, IVolumetricLightingGetter>>()?.CreateControl(VolumetricLighting),

            Climate Climate => _lifetimeScope.ResolveOptional<IRecordEditorVM<Climate, IClimateGetter>>()?.CreateControl(Climate),
            EncounterZone EncounterZone => _lifetimeScope.ResolveOptional<IRecordEditorVM<EncounterZone, IEncounterZoneGetter>>()?.CreateControl(EncounterZone),
            LightingTemplate LightingTemplate => _lifetimeScope.ResolveOptional<IRecordEditorVM<LightingTemplate, ILightingTemplateGetter>>()?.CreateControl(LightingTemplate),
            Location Location => _lifetimeScope.ResolveOptional<IRecordEditorVM<Location, ILocationGetter>>()?.CreateControl(Location),
            LocationReferenceType LocationReferenceType => _lifetimeScope.ResolveOptional<IRecordEditorVM<LocationReferenceType, ILocationReferenceTypeGetter>>()?.CreateControl(LocationReferenceType),
            ShaderParticleGeometry ShaderParticleGeometry => _lifetimeScope.ResolveOptional<IRecordEditorVM<ShaderParticleGeometry, IShaderParticleGeometryGetter>>()?.CreateControl(ShaderParticleGeometry),
            VisualEffect VisualEffect => _lifetimeScope.ResolveOptional<IRecordEditorVM<VisualEffect, IVisualEffectGetter>>()?.CreateControl(VisualEffect),
            Water Water => _lifetimeScope.ResolveOptional<IRecordEditorVM<Water, IWaterGetter>>()?.CreateControl(Water),
            Weather Weather => _lifetimeScope.ResolveOptional<IRecordEditorVM<Weather, IWeatherGetter>>()?.CreateControl(Weather),

            Activator Activator => _lifetimeScope.ResolveOptional<IRecordEditorVM<Activator, IActivatorGetter>>()?.CreateControl(Activator),
            Container Container => _lifetimeScope.ResolveOptional<IRecordEditorVM<Container, IContainerGetter>>()?.CreateControl(Container),
            Door Door => _lifetimeScope.ResolveOptional<IRecordEditorVM<Door, IDoorGetter>>()?.CreateControl(Door),
            Flora Flora => _lifetimeScope.ResolveOptional<IRecordEditorVM<Flora, IFloraGetter>>()?.CreateControl(Flora),
            Furniture Furniture => _lifetimeScope.ResolveOptional<IRecordEditorVM<Furniture, IFurnitureGetter>>()?.CreateControl(Furniture),
            Grass Grass => _lifetimeScope.ResolveOptional<IRecordEditorVM<Grass, IGrassGetter>>()?.CreateControl(Grass),
            Light Light => _lifetimeScope.ResolveOptional<IRecordEditorVM<Light, ILightGetter>>()?.CreateControl(Light),
            MoveableStatic MoveableStatic => _lifetimeScope.ResolveOptional<IRecordEditorVM<MoveableStatic, IMoveableStaticGetter>>()?.CreateControl(MoveableStatic),
            Static Static => _lifetimeScope.ResolveOptional<IRecordEditorVM<Static, IStaticGetter>>()?.CreateControl(Static),
            Tree Tree => _lifetimeScope.ResolveOptional<IRecordEditorVM<Tree, ITreeGetter>>()?.CreateControl(Tree),

            _ => null
        };
    }
}
