using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim;

public static class MajorRecordExtension {
    public static IEnumerable<ExtendedList<Condition>> GetConditions(this IMajorRecord record) {
        switch (record) {
            case IQuest quest:
                yield return quest.DialogConditions;
                yield return quest.EventConditions;

                foreach (var objective in quest.Objectives) {
                    foreach (var target in objective.Targets) {
                        yield return target.Conditions;
                    }
                }

                foreach (var stage in quest.Stages) {
                    foreach (var logEntry in stage.LogEntries) {
                        yield return logEntry.Conditions;
                    }
                }

                foreach (var alias in quest.Aliases) {
                    yield return alias.Conditions;
                }
                break;
            case IDialogResponses responses:
                yield return responses.Conditions;

                break;
            case ILoadScreen loadScreen:
                yield return loadScreen.Conditions;

                break;

            case IMessage message:
                foreach (var menuButton in message.MenuButtons) {
                    yield return menuButton.Conditions;
                }
                break;
            case IPerk perk:
                yield return perk.Conditions;

                foreach (var effect in perk.Effects) {
                    foreach (var perkCondition in effect.Conditions) {
                        yield return perkCondition.Conditions;
                    }
                }
                break;
            case IMusicTrack musicTrack:
                if (musicTrack.Conditions is not null) yield return musicTrack.Conditions;

                break;
            case ISoundDescriptor soundDescriptor:
                yield return soundDescriptor.Conditions;

                break;
            case IFaction faction:
                if (faction.Conditions is not null) yield return faction.Conditions;

                break;
            case IPackage package:
                yield return package.Conditions;

                break;
            case IScene scene:
                yield return scene.Conditions;

                foreach (var phase in scene.Phases) {
                    yield return phase.StartConditions;
                    yield return phase.CompletionConditions;
                }
                break;
            case IStoryManagerBranchNode storyManagerBranchNode:
                yield return storyManagerBranchNode.Conditions;

                break;
            case IStoryManagerEventNode storyManagerEventNode:
                yield return storyManagerEventNode.Conditions;

                break;
            case IStoryManagerQuestNode storyManagerQuestNode:
                yield return storyManagerQuestNode.Conditions;

                break;
            case IConstructibleObject constructibleObject:
                yield return constructibleObject.Conditions;

                break;
            case IIdleAnimation idleAnimation:
                yield return idleAnimation.Conditions;

                break;
            case IIngredient ingredient:
                foreach (var effect in ingredient.Effects) {
                    yield return effect.Conditions;
                }
                break;
            case IIngestible ingestible:
                foreach (var effect in ingestible.Effects) {
                    yield return effect.Conditions;
                }
                break;
            case IObjectEffect objectEffect:
                foreach (var effect in objectEffect.Effects) {
                    yield return effect.Conditions;
                }
                break;
            case IScroll scroll:
                foreach (var effect in scroll.Effects) {
                    yield return effect.Conditions;
                }
                break;
            case ISpell spell:
                foreach (var effect in spell.Effects) {
                    yield return effect.Conditions;
                }
                break;
            case IMagicEffect magicEffect:
                yield return magicEffect.Conditions;

                break;
            default:
                yield break;
        }
    }

    public static IEnumerable<IReadOnlyList<IConditionGetter>> GetConditions(this IMajorRecordGetter record) {
        switch (record) {
            case IQuestGetter quest:
                yield return quest.DialogConditions;
                yield return quest.EventConditions;

                foreach (var objective in quest.Objectives) {
                    foreach (var target in objective.Targets) {
                        yield return target.Conditions;
                    }
                }

                foreach (var stage in quest.Stages) {
                    foreach (var logEntry in stage.LogEntries) {
                        yield return logEntry.Conditions;
                    }
                }

                foreach (var alias in quest.Aliases) {
                    yield return alias.Conditions;
                }
                break;
            case IDialogResponsesGetter responses:
                yield return responses.Conditions;

                break;
            case ILoadScreenGetter loadScreen:
                yield return loadScreen.Conditions;

                break;

            case IMessageGetter message:
                foreach (var menuButton in message.MenuButtons) {
                    yield return menuButton.Conditions;
                }
                break;
            case IPerkGetter perk:
                yield return perk.Conditions;

                foreach (var effect in perk.Effects) {
                    foreach (var perkCondition in effect.Conditions) {
                        yield return perkCondition.Conditions;
                    }
                }
                break;
            case IMusicTrackGetter musicTrack:
                if (musicTrack.Conditions is not null) yield return musicTrack.Conditions;

                break;
            case ISoundDescriptorGetter soundDescriptor:
                yield return soundDescriptor.Conditions;

                break;
            case IFactionGetter faction:
                if (faction.Conditions is not null) yield return faction.Conditions;

                break;
            case IPackageGetter package:
                yield return package.Conditions;

                break;
            case ISceneGetter scene:
                yield return scene.Conditions;

                foreach (var phase in scene.Phases) {
                    yield return phase.StartConditions;
                    yield return phase.CompletionConditions;
                }
                break;
            case IStoryManagerBranchNodeGetter storyManagerBranchNode:
                yield return storyManagerBranchNode.Conditions;

                break;
            case IStoryManagerEventNodeGetter storyManagerEventNode:
                yield return storyManagerEventNode.Conditions;

                break;
            case IStoryManagerQuestNodeGetter storyManagerQuestNode:
                yield return storyManagerQuestNode.Conditions;

                break;
            case IConstructibleObjectGetter constructibleObject:
                yield return constructibleObject.Conditions;

                break;
            case IIdleAnimationGetter idleAnimation:
                yield return idleAnimation.Conditions;

                break;
            case IIngredientGetter ingredient:
                foreach (var effect in ingredient.Effects) {
                    yield return effect.Conditions;
                }
                break;
            case IIngestibleGetter ingestible:
                foreach (var effect in ingestible.Effects) {
                    yield return effect.Conditions;
                }
                break;
            case IObjectEffectGetter objectEffect:
                foreach (var effect in objectEffect.Effects) {
                    yield return effect.Conditions;
                }
                break;
            case IScrollGetter scroll:
                foreach (var effect in scroll.Effects) {
                    yield return effect.Conditions;
                }
                break;
            case ISpellGetter spell:
                foreach (var effect in spell.Effects) {
                    yield return effect.Conditions;
                }
                break;
            case IMagicEffectGetter magicEffect:
                yield return magicEffect.Conditions;

                break;
            default:
                yield break;
        }
    }
}
