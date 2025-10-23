using CreationEditor;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Mod;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Records.Assets.VoiceType;
using Noggog;
using Serilog;
namespace DialogueExporter.Services.VoiceSheets;

public class ExportVoiceSheets(
    ILogger logger,
    IModInfoProvider modInfoProvider,
    IReferenceService referenceService,
    IDataSourceService dataSourceService,
    IEditorEnvironment editorEnvironment) {
    public IEnumerable<ExportLine> GetLines(IModGetter currentMod, bool includeAlreadyVoiced) {
        logger.Here().Verbose("Start finding voice lines for mod {Mod}", currentMod.ModKey);

        var linkCache = editorEnvironment.LinkCache;

        var assetLinkCache = linkCache.CreateImmutableAssetLinkCache();
        var voiceTypeAssetLookup = assetLinkCache.GetComponent<VoiceTypeAssetLookup>();

        var voiceTypes = currentMod.EnumerateMajorRecords<IVoiceTypeGetter>().Select(v => v.EditorID).WhereNotNull().ToHashSet();

        var masterInfos = modInfoProvider.GetMasterInfos(editorEnvironment.LinkCache);
        if (!masterInfos.TryGetValue(currentMod.ModKey, out var currentModMasterInfo) || !currentModMasterInfo.Valid) {
            logger.Here().Error("Could not find master info for mod {Mod}", currentMod.ModKey);
            yield break;
        }

        foreach (var modKey in currentModMasterInfo.Masters.Append(currentMod.ModKey)) {
            var mod = editorEnvironment.LinkCache.ResolveMod(modKey);
            if (mod is null) {
                logger.Here().Error("Could not find mod {Mod} in link cache", modKey);
                continue;
            }

            foreach (var questGroup in mod.EnumerateMajorRecords<IDialogTopicGetter>().GroupBy(t => t.Quest.FormKey)) {
                if (!linkCache.TryResolve<IQuestGetter>(questGroup.Key, out var quest)) {
                    logger.Here().Error("Quest {Quest} not found in mod, skipping topics {Topics}", questGroup.Key, string.Join(", ", questGroup.Select(t => t.FormKey)));
                    continue;
                }

                foreach (var topic in questGroup) {
                    foreach (var responses in topic.Responses) {
                        var paths = voiceTypeAssetLookup.GetVoiceTypePaths(responses).ToArray();
                        if (paths.Length == 0) continue;

                        foreach (var path in paths) {
                            var lastSeparator = path.LastIndexOf(Path.DirectorySeparatorChar);
                            var voiceTypeFolder = path[..lastSeparator];
                            lastSeparator = voiceTypeFolder.LastIndexOf(Path.DirectorySeparatorChar);
                            var voiceType = voiceTypeFolder[(lastSeparator + 1)..];
                            if (!voiceTypes.Contains(voiceType)) continue;
                            if (!includeAlreadyVoiced && File.Exists(Path.Combine(dataSourceService.ActiveDataSource.Path, path))) continue;

                            var fileName = path[(lastSeparator + 1)..^4];

                            // WICastMagi_WICastMagicNonH_000073C7_1
                            var lastUnderscore = fileName.LastIndexOf('_');
                            var responseNumber = int.Parse(fileName[(lastUnderscore + 1)..]);
                            var response = responses.Responses.FirstOrDefault(r => r.ResponseNumber == responseNumber);
                            if (response is null) {
                                logger.Here().Error("Response not found for topic {Topic} response number {ResponseNumber}, skipping", topic.FormKey, responseNumber);
                                continue;
                            }

                            ExportLine? line = null;
                            try {
                                var (npc, speaker) = GetSpeakerWithSeparateCombatLines(quest, topic, responses, voiceType);
                                var (context, scene, sceneAction) = GetContext(quest, topic, responses);
                                var rootBranch = GetRootBranch(topic);
                                line = new ExportLine(
                                    voiceType,
                                    response,
                                    responses,
                                    topic,
                                    rootBranch,
                                    quest,
                                    npc,
                                    scene,
                                    sceneAction,
                                    speaker.Replace('\"', '\''),
                                    context.Replace('\"', '\''),
                                    response.Text.String?.Replace('\"', '\'') ?? "",
                                    response.ScriptNotes.Replace('\"', '\''),
                                    response.Emotion + " " + response.EmotionValue,
                                    path
                                );
                            } catch (Exception e) {
                                logger.Here().Error(e, "Error processing topic {Topic} response {ResponseNumber}", topic.FormKey, response.ResponseNumber);
                            }

                            if (line is not null) yield return line;
                        }
                    }
                }
            }
        }

        logger.Here().Verbose("Finished finding voice lines for mod {Mod}", currentMod.ModKey);

        (string Context, ISceneGetter? Scene, ISceneActionGetter? SceneAction) GetContext(
            IQuestGetter quest,
            IDialogTopicGetter topic,
            IDialogResponsesGetter responses) {
            var subtype = topic.SubtypeName.ToDialogTopicSubtype();
            var type = subtype switch {
                DialogTopic.SubtypeEnum.ActorCollideWithActor => "Player bumps into you",
                DialogTopic.SubtypeEnum.AcceptYield => "You accept the player's yield in a fight",
                DialogTopic.SubtypeEnum.Agree => "You are a follower and agree to do something for the player",
                DialogTopic.SubtypeEnum.AlertIdle => "You are searching for any enemy",
                DialogTopic.SubtypeEnum.AlertToCombat => "You find an enemy after searching for them",
                DialogTopic.SubtypeEnum.AlertToNormal => "You fail to find an enemy after searching for them",
                DialogTopic.SubtypeEnum.AssaultNC => "You see an assault and don't care about it",
                DialogTopic.SubtypeEnum.Assault => "You see an assault or are assaulted yourself",
                DialogTopic.SubtypeEnum.Attack => "You attack someone",
                DialogTopic.SubtypeEnum.Bash => "You perform a shield bash",
                DialogTopic.SubtypeEnum.Bleedout => "You are bleeding out",
                DialogTopic.SubtypeEnum.Block => "You block an attack",
                DialogTopic.SubtypeEnum.CombatToLost => "You lose sight of the enemy in combat",
                DialogTopic.SubtypeEnum.CombatToNormal => "You have won a fight",
                DialogTopic.SubtypeEnum.Death => "You are dying",
                DialogTopic.SubtypeEnum.DetectFriendDie => "You notice that a friendly npcs died from an unknown cause",
                DialogTopic.SubtypeEnum.ExitFavorState => "You are a follower and the player discords a request for you",
                DialogTopic.SubtypeEnum.ShootBow => "You notice someone randomly shooting a bow out of combat",
                DialogTopic.SubtypeEnum.Flee => "You flee from combat",
                DialogTopic.SubtypeEnum.Goodbye => "You say goodbye to the player after a conversation",
                DialogTopic.SubtypeEnum.CombatGrunt => "You say this during combat",
                DialogTopic.SubtypeEnum.Hello => "You greet the player",
                DialogTopic.SubtypeEnum.Idle => "You are randomly saying this",
                DialogTopic.SubtypeEnum.Hit => "You were hit",
                DialogTopic.SubtypeEnum.KnockOverObject => "You notice the player knocking over an object",
                DialogTopic.SubtypeEnum.LostIdle => "You were searching for an enemy but didn't find them",
                DialogTopic.SubtypeEnum.LockedObject => "You notice the player looking at a locked object",
                DialogTopic.SubtypeEnum.LostToCombat => "You previously lost sight of an enemy and now combat starts again",
                DialogTopic.SubtypeEnum.LostToNormal => "You lost sight of an enemy and now stop the search",
                DialogTopic.SubtypeEnum.MoralRefusal => "You are a follower and refuse to do something because it's against your morals",
                DialogTopic.SubtypeEnum.MurderNC => "You see a murder and don't care about it",
                DialogTopic.SubtypeEnum.Murder => "You see a murder",
                DialogTopic.SubtypeEnum.NormalToAlert => "You noticed that there is an enemy somewhere and start to search",
                DialogTopic.SubtypeEnum.NormalToCombat => "A fight starts",
                DialogTopic.SubtypeEnum.NoticeCorpse => "You notice a corpse",
                DialogTopic.SubtypeEnum.ObserveCombat => "You observe a fight",
                DialogTopic.SubtypeEnum.PlayerShout => "You notice the player using a shout out of combat",
                DialogTopic.SubtypeEnum.PickpocketCombat => "You caught a pickpocket",
                DialogTopic.SubtypeEnum.PickpocketNC => "You caught a pickpocket but don't care about it",
                DialogTopic.SubtypeEnum.PickpocketTopic => "You notice the player looking like they're trying to pickpocket",
                DialogTopic.SubtypeEnum.PlayerInIronSights => "Player aims drawn bow at you",
                DialogTopic.SubtypeEnum.PowerAttack => "You perform a power attack",
                DialogTopic.SubtypeEnum.PursueIdleTopic => "You are pursuing a criminal",
                DialogTopic.SubtypeEnum.Refuse => "You are a follower and refuse to do something because you can't do it",
                DialogTopic.SubtypeEnum.Rumors => "You say a rumor",
                DialogTopic.SubtypeEnum.ServiceRefusal => "You refuse to do something for the player",
                DialogTopic.SubtypeEnum.Show => "You are a follower and the player starts to give you a command",
                DialogTopic.SubtypeEnum.Steal => "You notice someone stealing something",
                DialogTopic.SubtypeEnum.StealFromNC => "You notice someone stealing something but don't care about it",
                DialogTopic.SubtypeEnum.SwingMeleeWeapon => "You notice the player swinging a weapon out of combat",
                DialogTopic.SubtypeEnum.Taunt => "You taunt an enemy during combat",
                DialogTopic.SubtypeEnum.TimeToGo => "You say this while the player is trespassing",
                DialogTopic.SubtypeEnum.TrespassAgainstNC => "You notice the someone trespassing but don't care about it",
                DialogTopic.SubtypeEnum.Trespass => "You notice the someone trespassing",
                DialogTopic.SubtypeEnum.WerewolfTransformCrime => "You notice the player transforming into a werewolf",
                DialogTopic.SubtypeEnum.ZKeyObject => "You notice the player grabbing and moving around an item",
                _ => null,
            };

            if (type is not null) return (type, null, null);

            if (GetPromptFromInvisibleContinue(topic, responses) is {} promptFromInvisibleContinue) {
                return (promptFromInvisibleContinue, null, null);
            }

            if (GetPromptFromBranchingDialog(topic, responses) is {} promptFromBranchingDialog) {
                return (promptFromBranchingDialog, null, null);
            }

            switch (subtype) {
                case DialogTopic.SubtypeEnum.Custom or DialogTopic.SubtypeEnum.ForceGreet:
                    return ("You say something to the player", null, null);
                case DialogTopic.SubtypeEnum.SharedInfo:
                    var results = referenceService.GetRecordReferences(responses)
                        .Select(reference => linkCache.TryResolveSimpleContext<IDialogResponsesGetter>(reference.FormKey, out var context) ? context : null)
                        .WhereNotNull()
                        .Select(r => {
                            if (r.Parent?.Record is not IDialogTopicGetter t) return (string.Empty, null, null);
                            if (topic.Quest.TryResolve(linkCache) is not {} q) return (string.Empty, null, null);

                            return GetContext(q, t, r.Record);
                        })
                        .Where(c => !c.Context.IsNullOrEmpty())
                        .ToArray();

                    switch (results) {
                        case []:
                            break;
                        case [var result]:
                            return result;
                        case var _:
                            return ("Used in multiple contexts: \n" + string.Join("\n", results.Select(x => x.Context)), null, null);
                    }

                    break;
                case DialogTopic.SubtypeEnum.Scene: {
                    foreach (var scene in GetQuestScenes(topic.Quest)) {
                        foreach (var action in scene.Actions) {
                            if (action.Type == SceneAction.TypeEnum.Dialog && action.Topic.FormKey == topic.FormKey) {
                                var aliases = scene.Actors
                                    .Where(a => a.ID != action.ActorID)
                                    .Select(a => GetAliasName(quest, (int) a.ID, responses))
                                    .ToArray();
                                if (aliases.Length == 0) {
                                    return ("You are speaking in a scene with no other actors", scene, action);
                                }

                                var lastDialog = scene.Actions
                                    .Where(a => a.Type == SceneAction.TypeEnum.Dialog)
                                    .Where(a => a.EndPhase < action.StartPhase)
                                    .OrderByDescending(a => a.StartPhase)
                                    .FirstOrDefault();

                                var speakers = string.Join(", ", aliases);
                                if (lastDialog?.ActorID == null) {
                                    return ($"You are speaking to {speakers}", scene, action);
                                }

                                var lastTopic = lastDialog.Topic.TryResolve(linkCache);
                                if (lastTopic is null) {
                                    return ($"You are speaking to {speakers}", scene, action);
                                }

                                var lastActor = lastDialog.ActorID == action.ActorID
                                    ? "You"
                                    : GetAliasName(quest, lastDialog.ActorID.Value, lastTopic.Responses[0]);
                                string textString;
                                if (!lastTopic.Responses[0].ResponseData.IsNull
                                 && lastTopic.Responses[0].ResponseData.TryResolve(linkCache, out var sharedInfo)) {
                                    textString = sharedInfo.Responses[^1].Text.String ?? string.Empty;
                                } else {
                                    textString = lastTopic.Responses[0].Responses[^1].Text.String ?? string.Empty;
                                }

                                return ($"You are speaking to {speakers}. {lastActor} last said '{textString}'", scene, action);
                            }
                        }
                    }

                    logger.Here().Error("Topic {Topic} is not part of a scene", topic.FormKey);
                    return ("", null, null);
                }
            }

            logger.Here().Error("Could not determine context for topic {Topic} with subtype {Subtype}", topic.FormKey, topic.SubtypeName.ToDialogTopicSubtype());
            return ("", null, null);

            string? GetPromptFromBranchingDialog(IDialogTopicGetter t, IDialogResponsesGetter r) {
                if (r.Prompt is not null && !r.Prompt.String.IsNullOrEmpty()) {
                    return $"Player said '{r.Prompt.String}'";
                }

                if (t.Name is not null && !t.Name.String.IsNullOrEmpty()) {
                    return $"Player said '{t.Name.String}'";
                }

                return null;
            }

            string? GetPromptFromInvisibleContinue(IDialogTopicGetter t, IDialogResponsesGetter r) {
                foreach (var linkResponses in referenceService.GetRecordReferences(t)
                        .Select(reference => linkCache.TryResolve<IDialogResponsesGetter>(reference.FormKey, out var response) ? response : null)
                        .WhereNotNull()
                        .Where(response => response.Flags is {} flags && flags.Flags.HasFlag(DialogResponses.Flag.InvisibleContinue))
                    ) {
                    var responsesContext = linkCache.ResolveSimpleContext<IDialogResponsesGetter>(r.FormKey);
                    if (responsesContext.Parent is not { Record: IDialogTopicGetter linkTopic }) continue;
                    if (linkTopic.FormKey == t.FormKey) continue;

                    if (GetPromptFromInvisibleContinue(linkTopic, linkResponses) is {} p) {
                        return p;
                    }

                    if (GetPromptFromBranchingDialog(linkTopic, linkResponses) is {} prompt) {
                        return prompt;
                    }
                }

                var rootBranch = GetRootBranch(t);
                if (rootBranch?.Flags is not null
                 && !rootBranch.Flags.Value.HasFlag(DialogBranch.Flag.TopLevel)) {
                    return "You initiate a conversation with the player";
                }

                return null;
            }
        }


        (INpcGetter? npc, string speaker) GetSpeakerWithSeparateCombatLines(
            IQuestGetter quest,
            IDialogTopicGetter topic,
            IDialogResponsesGetter responses,
            string voiceTypeName) {
            var (npc, speaker) = GetSpeaker(quest, topic, responses, voiceTypeName);

            var subtype = topic.SubtypeName.ToDialogTopicSubtype();
            if (subtype.HasValue && subtype.Value.IsCombatLine()) {
                speaker += " (Combat)";
            }

            return (npc, speaker);
        }

        (INpcGetter? npc, string speaker) GetSpeaker(
            IQuestGetter quest,
            IDialogTopicGetter topic,
            IDialogResponsesGetter responses,
            string voiceTypeName) {
            var conditions = responses.Conditions.Concat(quest.DialogConditions).ToArray();

            var condition = conditions
                .OfType<IConditionFloatGetter>()
                .FirstOrDefault(x => x.Data is IGetIsIDConditionDataGetter getIsId
                 && getIsId.Object.UsesLink()
                 && getIsId.Object.Link.TryResolve<IHasVoiceTypeGetter>(linkCache, out var hasVoiceType)
                 && hasVoiceType.Voice.TryResolve(linkCache, out var voiceType)
                 && voiceType.EditorID == voiceTypeName);

            if (condition?.Data is IGetIsIDConditionDataGetter { RunOnType: Condition.RunOnType.Subject } getIsID
             && Math.Abs(condition.ComparisonValue - 1) < 0.001) {
                if (linkCache.TryResolve<INpcGetter>(getIsID.Object.Link.FormKey, out var npc)) {
                    return (npc, GetNameOrEditorID(npc));
                }

                if (linkCache.TryResolve<ITalkingActivatorGetter>(getIsID.Object.Link.FormKey, out var talkingActivator)) {
                    return (null, GetNameOrEditorID(talkingActivator));
                }
            }

            condition = conditions.OfType<IConditionFloatGetter>().FirstOrDefault(x => x.Data is IGetIsAliasRefConditionDataGetter);
            if (condition?.Data is IGetIsAliasRefConditionDataGetter { RunOnType: Condition.RunOnType.Subject } getIsAliasReference
             && Math.Abs(condition.ComparisonValue - 1) < 0.001) {
                var aliasNpc = GetAliasNpc(quest, getIsAliasReference.ReferenceAliasIndex, responses);
                return aliasNpc is null
                    ? (null, GetAliasName(quest, getIsAliasReference.ReferenceAliasIndex, responses))
                    : (aliasNpc, GetNameOrEditorID(aliasNpc));
            }

            if (topic.SubtypeName.ToDialogTopicSubtype() == DialogTopic.SubtypeEnum.Scene) {
                foreach (var scene in GetQuestScenes(topic.Quest)) {
                    foreach (var action in scene.Actions) {
                        if (action.Type == SceneAction.TypeEnum.Dialog
                         && action.Topic.FormKey == topic.FormKey
                         && action.ActorID.HasValue) {
                            var aliasNpc = GetAliasNpc(quest, action.ActorID.Value, responses);
                            return aliasNpc is null
                                ? (null, GetAliasName(quest, action.ActorID.Value, responses))
                                : (aliasNpc, GetNameOrEditorID(aliasNpc));
                        }
                    }
                }
            }

            condition = conditions.OfType<IConditionFloatGetter>().FirstOrDefault(x => x.Data is IGetIsVoiceTypeConditionDataGetter);
            if (condition?.Data.RunOnType == Condition.RunOnType.Subject && Math.Abs(condition.ComparisonValue - 1) < 0.001) {
                return (null, "Any NPC using voice type " + voiceTypeName);
            }

            condition = conditions.OfType<IConditionFloatGetter>().FirstOrDefault(x => x.Data is GetInFactionConditionData);
            if (condition?.Data is GetInFactionConditionData { RunOnType: Condition.RunOnType.Subject } getInFaction
             && linkCache.TryResolve<IFactionGetter>(getInFaction.Faction.Link.FormKey, out var faction)
             && getInFaction.RunOnType == Condition.RunOnType.Subject
             && Math.Abs(condition.ComparisonValue - 1) < 0.001) {
                return (null, "Member of faction " + GetNameOrEditorID(faction));
            }

            logger.Here().Error("Could not determine speaker for quest {Quest} topic {Topic} responses {Responses}", quest.FormKey, topic.FormKey, responses.FormKey);
            return (null, "Anyone");
        }

        string GetAliasName(IQuestGetter quest, int id, IDialogResponsesGetter responses) {
            var alias = quest.Aliases.FirstOrDefault(a => a.ID == id);
            if (alias is null) {
                logger.Here().Error("Alias {Alias} not found in quest {Quest} for response {Response}", id, quest.FormKey, responses.FormKey);
                return "";
            }

            return alias.Name ?? throw new InvalidOperationException($"Alias {id} in {quest.EditorID} has no name");
        }

        INpcGetter? GetAliasNpc(IQuestGetter quest, int id, IDialogResponsesGetter responses) {
            var alias = quest.Aliases.FirstOrDefault(a => a.ID == id);
            if (alias is null) {
                logger.Here().Error("Alias {Alias} not found in quest {Quest} for response {Response}", id, quest.FormKey, responses.FormKey);
                return null;
            }

            // If the alias is swapped on death, don't associate it with a specific NPC and return null as it's not always the same NPC
            if (quest.VirtualMachineAdapter is not null && quest.VirtualMachineAdapter.Aliases.Any(x => x.Property.Alias == id
             && x.Scripts.Any(s => string.Equals(s.Name, "SwapAliasOnDeath", StringComparison.OrdinalIgnoreCase)))) {
                return null;
            }

            if (!alias.ForcedReference.IsNull) {
                var forcedRef = alias.ForcedReference.TryResolve<IPlacedNpcGetter>(linkCache);
                if (forcedRef is not null) {
                    return forcedRef.Base.TryResolve<INpcGetter>(linkCache);
                }
            }

            return alias.UniqueActor.TryResolve<INpcGetter>(linkCache);
        }

        IDialogBranchGetter? GetRootBranch(IDialogTopicGetter topic) {
            var processedTopics = new List<FormKey>();

            return GetRootBranchRec(topic);

            IDialogBranchGetter? GetRootBranchRec(IDialogTopicGetter currentTopic) {
                // Try to get branch from the starting topic
                var branch = referenceService.GetRecordReferences(currentTopic)
                    .Select(reference => linkCache.TryResolve<IDialogBranchGetter>(reference.FormKey, out var branch) ? branch : null)
                    .WhereNotNull()
                    .FirstOrDefault(branch => branch.StartingTopic.FormKey == currentTopic.FormKey);
                if (branch is not null) return branch;

                // If not a starting topic, try to find a starting topic linking to this
                foreach (var linkTopic in referenceService.GetRecordReferences(currentTopic)
                    .Select(reference => linkCache.TryResolve<IDialogTopicGetter>(reference.FormKey, out var t) ? t : null)
                    .WhereNotNull()) {
                    foreach (var responses in linkTopic.Responses) {
                        foreach (var linkTo in responses.LinkTo) {
                            if (processedTopics.Contains(linkTopic.FormKey)) continue;

                            processedTopics.Add(linkTopic.FormKey);

                            if (linkTo.FormKey == currentTopic.FormKey) {
                                var branchFromLinkTopic = GetRootBranchRec(linkTopic);
                                if (branchFromLinkTopic is not null) return branchFromLinkTopic;
                            }

                            if (GetRootBranchRec(linkTopic) is {} rootBranch) {
                                return rootBranch;
                            }
                        }
                    }
                }

                return null;
            }
        }

        IEnumerable<ISceneGetter> GetQuestScenes(IFormLinkNullableGetter<IQuestGetter> quest) {
            return referenceService.GetRecordReferences(quest)
                .Select(reference => linkCache.TryResolve<ISceneGetter>(reference.FormKey, out var scene) ? scene : null)
                .WhereNotNull()
                .Where(scene => scene.Quest.FormKey == quest.FormKey);
        }
    }

    private static string GetNameOrEditorID<T>(T named)
        where T : IMajorRecordGetter, INamedGetter =>
        named.Name ?? named.EditorID ?? throw new InvalidOperationException(
            $"Record {named.FormKey} has no name or editor ID");
}
