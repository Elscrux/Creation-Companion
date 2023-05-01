using System.Diagnostics.CodeAnalysis;
using Autofac;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels.Record.Editor;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Location = Mutagen.Bethesda.Skyrim.Location;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Editor;

public sealed class SkyrimRecordEditorFactory : IRecordEditorFactory {
    private readonly ILifetimeScope _lifetimeScope;

    public SkyrimRecordEditorFactory(
        ILifetimeScope lifetimeScope) {
        _lifetimeScope = lifetimeScope;
    }

    public bool FromType(IMajorRecord record, [MaybeNullWhen(false)] out Control control, [MaybeNullWhen(false)] out IRecordEditorVM recordEditor) {
        var newScope = _lifetimeScope.BeginLifetimeScope();
        switch (record) {
            case Npc npc: {
                if (newScope.TryResolve<IRecordEditorVM<Npc, INpcGetter>>(out var editor)) {
                    control = editor.CreateControl(npc);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case ActionRecord actionRecord: {
                if (newScope.TryResolve<IRecordEditorVM<ActionRecord, IActionRecordGetter>>(out var editor)) {
                    control = editor.CreateControl(actionRecord);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case BodyPartData bodyPartData: {
                if (newScope.TryResolve<IRecordEditorVM<BodyPartData, IBodyPartDataGetter>>(out var editor)) {
                    control = editor.CreateControl(bodyPartData);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case LeveledNpc leveledNpc: {
                if (newScope.TryResolve<IRecordEditorVM<LeveledNpc, ILeveledNpcGetter>>(out var editor)) {
                    control = editor.CreateControl(leveledNpc);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Perk perk: {
                if (newScope.TryResolve<IRecordEditorVM<Perk, IPerkGetter>>(out var editor)) {
                    control = editor.CreateControl(perk);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case TalkingActivator talkingActivator: {
                if (newScope.TryResolve<IRecordEditorVM<TalkingActivator, ITalkingActivatorGetter>>(out var editor)) {
                    control = editor.CreateControl(talkingActivator);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }

            case AcousticSpace acousticSpace: {
                if (newScope.TryResolve<IRecordEditorVM<AcousticSpace, IAcousticSpaceGetter>>(out var editor)) {
                    control = editor.CreateControl(acousticSpace);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case MusicTrack musicTrack: {
                if (newScope.TryResolve<IRecordEditorVM<MusicTrack, IMusicTrackGetter>>(out var editor)) {
                    control = editor.CreateControl(musicTrack);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case MusicType musicType: {
                if (newScope.TryResolve<IRecordEditorVM<MusicType, IMusicTypeGetter>>(out var editor)) {
                    control = editor.CreateControl(musicType);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case ReverbParameters reverbParameters: {
                if (newScope.TryResolve<IRecordEditorVM<ReverbParameters, IReverbParametersGetter>>(out var editor)) {
                    control = editor.CreateControl(reverbParameters);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case SoundCategory soundCategory: {
                if (newScope.TryResolve<IRecordEditorVM<SoundCategory, ISoundCategoryGetter>>(out var editor)) {
                    control = editor.CreateControl(soundCategory);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case SoundDescriptor soundDescriptor: {
                if (newScope.TryResolve<IRecordEditorVM<SoundDescriptor, ISoundDescriptorGetter>>(out var editor)) {
                    control = editor.CreateControl(soundDescriptor);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case SoundMarker soundMarker: {
                if (newScope.TryResolve<IRecordEditorVM<SoundMarker, ISoundMarkerGetter>>(out var editor)) {
                    control = editor.CreateControl(soundMarker);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case SoundOutputModel soundOutputModel: {
                if (newScope.TryResolve<IRecordEditorVM<SoundOutputModel, ISoundOutputModelGetter>>(out var editor)) {
                    control = editor.CreateControl(soundOutputModel);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }

            case AssociationType associationType: {
                if (newScope.TryResolve<IRecordEditorVM<AssociationType, IAssociationTypeGetter>>(out var editor)) {
                    control = editor.CreateControl(associationType);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Class @class: {
                if (newScope.TryResolve<IRecordEditorVM<Class, IClassGetter>>(out var editor)) {
                    control = editor.CreateControl(@class);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case EquipType equipType: {
                if (newScope.TryResolve<IRecordEditorVM<EquipType, IEquipTypeGetter>>(out var editor)) {
                    control = editor.CreateControl(equipType);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Faction faction: {
                if (newScope.TryResolve<IRecordEditorVM<Faction, IFactionGetter>>(out var editor)) {
                    control = editor.CreateControl(faction);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case HeadPart headPart: {
                if (newScope.TryResolve<IRecordEditorVM<HeadPart, IHeadPartGetter>>(out var editor)) {
                    control = editor.CreateControl(headPart);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case MovementType movementType: {
                if (newScope.TryResolve<IRecordEditorVM<MovementType, IMovementTypeGetter>>(out var editor)) {
                    control = editor.CreateControl(movementType);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Package package: {
                if (newScope.TryResolve<IRecordEditorVM<Package, IPackageGetter>>(out var editor)) {
                    control = editor.CreateControl(package);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Quest quest: {
                if (newScope.TryResolve<IRecordEditorVM<Quest, IQuestGetter>>(out var editor)) {
                    control = editor.CreateControl(quest);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Race race: {
                if (newScope.TryResolve<IRecordEditorVM<Race, IRaceGetter>>(out var editor)) {
                    control = editor.CreateControl(race);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Relationship relationship: {
                if (newScope.TryResolve<IRecordEditorVM<Relationship, IRelationshipGetter>>(out var editor)) {
                    control = editor.CreateControl(relationship);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case StoryManagerEventNode storyManagerEventNode: {
                if (newScope.TryResolve<IRecordEditorVM<StoryManagerEventNode, IStoryManagerEventNodeGetter>>(out var editor)) {
                    control = editor.CreateControl(storyManagerEventNode);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case VoiceType voiceType: {
                if (newScope.TryResolve<IRecordEditorVM<VoiceType, IVoiceTypeGetter>>(out var editor)) {
                    control = editor.CreateControl(voiceType);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }

            case Ammunition ammunition: {
                if (newScope.TryResolve<IRecordEditorVM<Ammunition, IAmmunitionGetter>>(out var editor)) {
                    control = editor.CreateControl(ammunition);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Armor armor: {
                if (newScope.TryResolve<IRecordEditorVM<Armor, IArmorGetter>>(out var editor)) {
                    control = editor.CreateControl(armor);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case ArmorAddon armorAddon: {
                if (newScope.TryResolve<IRecordEditorVM<ArmorAddon, IArmorAddonGetter>>(out var editor)) {
                    control = editor.CreateControl(armorAddon);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Book book: {
                if (newScope.TryResolve<IRecordEditorVM<Book, IBookGetter>>(out var editor)) {
                    control = editor.CreateControl(book);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case ConstructibleObject constructibleObject: {
                if (newScope.TryResolve<IRecordEditorVM<ConstructibleObject, IConstructibleObjectGetter>>(out var editor)) {
                    control = editor.CreateControl(constructibleObject);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Ingredient ingredient: {
                if (newScope.TryResolve<IRecordEditorVM<Ingredient, IIngredientGetter>>(out var editor)) {
                    control = editor.CreateControl(ingredient);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Key key: {
                if (newScope.TryResolve<IRecordEditorVM<Key, IKeyGetter>>(out var editor)) {
                    control = editor.CreateControl(key);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case LeveledItem leveledItem: {
                if (newScope.TryResolve<IRecordEditorVM<LeveledItem, ILeveledItemGetter>>(out var editor)) {
                    control = editor.CreateControl(leveledItem);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case MiscItem miscItem: {
                if (newScope.TryResolve<IRecordEditorVM<MiscItem, IMiscItemGetter>>(out var editor)) {
                    control = editor.CreateControl(miscItem);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Outfit outfit: {
                if (newScope.TryResolve<IRecordEditorVM<Outfit, IOutfitGetter>>(out var editor)) {
                    control = editor.CreateControl(outfit);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case SoulGem soulGem: {
                if (newScope.TryResolve<IRecordEditorVM<SoulGem, ISoulGemGetter>>(out var editor)) {
                    control = editor.CreateControl(soulGem);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Weapon weapon: {
                if (newScope.TryResolve<IRecordEditorVM<Weapon, IWeaponGetter>>(out var editor)) {
                    control = editor.CreateControl(weapon);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }

            case DualCastData dualCastData: {
                if (newScope.TryResolve<IRecordEditorVM<DualCastData, IDualCastDataGetter>>(out var editor)) {
                    control = editor.CreateControl(dualCastData);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case ObjectEffect objectEffect: {
                if (newScope.TryResolve<IRecordEditorVM<ObjectEffect, IObjectEffectGetter>>(out var editor)) {
                    control = editor.CreateControl(objectEffect);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case LeveledSpell leveledSpell: {
                if (newScope.TryResolve<IRecordEditorVM<LeveledSpell, ILeveledSpellGetter>>(out var editor)) {
                    control = editor.CreateControl(leveledSpell);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case MagicEffect magicEffect: {
                if (newScope.TryResolve<IRecordEditorVM<MagicEffect, IMagicEffectGetter>>(out var editor)) {
                    control = editor.CreateControl(magicEffect);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Ingestible ingestible: {
                if (newScope.TryResolve<IRecordEditorVM<Ingestible, IIngestibleGetter>>(out var editor)) {
                    control = editor.CreateControl(ingestible);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Scroll scroll: {
                if (newScope.TryResolve<IRecordEditorVM<Scroll, IScrollGetter>>(out var editor)) {
                    control = editor.CreateControl(scroll);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Spell spell: {
                if (newScope.TryResolve<IRecordEditorVM<Spell, ISpellGetter>>(out var editor)) {
                    control = editor.CreateControl(spell);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case WordOfPower wordOfPower: {
                if (newScope.TryResolve<IRecordEditorVM<WordOfPower, IWordOfPowerGetter>>(out var editor)) {
                    control = editor.CreateControl(wordOfPower);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }

            case AnimatedObject animatedObject: {
                if (newScope.TryResolve<IRecordEditorVM<AnimatedObject, IAnimatedObjectGetter>>(out var editor)) {
                    control = editor.CreateControl(animatedObject);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case ArtObject artObject: {
                if (newScope.TryResolve<IRecordEditorVM<ArtObject, IArtObjectGetter>>(out var editor)) {
                    control = editor.CreateControl(artObject);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case CollisionLayer collisionLayer: {
                if (newScope.TryResolve<IRecordEditorVM<CollisionLayer, ICollisionLayerGetter>>(out var editor)) {
                    control = editor.CreateControl(collisionLayer);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case FormList formList: {
                if (newScope.TryResolve<IRecordEditorVM<FormList, IFormListGetter>>(out var editor)) {
                    control = editor.CreateControl(formList);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Global global: {
                if (newScope.TryResolve<IRecordEditorVM<Global, IGlobalGetter>>(out var editor)) {
                    control = editor.CreateControl(global);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case IdleMarker idleMarker: {
                if (newScope.TryResolve<IRecordEditorVM<IdleMarker, IIdleMarkerGetter>>(out var editor)) {
                    control = editor.CreateControl(idleMarker);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Keyword keyword: {
                if (newScope.TryResolve<IRecordEditorVM<Keyword, IKeywordGetter>>(out var editor)) {
                    control = editor.CreateControl(keyword);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case LandscapeTexture landscapeTexture: {
                if (newScope.TryResolve<IRecordEditorVM<LandscapeTexture, ILandscapeTextureGetter>>(out var editor)) {
                    control = editor.CreateControl(landscapeTexture);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case LoadScreen loadScreen: {
                if (newScope.TryResolve<IRecordEditorVM<LoadScreen, ILoadScreenGetter>>(out var editor)) {
                    control = editor.CreateControl(loadScreen);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case MaterialObject materialObject: {
                if (newScope.TryResolve<IRecordEditorVM<MaterialObject, IMaterialObjectGetter>>(out var editor)) {
                    control = editor.CreateControl(materialObject);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Message message: {
                if (newScope.TryResolve<IRecordEditorVM<Message, IMessageGetter>>(out var editor)) {
                    control = editor.CreateControl(message);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case TextureSet textureSet: {
                if (newScope.TryResolve<IRecordEditorVM<TextureSet, ITextureSetGetter>>(out var editor)) {
                    control = editor.CreateControl(textureSet);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }

            case AddonNode addonNode: {
                if (newScope.TryResolve<IRecordEditorVM<AddonNode, IAddonNodeGetter>>(out var editor)) {
                    control = editor.CreateControl(addonNode);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case CameraShot cameraShot: {
                if (newScope.TryResolve<IRecordEditorVM<CameraShot, ICameraShotGetter>>(out var editor)) {
                    control = editor.CreateControl(cameraShot);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Debris debris: {
                if (newScope.TryResolve<IRecordEditorVM<Debris, IDebrisGetter>>(out var editor)) {
                    control = editor.CreateControl(debris);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case EffectShader effectShader: {
                if (newScope.TryResolve<IRecordEditorVM<EffectShader, IEffectShaderGetter>>(out var editor)) {
                    control = editor.CreateControl(effectShader);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Explosion explosion: {
                if (newScope.TryResolve<IRecordEditorVM<Explosion, IExplosionGetter>>(out var editor)) {
                    control = editor.CreateControl(explosion);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Footstep footstep: {
                if (newScope.TryResolve<IRecordEditorVM<Footstep, IFootstepGetter>>(out var editor)) {
                    control = editor.CreateControl(footstep);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case FootstepSet footstepSet: {
                if (newScope.TryResolve<IRecordEditorVM<FootstepSet, IFootstepSetGetter>>(out var editor)) {
                    control = editor.CreateControl(footstepSet);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Hazard hazard: {
                if (newScope.TryResolve<IRecordEditorVM<Hazard, IHazardGetter>>(out var editor)) {
                    control = editor.CreateControl(hazard);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case ImageSpace imageSpace: {
                if (newScope.TryResolve<IRecordEditorVM<ImageSpace, IImageSpaceGetter>>(out var editor)) {
                    control = editor.CreateControl(imageSpace);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case ImageSpaceAdapter imageSpaceAdapter: {
                if (newScope.TryResolve<IRecordEditorVM<ImageSpaceAdapter, IImageSpaceAdapterGetter>>(out var editor)) {
                    control = editor.CreateControl(imageSpaceAdapter);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case ImpactDataSet impactDataSet: {
                if (newScope.TryResolve<IRecordEditorVM<ImpactDataSet, IImpactDataSetGetter>>(out var editor)) {
                    control = editor.CreateControl(impactDataSet);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case LensFlare lensFlare: {
                if (newScope.TryResolve<IRecordEditorVM<LensFlare, ILensFlareGetter>>(out var editor)) {
                    control = editor.CreateControl(lensFlare);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case MaterialType materialType: {
                if (newScope.TryResolve<IRecordEditorVM<MaterialType, IMaterialTypeGetter>>(out var editor)) {
                    control = editor.CreateControl(materialType);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Projectile projectile: {
                if (newScope.TryResolve<IRecordEditorVM<Projectile, IProjectileGetter>>(out var editor)) {
                    control = editor.CreateControl(projectile);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case VolumetricLighting volumetricLighting: {
                if (newScope.TryResolve<IRecordEditorVM<VolumetricLighting, IVolumetricLightingGetter>>(out var editor)) {
                    control = editor.CreateControl(volumetricLighting);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }

            case Climate climate: {
                if (newScope.TryResolve<IRecordEditorVM<Climate, IClimateGetter>>(out var editor)) {
                    control = editor.CreateControl(climate);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case EncounterZone encounterZone: {
                if (newScope.TryResolve<IRecordEditorVM<EncounterZone, IEncounterZoneGetter>>(out var editor)) {
                    control = editor.CreateControl(encounterZone);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case LightingTemplate lightingTemplate: {
                if (newScope.TryResolve<IRecordEditorVM<LightingTemplate, ILightingTemplateGetter>>(out var editor)) {
                    control = editor.CreateControl(lightingTemplate);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Location location: {
                if (newScope.TryResolve<IRecordEditorVM<Location, ILocationGetter>>(out var editor)) {
                    control = editor.CreateControl(location);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case LocationReferenceType locationReferenceType: {
                if (newScope.TryResolve<IRecordEditorVM<LocationReferenceType, ILocationReferenceTypeGetter>>(out var editor)) {
                    control = editor.CreateControl(locationReferenceType);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case ShaderParticleGeometry shaderParticleGeometry: {
                if (newScope.TryResolve<IRecordEditorVM<ShaderParticleGeometry, IShaderParticleGeometryGetter>>(out var editor)) {
                    control = editor.CreateControl(shaderParticleGeometry);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case VisualEffect visualEffect: {
                if (newScope.TryResolve<IRecordEditorVM<VisualEffect, IVisualEffectGetter>>(out var editor)) {
                    control = editor.CreateControl(visualEffect);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Water water: {
                if (newScope.TryResolve<IRecordEditorVM<Water, IWaterGetter>>(out var editor)) {
                    control = editor.CreateControl(water);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Weather weather: {
                if (newScope.TryResolve<IRecordEditorVM<Weather, IWeatherGetter>>(out var editor)) {
                    control = editor.CreateControl(weather);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }

            case Activator activator: {
                if (newScope.TryResolve<IRecordEditorVM<Activator, IActivatorGetter>>(out var editor)) {
                    control = editor.CreateControl(activator);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Container container: {
                if (newScope.TryResolve<IRecordEditorVM<Container, IContainerGetter>>(out var editor)) {
                    control = editor.CreateControl(container);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Door door: {
                if (newScope.TryResolve<IRecordEditorVM<Door, IDoorGetter>>(out var editor)) {
                    control = editor.CreateControl(door);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Flora flora: {
                if (newScope.TryResolve<IRecordEditorVM<Flora, IFloraGetter>>(out var editor)) {
                    control = editor.CreateControl(flora);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Furniture furniture: {
                if (newScope.TryResolve<IRecordEditorVM<Furniture, IFurnitureGetter>>(out var editor)) {
                    control = editor.CreateControl(furniture);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Grass grass: {
                if (newScope.TryResolve<IRecordEditorVM<Grass, IGrassGetter>>(out var editor)) {
                    control = editor.CreateControl(grass);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Light light: {
                if (newScope.TryResolve<IRecordEditorVM<Light, ILightGetter>>(out var editor)) {
                    control = editor.CreateControl(light);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case MoveableStatic moveableStatic: {
                if (newScope.TryResolve<IRecordEditorVM<MoveableStatic, IMoveableStaticGetter>>(out var editor)) {
                    control = editor.CreateControl(moveableStatic);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Static @static: {
                if (newScope.TryResolve<IRecordEditorVM<Static, IStaticGetter>>(out var editor)) {
                    control = editor.CreateControl(@static);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
                    return true;
                }
                break;
            }
            case Tree tree: {
                if (newScope.TryResolve<IRecordEditorVM<Tree, ITreeGetter>>(out var editor)) {
                    control = editor.CreateControl(tree);
                    recordEditor = editor;
                    newScope.DisposeWith(recordEditor);
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
