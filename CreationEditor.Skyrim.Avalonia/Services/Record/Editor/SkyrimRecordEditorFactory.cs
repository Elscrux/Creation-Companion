using System.Diagnostics.CodeAnalysis;
using Autofac;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels.Record.Editor;
using CreationEditor.Skyrim.Avalonia.Views.Record.Editor.MajorRecord.Book;
using CreationEditor.Skyrim.Avalonia.Views.Record.Editor.MajorRecord.Faction;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Activator = Mutagen.Bethesda.Skyrim.Activator;
using Location = Mutagen.Bethesda.Skyrim.Location;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Editor;

public sealed class SkyrimRecordEditorFactory(ILifetimeScope lifetimeScope) : IRecordEditorFactory {
    public bool FromType(IMajorRecord record, [MaybeNullWhen(false)] out Control control, [MaybeNullWhen(false)] out IRecordEditorVM recordEditorVm) {
        var newScope = lifetimeScope.BeginLifetimeScope();
        switch (record) {
            case Npc npc: {
                if (newScope.TryResolve<IRecordEditorVM<Npc, INpcGetter>>(TypedParameter.From(npc), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case ActionRecord actionRecord: {
                if (newScope.TryResolve<IRecordEditorVM<ActionRecord, IActionRecordGetter>>(TypedParameter.From(actionRecord), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case BodyPartData bodyPartData: {
                if (newScope.TryResolve<IRecordEditorVM<BodyPartData, IBodyPartDataGetter>>(TypedParameter.From(bodyPartData), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case LeveledNpc leveledNpc: {
                if (newScope.TryResolve<IRecordEditorVM<LeveledNpc, ILeveledNpcGetter>>(TypedParameter.From(leveledNpc), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Perk perk: {
                if (newScope.TryResolve<IRecordEditorVM<Perk, IPerkGetter>>(TypedParameter.From(perk), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case TalkingActivator talkingActivator: {
                if (newScope.TryResolve<IRecordEditorVM<TalkingActivator, ITalkingActivatorGetter>>(TypedParameter.From(talkingActivator),
                    out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }

            case AcousticSpace acousticSpace: {
                if (newScope.TryResolve<IRecordEditorVM<AcousticSpace, IAcousticSpaceGetter>>(TypedParameter.From(acousticSpace), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case MusicTrack musicTrack: {
                if (newScope.TryResolve<IRecordEditorVM<MusicTrack, IMusicTrackGetter>>(TypedParameter.From(musicTrack), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case MusicType musicType: {
                if (newScope.TryResolve<IRecordEditorVM<MusicType, IMusicTypeGetter>>(TypedParameter.From(musicType), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case ReverbParameters reverbParameters: {
                if (newScope.TryResolve<IRecordEditorVM<ReverbParameters, IReverbParametersGetter>>(TypedParameter.From(reverbParameters),
                    out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case SoundCategory soundCategory: {
                if (newScope.TryResolve<IRecordEditorVM<SoundCategory, ISoundCategoryGetter>>(TypedParameter.From(soundCategory), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case SoundDescriptor soundDescriptor: {
                if (newScope.TryResolve<IRecordEditorVM<SoundDescriptor, ISoundDescriptorGetter>>(TypedParameter.From(soundDescriptor), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case SoundMarker soundMarker: {
                if (newScope.TryResolve<IRecordEditorVM<SoundMarker, ISoundMarkerGetter>>(TypedParameter.From(soundMarker), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case SoundOutputModel soundOutputModel: {
                if (newScope.TryResolve<IRecordEditorVM<SoundOutputModel, ISoundOutputModelGetter>>(TypedParameter.From(soundOutputModel),
                    out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }

            case AssociationType associationType: {
                if (newScope.TryResolve<IRecordEditorVM<AssociationType, IAssociationTypeGetter>>(TypedParameter.From(associationType), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Class @class: {
                if (newScope.TryResolve<IRecordEditorVM<Class, IClassGetter>>(TypedParameter.From(@class), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case EquipType equipType: {
                if (newScope.TryResolve<IRecordEditorVM<EquipType, IEquipTypeGetter>>(TypedParameter.From(equipType), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Faction faction: {
                if (newScope.TryResolve<IRecordEditorVM<Faction, IFactionGetter>>(TypedParameter.From(faction), out var vm)) {
                    control = new FactionEditor(vm);
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case HeadPart headPart: {
                if (newScope.TryResolve<IRecordEditorVM<HeadPart, IHeadPartGetter>>(TypedParameter.From(headPart), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case MovementType movementType: {
                if (newScope.TryResolve<IRecordEditorVM<MovementType, IMovementTypeGetter>>(TypedParameter.From(movementType), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Package package: {
                if (newScope.TryResolve<IRecordEditorVM<Package, IPackageGetter>>(TypedParameter.From(package), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Quest quest: {
                if (newScope.TryResolve<IRecordEditorVM<Quest, IQuestGetter>>(TypedParameter.From(quest), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Race race: {
                if (newScope.TryResolve<IRecordEditorVM<Race, IRaceGetter>>(TypedParameter.From(race), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Relationship relationship: {
                if (newScope.TryResolve<IRecordEditorVM<Relationship, IRelationshipGetter>>(TypedParameter.From(relationship), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case StoryManagerEventNode storyManagerEventNode: {
                if (newScope.TryResolve<IRecordEditorVM<StoryManagerEventNode, IStoryManagerEventNodeGetter>>(
                    TypedParameter.From(storyManagerEventNode),
                    out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case VoiceType voiceType: {
                if (newScope.TryResolve<IRecordEditorVM<VoiceType, IVoiceTypeGetter>>(TypedParameter.From(voiceType), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }

            case Ammunition ammunition: {
                if (newScope.TryResolve<IRecordEditorVM<Ammunition, IAmmunitionGetter>>(TypedParameter.From(ammunition), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Armor armor: {
                if (newScope.TryResolve<IRecordEditorVM<Armor, IArmorGetter>>(TypedParameter.From(armor), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case ArmorAddon armorAddon: {
                if (newScope.TryResolve<IRecordEditorVM<ArmorAddon, IArmorAddonGetter>>(TypedParameter.From(armorAddon), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Book book: {
                if (newScope.TryResolve<IRecordEditorVM<Book, IBookGetter>>(TypedParameter.From(book), out var vm)) {
                    control = new BookEditor(vm);
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case ConstructibleObject constructibleObject: {
                if (newScope.TryResolve<IRecordEditorVM<ConstructibleObject, IConstructibleObjectGetter>>(TypedParameter.From(constructibleObject),
                    out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Ingredient ingredient: {
                if (newScope.TryResolve<IRecordEditorVM<Ingredient, IIngredientGetter>>(TypedParameter.From(ingredient), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Key key: {
                if (newScope.TryResolve<IRecordEditorVM<Key, IKeyGetter>>(TypedParameter.From(key), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case LeveledItem leveledItem: {
                if (newScope.TryResolve<IRecordEditorVM<LeveledItem, ILeveledItemGetter>>(TypedParameter.From(leveledItem), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case MiscItem miscItem: {
                if (newScope.TryResolve<IRecordEditorVM<MiscItem, IMiscItemGetter>>(TypedParameter.From(miscItem), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Outfit outfit: {
                if (newScope.TryResolve<IRecordEditorVM<Outfit, IOutfitGetter>>(TypedParameter.From(outfit), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case SoulGem soulGem: {
                if (newScope.TryResolve<IRecordEditorVM<SoulGem, ISoulGemGetter>>(TypedParameter.From(soulGem), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Weapon weapon: {
                if (newScope.TryResolve<IRecordEditorVM<Weapon, IWeaponGetter>>(TypedParameter.From(weapon), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }

            case DualCastData dualCastData: {
                if (newScope.TryResolve<IRecordEditorVM<DualCastData, IDualCastDataGetter>>(TypedParameter.From(dualCastData), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case ObjectEffect objectEffect: {
                if (newScope.TryResolve<IRecordEditorVM<ObjectEffect, IObjectEffectGetter>>(TypedParameter.From(objectEffect), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case LeveledSpell leveledSpell: {
                if (newScope.TryResolve<IRecordEditorVM<LeveledSpell, ILeveledSpellGetter>>(TypedParameter.From(leveledSpell), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case MagicEffect magicEffect: {
                if (newScope.TryResolve<IRecordEditorVM<MagicEffect, IMagicEffectGetter>>(TypedParameter.From(magicEffect), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Ingestible ingestible: {
                if (newScope.TryResolve<IRecordEditorVM<Ingestible, IIngestibleGetter>>(TypedParameter.From(ingestible), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Scroll scroll: {
                if (newScope.TryResolve<IRecordEditorVM<Scroll, IScrollGetter>>(TypedParameter.From(scroll), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Spell spell: {
                if (newScope.TryResolve<IRecordEditorVM<Spell, ISpellGetter>>(TypedParameter.From(spell), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case WordOfPower wordOfPower: {
                if (newScope.TryResolve<IRecordEditorVM<WordOfPower, IWordOfPowerGetter>>(TypedParameter.From(wordOfPower), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }

            case AnimatedObject animatedObject: {
                if (newScope.TryResolve<IRecordEditorVM<AnimatedObject, IAnimatedObjectGetter>>(TypedParameter.From(animatedObject), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case ArtObject artObject: {
                if (newScope.TryResolve<IRecordEditorVM<ArtObject, IArtObjectGetter>>(TypedParameter.From(artObject), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case CollisionLayer collisionLayer: {
                if (newScope.TryResolve<IRecordEditorVM<CollisionLayer, ICollisionLayerGetter>>(TypedParameter.From(collisionLayer), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case FormList formList: {
                if (newScope.TryResolve<IRecordEditorVM<FormList, IFormListGetter>>(TypedParameter.From(formList), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Global global: {
                if (newScope.TryResolve<IRecordEditorVM<Global, IGlobalGetter>>(TypedParameter.From(global), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case IdleMarker idleMarker: {
                if (newScope.TryResolve<IRecordEditorVM<IdleMarker, IIdleMarkerGetter>>(TypedParameter.From(idleMarker), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Keyword keyword: {
                if (newScope.TryResolve<IRecordEditorVM<Keyword, IKeywordGetter>>(TypedParameter.From(keyword), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case LandscapeTexture landscapeTexture: {
                if (newScope.TryResolve<IRecordEditorVM<LandscapeTexture, ILandscapeTextureGetter>>(TypedParameter.From(landscapeTexture),
                    out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case LoadScreen loadScreen: {
                if (newScope.TryResolve<IRecordEditorVM<LoadScreen, ILoadScreenGetter>>(TypedParameter.From(loadScreen), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case MaterialObject materialObject: {
                if (newScope.TryResolve<IRecordEditorVM<MaterialObject, IMaterialObjectGetter>>(TypedParameter.From(materialObject), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Message message: {
                if (newScope.TryResolve<IRecordEditorVM<Message, IMessageGetter>>(TypedParameter.From(message), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case TextureSet textureSet: {
                if (newScope.TryResolve<IRecordEditorVM<TextureSet, ITextureSetGetter>>(TypedParameter.From(textureSet), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }

            case AddonNode addonNode: {
                if (newScope.TryResolve<IRecordEditorVM<AddonNode, IAddonNodeGetter>>(TypedParameter.From(addonNode), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case CameraShot cameraShot: {
                if (newScope.TryResolve<IRecordEditorVM<CameraShot, ICameraShotGetter>>(TypedParameter.From(cameraShot), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Debris debris: {
                if (newScope.TryResolve<IRecordEditorVM<Debris, IDebrisGetter>>(TypedParameter.From(debris), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case EffectShader effectShader: {
                if (newScope.TryResolve<IRecordEditorVM<EffectShader, IEffectShaderGetter>>(TypedParameter.From(effectShader), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Explosion explosion: {
                if (newScope.TryResolve<IRecordEditorVM<Explosion, IExplosionGetter>>(TypedParameter.From(explosion), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Footstep footstep: {
                if (newScope.TryResolve<IRecordEditorVM<Footstep, IFootstepGetter>>(TypedParameter.From(footstep), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case FootstepSet footstepSet: {
                if (newScope.TryResolve<IRecordEditorVM<FootstepSet, IFootstepSetGetter>>(TypedParameter.From(footstepSet), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Hazard hazard: {
                if (newScope.TryResolve<IRecordEditorVM<Hazard, IHazardGetter>>(TypedParameter.From(hazard), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case ImageSpace imageSpace: {
                if (newScope.TryResolve<IRecordEditorVM<ImageSpace, IImageSpaceGetter>>(TypedParameter.From(imageSpace), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case ImageSpaceAdapter imageSpaceAdapter: {
                if (newScope.TryResolve<IRecordEditorVM<ImageSpaceAdapter, IImageSpaceAdapterGetter>>(TypedParameter.From(imageSpaceAdapter),
                    out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case ImpactDataSet impactDataSet: {
                if (newScope.TryResolve<IRecordEditorVM<ImpactDataSet, IImpactDataSetGetter>>(TypedParameter.From(impactDataSet), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case LensFlare lensFlare: {
                if (newScope.TryResolve<IRecordEditorVM<LensFlare, ILensFlareGetter>>(TypedParameter.From(lensFlare), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case MaterialType materialType: {
                if (newScope.TryResolve<IRecordEditorVM<MaterialType, IMaterialTypeGetter>>(TypedParameter.From(materialType), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Projectile projectile: {
                if (newScope.TryResolve<IRecordEditorVM<Projectile, IProjectileGetter>>(TypedParameter.From(projectile), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case VolumetricLighting volumetricLighting: {
                if (newScope.TryResolve<IRecordEditorVM<VolumetricLighting, IVolumetricLightingGetter>>(TypedParameter.From(volumetricLighting),
                    out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }

            case Climate climate: {
                if (newScope.TryResolve<IRecordEditorVM<Climate, IClimateGetter>>(TypedParameter.From(climate), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case EncounterZone encounterZone: {
                if (newScope.TryResolve<IRecordEditorVM<EncounterZone, IEncounterZoneGetter>>(TypedParameter.From(encounterZone), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case LightingTemplate lightingTemplate: {
                if (newScope.TryResolve<IRecordEditorVM<LightingTemplate, ILightingTemplateGetter>>(TypedParameter.From(lightingTemplate),
                    out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Location location: {
                if (newScope.TryResolve<IRecordEditorVM<Location, ILocationGetter>>(TypedParameter.From(location), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case LocationReferenceType locationReferenceType: {
                if (newScope.TryResolve<IRecordEditorVM<LocationReferenceType, ILocationReferenceTypeGetter>>(
                    TypedParameter.From(locationReferenceType),
                    out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case ShaderParticleGeometry shaderParticleGeometry: {
                if (newScope.TryResolve<IRecordEditorVM<ShaderParticleGeometry, IShaderParticleGeometryGetter>>(
                    TypedParameter.From(shaderParticleGeometry),
                    out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case VisualEffect visualEffect: {
                if (newScope.TryResolve<IRecordEditorVM<VisualEffect, IVisualEffectGetter>>(TypedParameter.From(visualEffect), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Water water: {
                if (newScope.TryResolve<IRecordEditorVM<Water, IWaterGetter>>(TypedParameter.From(water), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Weather weather: {
                if (newScope.TryResolve<IRecordEditorVM<Weather, IWeatherGetter>>(TypedParameter.From(weather), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }

            case Activator activator: {
                if (newScope.TryResolve<IRecordEditorVM<Activator, IActivatorGetter>>(TypedParameter.From(activator), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Container container: {
                if (newScope.TryResolve<IRecordEditorVM<Container, IContainerGetter>>(TypedParameter.From(container), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Door door: {
                if (newScope.TryResolve<IRecordEditorVM<Door, IDoorGetter>>(TypedParameter.From(door), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Flora flora: {
                if (newScope.TryResolve<IRecordEditorVM<Flora, IFloraGetter>>(TypedParameter.From(flora), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Furniture furniture: {
                if (newScope.TryResolve<IRecordEditorVM<Furniture, IFurnitureGetter>>(TypedParameter.From(furniture), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Grass grass: {
                if (newScope.TryResolve<IRecordEditorVM<Grass, IGrassGetter>>(TypedParameter.From(grass), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Light light: {
                if (newScope.TryResolve<IRecordEditorVM<Light, ILightGetter>>(TypedParameter.From(light), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case MoveableStatic moveableStatic: {
                if (newScope.TryResolve<IRecordEditorVM<MoveableStatic, IMoveableStaticGetter>>(TypedParameter.From(moveableStatic), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Static @static: {
                if (newScope.TryResolve<IRecordEditorVM<Static, IStaticGetter>>(TypedParameter.From(@static), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
            case Tree tree: {
                if (newScope.TryResolve<IRecordEditorVM<Tree, ITreeGetter>>(TypedParameter.From(tree), out var vm)) {
                    control = null!;
                    recordEditorVm = vm;
                    newScope.DisposeWith(vm.Core);
                    return true;
                }
                break;
            }
        }

        control = null;
        recordEditorVm = null;
        return false;
    }
}
