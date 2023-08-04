using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim;

public static class ModExtension {
    public static uint GetRecordCount(this IModGetter mod) {
        return mod switch {
            ISkyrimModGetter skyrimMod => skyrimMod.ModHeader.Stats.NumRecords,
            _ => throw new ArgumentOutOfRangeException(nameof(mod))
        };
    }

    public static IEnumerable<IReadOnlyList<IConditionGetter>> GetConditions(this IModGetter mod) {
        foreach (var quest in mod.EnumerateMajorRecords<IQuestGetter>()) {
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
        }

        foreach (var responses in mod.EnumerateMajorRecords<IDialogResponsesGetter>()) {
            yield return responses.Conditions;
        }

        foreach (var loadScreen in mod.EnumerateMajorRecords<ILoadScreenGetter>()) {
            yield return loadScreen.Conditions;
        }

        foreach (var message in mod.EnumerateMajorRecords<IMessageGetter>()) {
            foreach (var menuButton in message.MenuButtons) {
                yield return menuButton.Conditions;
            }
        }

        foreach (var perk in mod.EnumerateMajorRecords<IPerkGetter>()) {
            yield return perk.Conditions;

            foreach (var effect in perk.Effects) {
                foreach (var perkCondition in effect.Conditions) {
                    yield return perkCondition.Conditions;
                }
            }
        }

        foreach (var musicTrack in mod.EnumerateMajorRecords<IMusicTrackGetter>()) {
            if (musicTrack.Conditions != null) yield return musicTrack.Conditions;
        }

        foreach (var soundDescriptor in mod.EnumerateMajorRecords<ISoundDescriptorGetter>()) {
            yield return soundDescriptor.Conditions;
        }

        foreach (var faction in mod.EnumerateMajorRecords<IFactionGetter>()) {
            if (faction.Conditions != null) yield return faction.Conditions;
        }

        foreach (var package in mod.EnumerateMajorRecords<IPackageGetter>()) {
            yield return package.Conditions;
        }

        foreach (var scene in mod.EnumerateMajorRecords<ISceneGetter>()) {
            yield return scene.Conditions;

            foreach (var phase in scene.Phases) {
                yield return phase.StartConditions;
                yield return phase.CompletionConditions;
            }
        }

        foreach (var storyManagerBranchNode in mod.EnumerateMajorRecords<IStoryManagerBranchNodeGetter>()) {
            yield return storyManagerBranchNode.Conditions;
        }

        foreach (var storyManagerEventNode in mod.EnumerateMajorRecords<IStoryManagerEventNodeGetter>()) {
            yield return storyManagerEventNode.Conditions;
        }

        foreach (var storyManagerQuestNode in mod.EnumerateMajorRecords<IStoryManagerQuestNodeGetter>()) {
            yield return storyManagerQuestNode.Conditions;
        }

        foreach (var constructibleObject in mod.EnumerateMajorRecords<IConstructibleObjectGetter>()) {
            yield return constructibleObject.Conditions;
        }

        foreach (var idleAnimation in mod.EnumerateMajorRecords<IIdleAnimationGetter>()) {
            yield return idleAnimation.Conditions;
        }

        foreach (var ingredient in mod.EnumerateMajorRecords<IIngredientGetter>()) {
            foreach (var effect in ingredient.Effects) {
                yield return effect.Conditions;
            }
        }

        foreach (var ingestible in mod.EnumerateMajorRecords<IIngestibleGetter>()) {
            foreach (var effect in ingestible.Effects) {
                yield return effect.Conditions;
            }
        }

        foreach (var objectEffect in mod.EnumerateMajorRecords<IObjectEffectGetter>()) {
            foreach (var effect in objectEffect.Effects) {
                yield return effect.Conditions;
            }
        }

        foreach (var scroll in mod.EnumerateMajorRecords<IScrollGetter>()) {
            foreach (var effect in scroll.Effects) {
                yield return effect.Conditions;
            }
        }

        foreach (var spell in mod.EnumerateMajorRecords<ISpellGetter>()) {
            foreach (var effect in spell.Effects) {
                yield return effect.Conditions;
            }
        }

        foreach (var magicEffect in mod.EnumerateMajorRecords<IMagicEffectGetter>()) {
            yield return magicEffect.Conditions;
        }
    }
}
