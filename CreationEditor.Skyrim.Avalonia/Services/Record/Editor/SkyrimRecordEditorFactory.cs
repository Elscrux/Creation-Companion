using System.Diagnostics.CodeAnalysis;
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

    public bool FromType(IMajorRecord record, [MaybeNullWhen(false)] out Control control, [MaybeNullWhen(false)] out IRecordEditorVM recordEditor) {
        switch (record) {
            case Npc npc: {
                if (_componentContext.TryResolve<IRecordEditorVM<Npc, INpcGetter>>(out var editor)) {
                    control = editor.CreateControl(npc);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case ActionRecord actionRecord: {
                if (_componentContext.TryResolve<IRecordEditorVM<ActionRecord, IActionRecordGetter>>(out var editor)) {
                    control = editor.CreateControl(actionRecord);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case BodyPartData bodyPartData: {
                if (_componentContext.TryResolve<IRecordEditorVM<BodyPartData, IBodyPartDataGetter>>(out var editor)) {
                    control = editor.CreateControl(bodyPartData);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case LeveledNpc leveledNpc: {
                if (_componentContext.TryResolve<IRecordEditorVM<LeveledNpc, ILeveledNpcGetter>>(out var editor)) {
                    control = editor.CreateControl(leveledNpc);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Perk perk: {
                if (_componentContext.TryResolve<IRecordEditorVM<Perk, IPerkGetter>>(out var editor)) {
                    control = editor.CreateControl(perk);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case TalkingActivator talkingActivator: {
                if (_componentContext.TryResolve<IRecordEditorVM<TalkingActivator, ITalkingActivatorGetter>>(out var editor)) {
                    control = editor.CreateControl(talkingActivator);
                    recordEditor = editor;
                    return true;
                }
                break;
            }

            case AcousticSpace acousticSpace: {
                if (_componentContext.TryResolve<IRecordEditorVM<AcousticSpace, IAcousticSpaceGetter>>(out var editor)) {
                    control = editor.CreateControl(acousticSpace);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case MusicTrack musicTrack: {
                if (_componentContext.TryResolve<IRecordEditorVM<MusicTrack, IMusicTrackGetter>>(out var editor)) {
                    control = editor.CreateControl(musicTrack);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case MusicType musicType: {
                if (_componentContext.TryResolve<IRecordEditorVM<MusicType, IMusicTypeGetter>>(out var editor)) {
                    control = editor.CreateControl(musicType);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case ReverbParameters reverbParameters: {
                if (_componentContext.TryResolve<IRecordEditorVM<ReverbParameters, IReverbParametersGetter>>(out var editor)) {
                    control = editor.CreateControl(reverbParameters);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case SoundCategory soundCategory: {
                if (_componentContext.TryResolve<IRecordEditorVM<SoundCategory, ISoundCategoryGetter>>(out var editor)) {
                    control = editor.CreateControl(soundCategory);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case SoundDescriptor soundDescriptor: {
                if (_componentContext.TryResolve<IRecordEditorVM<SoundDescriptor, ISoundDescriptorGetter>>(out var editor)) {
                    control = editor.CreateControl(soundDescriptor);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case SoundMarker soundMarker: {
                if (_componentContext.TryResolve<IRecordEditorVM<SoundMarker, ISoundMarkerGetter>>(out var editor)) {
                    control = editor.CreateControl(soundMarker);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case SoundOutputModel soundOutputModel: {
                if (_componentContext.TryResolve<IRecordEditorVM<SoundOutputModel, ISoundOutputModelGetter>>(out var editor)) {
                    control = editor.CreateControl(soundOutputModel);
                    recordEditor = editor;
                    return true;
                }
                break;
            }

            case AssociationType associationType: {
                if (_componentContext.TryResolve<IRecordEditorVM<AssociationType, IAssociationTypeGetter>>(out var editor)) {
                    control = editor.CreateControl(associationType);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Class @class: {
                if (_componentContext.TryResolve<IRecordEditorVM<Class, IClassGetter>>(out var editor)) {
                    control = editor.CreateControl(@class);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case EquipType equipType: {
                if (_componentContext.TryResolve<IRecordEditorVM<EquipType, IEquipTypeGetter>>(out var editor)) {
                    control = editor.CreateControl(equipType);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Faction faction: {
                if (_componentContext.TryResolve<IRecordEditorVM<Faction, IFactionGetter>>(out var editor)) {
                    control = editor.CreateControl(faction);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case HeadPart headPart: {
                if (_componentContext.TryResolve<IRecordEditorVM<HeadPart, IHeadPartGetter>>(out var editor)) {
                    control = editor.CreateControl(headPart);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case MovementType movementType: {
                if (_componentContext.TryResolve<IRecordEditorVM<MovementType, IMovementTypeGetter>>(out var editor)) {
                    control = editor.CreateControl(movementType);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Package package: {
                if (_componentContext.TryResolve<IRecordEditorVM<Package, IPackageGetter>>(out var editor)) {
                    control = editor.CreateControl(package);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Quest quest: {
                if (_componentContext.TryResolve<IRecordEditorVM<Quest, IQuestGetter>>(out var editor)) {
                    control = editor.CreateControl(quest);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Race race: {
                if (_componentContext.TryResolve<IRecordEditorVM<Race, IRaceGetter>>(out var editor)) {
                    control = editor.CreateControl(race);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Relationship relationship: {
                if (_componentContext.TryResolve<IRecordEditorVM<Relationship, IRelationshipGetter>>(out var editor)) {
                    control = editor.CreateControl(relationship);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case StoryManagerEventNode storyManagerEventNode: {
                if (_componentContext.TryResolve<IRecordEditorVM<StoryManagerEventNode, IStoryManagerEventNodeGetter>>(out var editor)) {
                    control = editor.CreateControl(storyManagerEventNode);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case VoiceType voiceType: {
                if (_componentContext.TryResolve<IRecordEditorVM<VoiceType, IVoiceTypeGetter>>(out var editor)) {
                    control = editor.CreateControl(voiceType);
                    recordEditor = editor;
                    return true;
                }
                break;
            }

            case Ammunition ammunition: {
                if (_componentContext.TryResolve<IRecordEditorVM<Ammunition, IAmmunitionGetter>>(out var editor)) {
                    control = editor.CreateControl(ammunition);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Armor armor: {
                if (_componentContext.TryResolve<IRecordEditorVM<Armor, IArmorGetter>>(out var editor)) {
                    control = editor.CreateControl(armor);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case ArmorAddon armorAddon: {
                if (_componentContext.TryResolve<IRecordEditorVM<ArmorAddon, IArmorAddonGetter>>(out var editor)) {
                    control = editor.CreateControl(armorAddon);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Book book: {
                if (_componentContext.TryResolve<IRecordEditorVM<Book, IBookGetter>>(out var editor)) {
                    control = editor.CreateControl(book);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case ConstructibleObject constructibleObject: {
                if (_componentContext.TryResolve<IRecordEditorVM<ConstructibleObject, IConstructibleObjectGetter>>(out var editor)) {
                    control = editor.CreateControl(constructibleObject);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Ingredient ingredient: {
                if (_componentContext.TryResolve<IRecordEditorVM<Ingredient, IIngredientGetter>>(out var editor)) {
                    control = editor.CreateControl(ingredient);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Key key: {
                if (_componentContext.TryResolve<IRecordEditorVM<Key, IKeyGetter>>(out var editor)) {
                    control = editor.CreateControl(key);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case LeveledItem leveledItem: {
                if (_componentContext.TryResolve<IRecordEditorVM<LeveledItem, ILeveledItemGetter>>(out var editor)) {
                    control = editor.CreateControl(leveledItem);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case MiscItem miscItem: {
                if (_componentContext.TryResolve<IRecordEditorVM<MiscItem, IMiscItemGetter>>(out var editor)) {
                    control = editor.CreateControl(miscItem);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Outfit outfit: {
                if (_componentContext.TryResolve<IRecordEditorVM<Outfit, IOutfitGetter>>(out var editor)) {
                    control = editor.CreateControl(outfit);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case SoulGem soulGem: {
                if (_componentContext.TryResolve<IRecordEditorVM<SoulGem, ISoulGemGetter>>(out var editor)) {
                    control = editor.CreateControl(soulGem);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Weapon weapon: {
                if (_componentContext.TryResolve<IRecordEditorVM<Weapon, IWeaponGetter>>(out var editor)) {
                    control = editor.CreateControl(weapon);
                    recordEditor = editor;
                    return true;
                }
                break;
            }

            case DualCastData dualCastData: {
                if (_componentContext.TryResolve<IRecordEditorVM<DualCastData, IDualCastDataGetter>>(out var editor)) {
                    control = editor.CreateControl(dualCastData);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case ObjectEffect objectEffect: {
                if (_componentContext.TryResolve<IRecordEditorVM<ObjectEffect, IObjectEffectGetter>>(out var editor)) {
                    control = editor.CreateControl(objectEffect);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case LeveledSpell leveledSpell: {
                if (_componentContext.TryResolve<IRecordEditorVM<LeveledSpell, ILeveledSpellGetter>>(out var editor)) {
                    control = editor.CreateControl(leveledSpell);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case MagicEffect magicEffect: {
                if (_componentContext.TryResolve<IRecordEditorVM<MagicEffect, IMagicEffectGetter>>(out var editor)) {
                    control = editor.CreateControl(magicEffect);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Ingestible ingestible: {
                if (_componentContext.TryResolve<IRecordEditorVM<Ingestible, IIngestibleGetter>>(out var editor)) {
                    control = editor.CreateControl(ingestible);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Scroll scroll: {
                if (_componentContext.TryResolve<IRecordEditorVM<Scroll, IScrollGetter>>(out var editor)) {
                    control = editor.CreateControl(scroll);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Spell spell: {
                if (_componentContext.TryResolve<IRecordEditorVM<Spell, ISpellGetter>>(out var editor)) {
                    control = editor.CreateControl(spell);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case WordOfPower wordOfPower: {
                if (_componentContext.TryResolve<IRecordEditorVM<WordOfPower, IWordOfPowerGetter>>(out var editor)) {
                    control = editor.CreateControl(wordOfPower);
                    recordEditor = editor;
                    return true;
                }
                break;
            }

            case AnimatedObject animatedObject: {
                if (_componentContext.TryResolve<IRecordEditorVM<AnimatedObject, IAnimatedObjectGetter>>(out var editor)) {
                    control = editor.CreateControl(animatedObject);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case ArtObject artObject: {
                if (_componentContext.TryResolve<IRecordEditorVM<ArtObject, IArtObjectGetter>>(out var editor)) {
                    control = editor.CreateControl(artObject);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case CollisionLayer collisionLayer: {
                if (_componentContext.TryResolve<IRecordEditorVM<CollisionLayer, ICollisionLayerGetter>>(out var editor)) {
                    control = editor.CreateControl(collisionLayer);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case FormList formList: {
                if (_componentContext.TryResolve<IRecordEditorVM<FormList, IFormListGetter>>(out var editor)) {
                    control = editor.CreateControl(formList);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Global global: {
                if (_componentContext.TryResolve<IRecordEditorVM<Global, IGlobalGetter>>(out var editor)) {
                    control = editor.CreateControl(global);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case IdleMarker idleMarker: {
                if (_componentContext.TryResolve<IRecordEditorVM<IdleMarker, IIdleMarkerGetter>>(out var editor)) {
                    control = editor.CreateControl(idleMarker);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Keyword keyword: {
                if (_componentContext.TryResolve<IRecordEditorVM<Keyword, IKeywordGetter>>(out var editor)) {
                    control = editor.CreateControl(keyword);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case LandscapeTexture landscapeTexture: {
                if (_componentContext.TryResolve<IRecordEditorVM<LandscapeTexture, ILandscapeTextureGetter>>(out var editor)) {
                    control = editor.CreateControl(landscapeTexture);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case LoadScreen loadScreen: {
                if (_componentContext.TryResolve<IRecordEditorVM<LoadScreen, ILoadScreenGetter>>(out var editor)) {
                    control = editor.CreateControl(loadScreen);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case MaterialObject materialObject: {
                if (_componentContext.TryResolve<IRecordEditorVM<MaterialObject, IMaterialObjectGetter>>(out var editor)) {
                    control = editor.CreateControl(materialObject);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Message message: {
                if (_componentContext.TryResolve<IRecordEditorVM<Message, IMessageGetter>>(out var editor)) {
                    control = editor.CreateControl(message);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case TextureSet textureSet: {
                if (_componentContext.TryResolve<IRecordEditorVM<TextureSet, ITextureSetGetter>>(out var editor)) {
                    control = editor.CreateControl(textureSet);
                    recordEditor = editor;
                    return true;
                }
                break;
            }

            case AddonNode addonNode: {
                if (_componentContext.TryResolve<IRecordEditorVM<AddonNode, IAddonNodeGetter>>(out var editor)) {
                    control = editor.CreateControl(addonNode);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case CameraShot cameraShot: {
                if (_componentContext.TryResolve<IRecordEditorVM<CameraShot, ICameraShotGetter>>(out var editor)) {
                    control = editor.CreateControl(cameraShot);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Debris debris: {
                if (_componentContext.TryResolve<IRecordEditorVM<Debris, IDebrisGetter>>(out var editor)) {
                    control = editor.CreateControl(debris);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case EffectShader effectShader: {
                if (_componentContext.TryResolve<IRecordEditorVM<EffectShader, IEffectShaderGetter>>(out var editor)) {
                    control = editor.CreateControl(effectShader);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Explosion explosion: {
                if (_componentContext.TryResolve<IRecordEditorVM<Explosion, IExplosionGetter>>(out var editor)) {
                    control = editor.CreateControl(explosion);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Footstep footstep: {
                if (_componentContext.TryResolve<IRecordEditorVM<Footstep, IFootstepGetter>>(out var editor)) {
                    control = editor.CreateControl(footstep);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case FootstepSet footstepSet: {
                if (_componentContext.TryResolve<IRecordEditorVM<FootstepSet, IFootstepSetGetter>>(out var editor)) {
                    control = editor.CreateControl(footstepSet);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Hazard hazard: {
                if (_componentContext.TryResolve<IRecordEditorVM<Hazard, IHazardGetter>>(out var editor)) {
                    control = editor.CreateControl(hazard);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case ImageSpace imageSpace: {
                if (_componentContext.TryResolve<IRecordEditorVM<ImageSpace, IImageSpaceGetter>>(out var editor)) {
                    control = editor.CreateControl(imageSpace);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case ImageSpaceAdapter imageSpaceAdapter: {
                if (_componentContext.TryResolve<IRecordEditorVM<ImageSpaceAdapter, IImageSpaceAdapterGetter>>(out var editor)) {
                    control = editor.CreateControl(imageSpaceAdapter);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case ImpactDataSet impactDataSet: {
                if (_componentContext.TryResolve<IRecordEditorVM<ImpactDataSet, IImpactDataSetGetter>>(out var editor)) {
                    control = editor.CreateControl(impactDataSet);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case LensFlare lensFlare: {
                if (_componentContext.TryResolve<IRecordEditorVM<LensFlare, ILensFlareGetter>>(out var editor)) {
                    control = editor.CreateControl(lensFlare);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case MaterialType materialType: {
                if (_componentContext.TryResolve<IRecordEditorVM<MaterialType, IMaterialTypeGetter>>(out var editor)) {
                    control = editor.CreateControl(materialType);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Projectile projectile: {
                if (_componentContext.TryResolve<IRecordEditorVM<Projectile, IProjectileGetter>>(out var editor)) {
                    control = editor.CreateControl(projectile);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case VolumetricLighting volumetricLighting: {
                if (_componentContext.TryResolve<IRecordEditorVM<VolumetricLighting, IVolumetricLightingGetter>>(out var editor)) {
                    control = editor.CreateControl(volumetricLighting);
                    recordEditor = editor;
                    return true;
                }
                break;
            }

            case Climate climate: {
                if (_componentContext.TryResolve<IRecordEditorVM<Climate, IClimateGetter>>(out var editor)) {
                    control = editor.CreateControl(climate);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case EncounterZone encounterZone: {
                if (_componentContext.TryResolve<IRecordEditorVM<EncounterZone, IEncounterZoneGetter>>(out var editor)) {
                    control = editor.CreateControl(encounterZone);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case LightingTemplate lightingTemplate: {
                if (_componentContext.TryResolve<IRecordEditorVM<LightingTemplate, ILightingTemplateGetter>>(out var editor)) {
                    control = editor.CreateControl(lightingTemplate);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Location location: {
                if (_componentContext.TryResolve<IRecordEditorVM<Location, ILocationGetter>>(out var editor)) {
                    control = editor.CreateControl(location);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case LocationReferenceType locationReferenceType: {
                if (_componentContext.TryResolve<IRecordEditorVM<LocationReferenceType, ILocationReferenceTypeGetter>>(out var editor)) {
                    control = editor.CreateControl(locationReferenceType);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case ShaderParticleGeometry shaderParticleGeometry: {
                if (_componentContext.TryResolve<IRecordEditorVM<ShaderParticleGeometry, IShaderParticleGeometryGetter>>(out var editor)) {
                    control = editor.CreateControl(shaderParticleGeometry);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case VisualEffect visualEffect: {
                if (_componentContext.TryResolve<IRecordEditorVM<VisualEffect, IVisualEffectGetter>>(out var editor)) {
                    control = editor.CreateControl(visualEffect);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Water water: {
                if (_componentContext.TryResolve<IRecordEditorVM<Water, IWaterGetter>>(out var editor)) {
                    control = editor.CreateControl(water);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Weather weather: {
                if (_componentContext.TryResolve<IRecordEditorVM<Weather, IWeatherGetter>>(out var editor)) {
                    control = editor.CreateControl(weather);
                    recordEditor = editor;
                    return true;
                }
                break;
            }

            case Activator activator: {
                if (_componentContext.TryResolve<IRecordEditorVM<Activator, IActivatorGetter>>(out var editor)) {
                    control = editor.CreateControl(activator);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Container container: {
                if (_componentContext.TryResolve<IRecordEditorVM<Container, IContainerGetter>>(out var editor)) {
                    control = editor.CreateControl(container);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Door door: {
                if (_componentContext.TryResolve<IRecordEditorVM<Door, IDoorGetter>>(out var editor)) {
                    control = editor.CreateControl(door);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Flora flora: {
                if (_componentContext.TryResolve<IRecordEditorVM<Flora, IFloraGetter>>(out var editor)) {
                    control = editor.CreateControl(flora);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Furniture furniture: {
                if (_componentContext.TryResolve<IRecordEditorVM<Furniture, IFurnitureGetter>>(out var editor)) {
                    control = editor.CreateControl(furniture);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Grass grass: {
                if (_componentContext.TryResolve<IRecordEditorVM<Grass, IGrassGetter>>(out var editor)) {
                    control = editor.CreateControl(grass);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Light light: {
                if (_componentContext.TryResolve<IRecordEditorVM<Light, ILightGetter>>(out var editor)) {
                    control = editor.CreateControl(light);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case MoveableStatic moveableStatic: {
                if (_componentContext.TryResolve<IRecordEditorVM<MoveableStatic, IMoveableStaticGetter>>(out var editor)) {
                    control = editor.CreateControl(moveableStatic);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Static @static: {
                if (_componentContext.TryResolve<IRecordEditorVM<Static, IStaticGetter>>(out var editor)) {
                    control = editor.CreateControl(@static);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
            case Tree tree: {
                if (_componentContext.TryResolve<IRecordEditorVM<Tree, ITreeGetter>>(out var editor)) {
                    control = editor.CreateControl(tree);
                    recordEditor = editor;
                    return true;
                }
                break;
            }
        }

        control = null;
        recordEditor = null;
        return false;
    }
}
