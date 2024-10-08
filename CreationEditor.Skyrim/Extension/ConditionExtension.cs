﻿using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim;

public static class ConditionExtension {
    public static ConditionData ToCondition(this Condition.Function function) {
        return function switch {
            Condition.Function.GetWantBlocking => new GetWantBlockingConditionData(),
            Condition.Function.GetDistance => new GetDistanceConditionData(),
            Condition.Function.GetLocked => new GetLockedConditionData(),
            Condition.Function.GetPos => new GetPosConditionData(),
            Condition.Function.GetAngle => new GetAngleConditionData(),
            Condition.Function.GetStartingPos => new GetStartingPosConditionData(),
            Condition.Function.GetStartingAngle => new GetStartingAngleConditionData(),
            Condition.Function.GetSecondsPassed => new GetSecondsPassedConditionData(),
            Condition.Function.GetActorValue => new GetActorValueConditionData(),
            Condition.Function.GetCurrentTime => new GetCurrentTimeConditionData(),
            Condition.Function.GetScale => new GetScaleConditionData(),
            Condition.Function.IsMoving => new IsMovingConditionData(),
            Condition.Function.IsTurning => new IsTurningConditionData(),
            Condition.Function.GetLineOfSight => new GetLineOfSightConditionData(),
            Condition.Function.GetInSameCell => new GetInSameCellConditionData(),
            Condition.Function.GetDisabled => new GetDisabledConditionData(),
            Condition.Function.GetDisease => new GetDiseaseConditionData(),
            Condition.Function.GetClothingValue => new GetClothingValueConditionData(),
            Condition.Function.SameFaction => new SameFactionConditionData(),
            Condition.Function.SameRace => new SameRaceConditionData(),
            Condition.Function.SameSex => new SameSexConditionData(),
            Condition.Function.GetDetected => new GetDetectedConditionData(),
            Condition.Function.GetDead => new GetDeadConditionData(),
            Condition.Function.GetItemCount => new GetItemCountConditionData(),
            Condition.Function.GetGold => new GetGoldConditionData(),
            Condition.Function.GetSleeping => new GetSleepingConditionData(),
            Condition.Function.GetTalkedToPC => new GetTalkedToPCConditionData(),
            Condition.Function.GetQuestRunning => new GetQuestRunningConditionData(),
            Condition.Function.GetStage => new GetStageConditionData(),
            Condition.Function.GetStageDone => new GetStageDoneConditionData(),
            Condition.Function.GetFactionRankDifference => new GetFactionRankDifferenceConditionData(),
            Condition.Function.GetAlarmed => new GetAlarmedConditionData(),
            Condition.Function.IsRaining => new IsRainingConditionData(),
            Condition.Function.GetAttacked => new GetAttackedConditionData(),
            Condition.Function.GetLockLevel => new GetLockLevelConditionData(),
            Condition.Function.GetShouldAttack => new GetShouldAttackConditionData(),
            Condition.Function.GetInCell => new GetInCellConditionData(),
            Condition.Function.GetIsClass => new GetIsClassConditionData(),
            Condition.Function.GetIsRace => new GetIsRaceConditionData(),
            Condition.Function.GetIsSex => new GetIsSexConditionData(),
            Condition.Function.GetInFaction => new GetInFactionConditionData(),
            Condition.Function.GetIsID => new GetIsIDConditionData(),
            Condition.Function.GetFactionRank => new GetFactionRankConditionData(),
            Condition.Function.GetGlobalValue => new GetGlobalValueConditionData(),
            Condition.Function.IsSnowing => new IsSnowingConditionData(),
            Condition.Function.GetRandomPercent => new GetRandomPercentConditionData(),
            Condition.Function.GetLevel => new GetLevelConditionData(),
            Condition.Function.IsRotating => new IsRotatingConditionData(),
            Condition.Function.GetDeadCount => new GetDeadCountConditionData(),
            Condition.Function.GetIsAlerted => new GetIsAlertedConditionData(),
            Condition.Function.GetPlayerControlsDisabled => new GetPlayerControlsDisabledConditionData(),
            Condition.Function.GetHeadingAngle => new GetHeadingAngleConditionData(),
            Condition.Function.IsWeaponMagicOut => new IsWeaponMagicOutConditionData(),
            Condition.Function.IsTorchOut => new IsTorchOutConditionData(),
            Condition.Function.IsShieldOut => new IsShieldOutConditionData(),
            Condition.Function.IsFacingUp => new IsFacingUpConditionData(),
            Condition.Function.GetKnockedState => new GetKnockedStateConditionData(),
            Condition.Function.GetWeaponAnimType => new GetWeaponAnimTypeConditionData(),
            Condition.Function.IsWeaponSkillType => new IsWeaponSkillTypeConditionData(),
            Condition.Function.GetCurrentAIPackage => new GetCurrentAIPackageConditionData(),
            Condition.Function.IsWaiting => new IsWaitingConditionData(),
            Condition.Function.IsIntimidatedbyPlayer => new IsIntimidatedbyPlayerConditionData(),
            Condition.Function.IsPlayerInRegion => new IsPlayerInRegionConditionData(),
            Condition.Function.GetActorAggroRadiusViolated => new GetActorAggroRadiusViolatedConditionData(),
            Condition.Function.GetCrime => new GetCrimeConditionData(),
            Condition.Function.IsGreetingPlayer => new IsGreetingPlayerConditionData(),
            Condition.Function.IsGuard => new IsGuardConditionData(),
            Condition.Function.HasBeenEaten => new HasBeenEatenConditionData(),
            Condition.Function.GetStaminaPercentage => new GetStaminaPercentageConditionData(),
            Condition.Function.GetPCIsClass => new GetPCIsClassConditionData(),
            Condition.Function.GetPCIsRace => new GetPCIsRaceConditionData(),
            Condition.Function.GetPCIsSex => new GetPCIsSexConditionData(),
            Condition.Function.GetPCInFaction => new GetPCInFactionConditionData(),
            Condition.Function.SameFactionAsPC => new SameFactionAsPCConditionData(),
            Condition.Function.SameRaceAsPC => new SameRaceAsPCConditionData(),
            Condition.Function.SameSexAsPC => new SameSexAsPCConditionData(),
            Condition.Function.GetIsReference => new GetIsReferenceConditionData(),
            Condition.Function.IsTalking => new IsTalkingConditionData(),
            Condition.Function.GetWalkSpeed => new GetWalkSpeedConditionData(),
            Condition.Function.GetCurrentAIProcedure => new GetCurrentAIProcedureConditionData(),
            Condition.Function.GetTrespassWarningLevel => new GetTrespassWarningLevelConditionData(),
            Condition.Function.IsTrespassing => new IsTrespassingConditionData(),
            Condition.Function.IsInMyOwnedCell => new IsInMyOwnedCellConditionData(),
            Condition.Function.GetWindSpeed => new GetWindSpeedConditionData(),
            Condition.Function.GetCurrentWeatherPercent => new GetCurrentWeatherPercentConditionData(),
            Condition.Function.GetIsCurrentWeather => new GetIsCurrentWeatherConditionData(),
            Condition.Function.IsContinuingPackagePCNear => new IsContinuingPackagePCNearConditionData(),
            Condition.Function.GetIsCrimeFaction => new GetIsCrimeFactionConditionData(),
            Condition.Function.CanHaveFlames => new CanHaveFlamesConditionData(),
            Condition.Function.HasFlames => new HasFlamesConditionData(),
            Condition.Function.GetOpenState => new GetOpenStateConditionData(),
            Condition.Function.GetSitting => new GetSittingConditionData(),
            Condition.Function.GetIsCurrentPackage => new GetIsCurrentPackageConditionData(),
            Condition.Function.IsCurrentFurnitureRef => new IsCurrentFurnitureRefConditionData(),
            Condition.Function.IsCurrentFurnitureObj => new IsCurrentFurnitureObjConditionData(),
            Condition.Function.GetDayOfWeek => new GetDayOfWeekConditionData(),
            Condition.Function.GetTalkedToPCParam => new GetTalkedToPCParamConditionData(),
            Condition.Function.IsPCSleeping => new IsPCSleepingConditionData(),
            Condition.Function.IsPCAMurderer => new IsPCAMurdererConditionData(),
            Condition.Function.HasSameEditorLocAsRef => new HasSameEditorLocAsRefConditionData(),
            Condition.Function.HasSameEditorLocAsRefAlias => new HasSameEditorLocAsRefAliasConditionData(),
            Condition.Function.GetEquipped => new GetEquippedConditionData(),
            Condition.Function.IsSwimming => new IsSwimmingConditionData(),
            Condition.Function.GetAmountSoldStolen => new GetAmountSoldStolenConditionData(),
            Condition.Function.GetIgnoreCrime => new GetIgnoreCrimeConditionData(),
            Condition.Function.GetPCExpelled => new GetPCExpelledConditionData(),
            Condition.Function.GetPCFactionMurder => new GetPCFactionMurderConditionData(),
            Condition.Function.GetPCEnemyofFaction => new GetPCEnemyofFactionConditionData(),
            Condition.Function.GetPCFactionAttack => new GetPCFactionAttackConditionData(),
            Condition.Function.GetDestroyed => new GetDestroyedConditionData(),
            Condition.Function.HasMagicEffect => new HasMagicEffectConditionData(),
            Condition.Function.GetDefaultOpen => new GetDefaultOpenConditionData(),
            Condition.Function.GetAnimAction => new GetAnimActionConditionData(),
            Condition.Function.IsSpellTarget => new IsSpellTargetConditionData(),
            Condition.Function.GetVATSMode => new GetVATSModeConditionData(),
            Condition.Function.GetVampireFeed => new GetVampireFeedConditionData(),
            Condition.Function.GetCannibal => new GetCannibalConditionData(),
            Condition.Function.GetIsClassDefault => new GetIsClassDefaultConditionData(),
            Condition.Function.GetInCellParam => new GetInCellParamConditionData(),
            Condition.Function.GetVatsTargetHeight => new GetVatsTargetHeightConditionData(),
            Condition.Function.GetIsGhost => new GetIsGhostConditionData(),
            Condition.Function.GetUnconscious => new GetUnconsciousConditionData(),
            Condition.Function.GetRestrained => new GetRestrainedConditionData(),
            Condition.Function.GetIsUsedItem => new GetIsUsedItemConditionData(),
            Condition.Function.GetIsUsedItemType => new GetIsUsedItemTypeConditionData(),
            Condition.Function.IsScenePlaying => new IsScenePlayingConditionData(),
            Condition.Function.IsInDialogueWithPlayer => new IsInDialogueWithPlayerConditionData(),
            Condition.Function.GetLocationCleared => new GetLocationClearedConditionData(),
            Condition.Function.GetIsPlayableRace => new GetIsPlayableRaceConditionData(),
            Condition.Function.GetOffersServicesNow => new GetOffersServicesNowConditionData(),
            Condition.Function.HasAssociationType => new HasAssociationTypeConditionData(),
            Condition.Function.HasFamilyRelationship => new HasFamilyRelationshipConditionData(),
            Condition.Function.HasParentRelationship => new HasParentRelationshipConditionData(),
            Condition.Function.IsWarningAbout => new IsWarningAboutConditionData(),
            Condition.Function.IsWeaponOut => new IsWeaponOutConditionData(),
            Condition.Function.HasSpell => new HasSpellConditionData(),
            Condition.Function.IsTimePassing => new IsTimePassingConditionData(),
            Condition.Function.IsPleasant => new IsPleasantConditionData(),
            Condition.Function.IsCloudy => new IsCloudyConditionData(),
            Condition.Function.IsSmallBump => new IsSmallBumpConditionData(),
            Condition.Function.GetBaseActorValue => new GetBaseActorValueConditionData(),
            Condition.Function.IsOwner => new IsOwnerConditionData(),
            Condition.Function.IsCellOwner => new IsCellOwnerConditionData(),
            Condition.Function.IsLeftUp => new IsLeftUpConditionData(),
            Condition.Function.IsSneaking => new IsSneakingConditionData(),
            Condition.Function.IsRunning => new IsRunningConditionData(),
            Condition.Function.GetFriendHit => new GetFriendHitConditionData(),
            Condition.Function.IsInCombat => new IsInCombatConditionData(),
            Condition.Function.IsInInterior => new IsInInteriorConditionData(),
            Condition.Function.IsWaterObject => new IsWaterObjectConditionData(),
            Condition.Function.GetPlayerAction => new GetPlayerActionConditionData(),
            Condition.Function.IsActorUsingATorch => new IsActorUsingATorchConditionData(),
            Condition.Function.IsXBox => new IsXBoxConditionData(),
            Condition.Function.GetInWorldspace => new GetInWorldspaceConditionData(),
            Condition.Function.GetPCMiscStat => new GetPCMiscStatConditionData(),
            Condition.Function.GetPairedAnimation => new GetPairedAnimationConditionData(),
            Condition.Function.IsActorAVictim => new IsActorAVictimConditionData(),
            Condition.Function.GetIdleDoneOnce => new GetIdleDoneOnceConditionData(),
            Condition.Function.GetNoRumors => new GetNoRumorsConditionData(),
            Condition.Function.GetCombatState => new GetCombatStateConditionData(),
            Condition.Function.GetWithinPackageLocation => new GetWithinPackageLocationConditionData(),
            Condition.Function.IsRidingMount => new IsRidingMountConditionData(),
            Condition.Function.IsFleeing => new IsFleeingConditionData(),
            Condition.Function.IsInDangerousWater => new IsInDangerousWaterConditionData(),
            Condition.Function.GetIgnoreFriendlyHits => new GetIgnoreFriendlyHitsConditionData(),
            Condition.Function.IsPlayersLastRiddenMount => new IsPlayersLastRiddenMountConditionData(),
            Condition.Function.IsActor => new IsActorConditionData(),
            Condition.Function.IsEssential => new IsEssentialConditionData(),
            Condition.Function.IsPlayerMovingIntoNewSpace => new IsPlayerMovingIntoNewSpaceConditionData(),
            Condition.Function.GetInCurrentLoc => new GetInCurrentLocConditionData(),
            Condition.Function.GetInCurrentLocAlias => new GetInCurrentLocAliasConditionData(),
            Condition.Function.GetTimeDead => new GetTimeDeadConditionData(),
            Condition.Function.HasLinkedRef => new HasLinkedRefConditionData(),
            Condition.Function.IsChild => new IsChildConditionData(),
            Condition.Function.GetStolenItemValueNoCrime => new GetStolenItemValueNoCrimeConditionData(),
            Condition.Function.GetLastPlayerAction => new GetLastPlayerActionConditionData(),
            Condition.Function.IsPlayerActionActive => new IsPlayerActionActiveConditionData(),
            Condition.Function.IsTalkingActivatorActor => new IsTalkingActivatorActorConditionData(),
            Condition.Function.IsInList => new IsInListConditionData(),
            Condition.Function.GetStolenItemValue => new GetStolenItemValueConditionData(),
            Condition.Function.GetCrimeGoldViolent => new GetCrimeGoldViolentConditionData(),
            Condition.Function.GetCrimeGoldNonviolent => new GetCrimeGoldNonviolentConditionData(),
            Condition.Function.HasShout => new HasShoutConditionData(),
            Condition.Function.GetHasNote => new GetHasNoteConditionData(),
            Condition.Function.IsPC1stPerson => new IsPC1stPersonConditionData(),
            Condition.Function.GetCauseofDeath => new GetCauseofDeathConditionData(),
            Condition.Function.IsLimbGone => new IsLimbGoneConditionData(),
            Condition.Function.IsWeaponInList => new IsWeaponInListConditionData(),
            Condition.Function.IsBribedbyPlayer => new IsBribedbyPlayerConditionData(),
            Condition.Function.GetRelationshipRank => new GetRelationshipRankConditionData(),
            Condition.Function.GetVATSValue => new GetVATSValueActionConditionData(), // default choice
            Condition.Function.IsKiller => new IsKillerConditionData(),
            Condition.Function.IsKillerObject => new IsKillerObjectConditionData(),
            Condition.Function.GetFactionCombatReaction => new GetFactionCombatReactionConditionData(),
            Condition.Function.Exists => new ExistsConditionData(),
            Condition.Function.GetGroupMemberCount => new GetGroupMemberCountConditionData(),
            Condition.Function.GetGroupTargetCount => new GetGroupTargetCountConditionData(),
            Condition.Function.GetIsVoiceType => new GetIsVoiceTypeConditionData(),
            Condition.Function.GetPlantedExplosive => new GetPlantedExplosiveConditionData(),
            Condition.Function.IsScenePackageRunning => new IsScenePackageRunningConditionData(),
            Condition.Function.GetHealthPercentage => new GetHealthPercentageConditionData(),
            Condition.Function.GetIsObjectType => new GetIsObjectTypeConditionData(),
            Condition.Function.GetDialogueEmotion => new GetDialogueEmotionConditionData(),
            Condition.Function.GetDialogueEmotionValue => new GetDialogueEmotionValueConditionData(),
            Condition.Function.GetInCurrentLocFormList => new GetInCurrentLocFormListConditionData(),
            Condition.Function.GetInZone => new GetInZoneConditionData(),
            Condition.Function.GetVelocity => new GetVelocityConditionData(),
            Condition.Function.GetGraphVariableFloat => new GetGraphVariableFloatConditionData(),
            Condition.Function.HasPerk => new HasPerkConditionData(),
            Condition.Function.GetFactionRelation => new GetFactionRelationConditionData(),
            Condition.Function.IsLastIdlePlayed => new IsLastIdlePlayedConditionData(),
            Condition.Function.GetPlayerTeammate => new GetPlayerTeammateConditionData(),
            Condition.Function.GetPlayerTeammateCount => new GetPlayerTeammateCountConditionData(),
            Condition.Function.GetActorCrimePlayerEnemy => new GetActorCrimePlayerEnemyConditionData(),
            Condition.Function.GetCrimeGold => new GetCrimeGoldConditionData(),
            Condition.Function.IsPlayerGrabbedRef => new IsPlayerGrabbedRefConditionData(),
            Condition.Function.GetKeywordItemCount => new GetKeywordItemCountConditionData(),
            Condition.Function.GetDestructionStage => new GetDestructionStageConditionData(),
            Condition.Function.IsProtected => new IsProtectedConditionData(),
            Condition.Function.GetThreatRatio => new GetThreatRatioConditionData(),
            Condition.Function.GetIsUsedItemEquipType => new GetIsUsedItemEquipTypeConditionData(),
            Condition.Function.IsCarryable => new IsCarryableConditionData(),
            Condition.Function.GetMapMarkerVisible => new GetMapMarkerVisibleConditionData(),
            Condition.Function.PlayerKnows => new PlayerKnowsConditionData(),
            Condition.Function.GetPermanentActorValue => new GetPermanentActorValueConditionData(),
            Condition.Function.CanPayCrimeGold => new CanPayCrimeGoldConditionData(),
            Condition.Function.GetDaysInJail => new GetDaysInJailConditionData(),
            Condition.Function.EPAlchemyGetMakingPoison => new EPAlchemyGetMakingPoisonConditionData(),
            Condition.Function.EPAlchemyEffectHasKeyword => new EPAlchemyEffectHasKeywordConditionData(),
            Condition.Function.GetAllowWorldInteractions => new GetAllowWorldInteractionsConditionData(),
            Condition.Function.GetLastHitCritical => new GetLastHitCriticalConditionData(),
            Condition.Function.IsCombatTarget => new IsCombatTargetConditionData(),
            Condition.Function.GetVATSRightAreaFree => new GetVATSRightAreaFreeConditionData(),
            Condition.Function.GetVATSLeftAreaFree => new GetVATSLeftAreaFreeConditionData(),
            Condition.Function.GetVATSBackAreaFree => new GetVATSBackAreaFreeConditionData(),
            Condition.Function.GetVATSFrontAreaFree => new GetVATSFrontAreaFreeConditionData(),
            Condition.Function.GetLockIsBroken => new GetLockIsBrokenConditionData(),
            Condition.Function.IsPS3 => new IsPS3ConditionData(),
            Condition.Function.IsWin32 => new IsWin32ConditionData(),
            Condition.Function.GetVATSRightTargetVisible => new GetVATSRightTargetVisibleConditionData(),
            Condition.Function.GetVATSLeftTargetVisible => new GetVATSLeftTargetVisibleConditionData(),
            Condition.Function.GetVATSBackTargetVisible => new GetVATSBackTargetVisibleConditionData(),
            Condition.Function.GetVATSFrontTargetVisible => new GetVATSFrontTargetVisibleConditionData(),
            Condition.Function.IsInCriticalStage => new IsInCriticalStageConditionData(),
            Condition.Function.GetXPForNextLevel => new GetXPForNextLevelConditionData(),
            Condition.Function.GetInfamy => new GetInfamyConditionData(),
            Condition.Function.GetInfamyViolent => new GetInfamyViolentConditionData(),
            Condition.Function.GetInfamyNonViolent => new GetInfamyNonViolentConditionData(),
            Condition.Function.GetQuestCompleted => new GetQuestCompletedConditionData(),
            Condition.Function.IsGoreDisabled => new IsGoreDisabledConditionData(),
            Condition.Function.IsSceneActionComplete => new IsSceneActionCompleteConditionData(),
            Condition.Function.GetSpellUsageNum => new GetSpellUsageNumConditionData(),
            Condition.Function.GetActorsInHigh => new GetActorsInHighConditionData(),
            Condition.Function.HasLoaded3D => new HasLoaded3DConditionData(),
            Condition.Function.HasKeyword => new HasKeywordConditionData(),
            Condition.Function.HasRefType => new HasRefTypeConditionData(),
            Condition.Function.LocationHasKeyword => new LocationHasKeywordConditionData(),
            Condition.Function.LocationHasRefType => new LocationHasRefTypeConditionData(),
            Condition.Function.GetIsEditorLocation => new GetIsEditorLocationConditionData(),
            Condition.Function.GetIsAliasRef => new GetIsAliasRefConditionData(),
            Condition.Function.GetIsEditorLocAlias => new GetIsEditorLocAliasConditionData(),
            Condition.Function.IsSprinting => new IsSprintingConditionData(),
            Condition.Function.IsBlocking => new IsBlockingConditionData(),
            Condition.Function.HasEquippedSpell => new HasEquippedSpellConditionData(),
            Condition.Function.GetCurrentCastingType => new GetCurrentCastingTypeConditionData(),
            Condition.Function.GetCurrentDeliveryType => new GetCurrentDeliveryTypeConditionData(),
            Condition.Function.GetAttackState => new GetAttackStateConditionData(),
            Condition.Function.GetEventData => new GetEventDataConditionData(),
            Condition.Function.IsCloserToAThanB => new IsCloserToAThanBConditionData(),
            Condition.Function.GetEquippedShout => new GetEquippedShoutConditionData(),
            Condition.Function.IsBleedingOut => new IsBleedingOutConditionData(),
            Condition.Function.GetRelativeAngle => new GetRelativeAngleConditionData(),
            Condition.Function.GetMovementDirection => new GetMovementDirectionConditionData(),
            Condition.Function.IsInScene => new IsInSceneConditionData(),
            Condition.Function.GetRefTypeDeadCount => new GetRefTypeDeadCountConditionData(),
            Condition.Function.GetRefTypeAliveCount => new GetRefTypeAliveCountConditionData(),
            Condition.Function.GetIsFlying => new GetIsFlyingConditionData(),
            Condition.Function.IsCurrentSpell => new IsCurrentSpellConditionData(),
            Condition.Function.SpellHasKeyword => new SpellHasKeywordConditionData(),
            Condition.Function.GetEquippedItemType => new GetEquippedItemTypeConditionData(),
            Condition.Function.GetLocationAliasCleared => new GetLocationAliasClearedConditionData(),
            Condition.Function.GetLocAliasRefTypeDeadCount => new GetLocAliasRefTypeDeadCountConditionData(),
            Condition.Function.GetLocAliasRefTypeAliveCount => new GetLocAliasRefTypeAliveCountConditionData(),
            Condition.Function.IsWardState => new IsWardStateConditionData(),
            Condition.Function.IsInSameCurrentLocAsRef => new IsInSameCurrentLocAsRefConditionData(),
            Condition.Function.IsInSameCurrentLocAsRefAlias => new IsInSameCurrentLocAsRefAliasConditionData(),
            Condition.Function.LocAliasIsLocation => new LocAliasIsLocationConditionData(),
            Condition.Function.GetKeywordDataForLocation => new GetKeywordDataForLocationConditionData(),
            Condition.Function.GetKeywordDataForAlias => new GetKeywordDataForAliasConditionData(),
            Condition.Function.LocAliasHasKeyword => new LocAliasHasKeywordConditionData(),
            Condition.Function.IsNullPackageData => new IsNullPackageDataConditionData(),
            Condition.Function.GetNumericPackageData => new GetNumericPackageDataConditionData(),
            Condition.Function.IsFurnitureAnimType => new IsFurnitureAnimTypeConditionData(),
            Condition.Function.IsFurnitureEntryType => new IsFurnitureEntryTypeConditionData(),
            Condition.Function.GetHighestRelationshipRank => new GetHighestRelationshipRankConditionData(),
            Condition.Function.GetLowestRelationshipRank => new GetLowestRelationshipRankConditionData(),
            Condition.Function.HasAssociationTypeAny => new HasAssociationTypeAnyConditionData(),
            Condition.Function.HasFamilyRelationshipAny => new HasFamilyRelationshipAnyConditionData(),
            Condition.Function.GetPathingTargetOffset => new GetPathingTargetOffsetConditionData(),
            Condition.Function.GetPathingTargetAngleOffset => new GetPathingTargetAngleOffsetConditionData(),
            Condition.Function.GetPathingTargetSpeed => new GetPathingTargetSpeedConditionData(),
            Condition.Function.GetPathingTargetSpeedAngle => new GetPathingTargetSpeedAngleConditionData(),
            Condition.Function.GetMovementSpeed => new GetMovementSpeedConditionData(),
            Condition.Function.GetInContainer => new GetInContainerConditionData(),
            Condition.Function.IsLocationLoaded => new IsLocationLoadedConditionData(),
            Condition.Function.IsLocAliasLoaded => new IsLocAliasLoadedConditionData(),
            Condition.Function.IsDualCasting => new IsDualCastingConditionData(),
            Condition.Function.GetVMQuestVariable => new GetVMQuestVariableConditionData(),
            Condition.Function.GetVMScriptVariable => new GetVMScriptVariableConditionData(),
            Condition.Function.IsEnteringInteractionQuick => new IsEnteringInteractionQuickConditionData(),
            Condition.Function.IsCasting => new IsCastingConditionData(),
            Condition.Function.GetFlyingState => new GetFlyingStateConditionData(),
            Condition.Function.IsInFavorState => new IsInFavorStateConditionData(),
            Condition.Function.HasTwoHandedWeaponEquipped => new HasTwoHandedWeaponEquippedConditionData(),
            Condition.Function.IsExitingInstant => new IsExitingInstantConditionData(),
            Condition.Function.IsInFriendStateWithPlayer => new IsInFriendStateWithPlayerConditionData(),
            Condition.Function.GetWithinDistance => new GetWithinDistanceConditionData(),
            Condition.Function.GetActorValuePercent => new GetActorValuePercentConditionData(),
            Condition.Function.IsUnique => new IsUniqueConditionData(),
            Condition.Function.GetLastBumpDirection => new GetLastBumpDirectionConditionData(),
            Condition.Function.IsInFurnitureState => new IsInFurnitureStateConditionData(),
            Condition.Function.GetIsInjured => new GetIsInjuredConditionData(),
            Condition.Function.GetIsCrashLandRequest => new GetIsCrashLandRequestConditionData(),
            Condition.Function.GetIsHastyLandRequest => new GetIsHastyLandRequestConditionData(),
            Condition.Function.IsLinkedTo => new IsLinkedToConditionData(),
            Condition.Function.GetKeywordDataForCurrentLocation => new GetKeywordDataForCurrentLocationConditionData(),
            Condition.Function.GetInSharedCrimeFaction => new GetInSharedCrimeFactionConditionData(),
            Condition.Function.GetBribeSuccess => new GetBribeSuccessConditionData(),
            Condition.Function.GetIntimidateSuccess => new GetIntimidateSuccessConditionData(),
            Condition.Function.GetArrestedState => new GetArrestedStateConditionData(),
            Condition.Function.GetArrestingActor => new GetArrestingActorConditionData(),
            Condition.Function.EPTemperingItemIsEnchanted => new EPTemperingItemIsEnchantedConditionData(),
            Condition.Function.EPTemperingItemHasKeyword => new EPTemperingItemHasKeywordConditionData(),
            Condition.Function.GetReplacedItemType => new GetReplacedItemTypeConditionData(),
            Condition.Function.IsAttacking => new IsAttackingConditionData(),
            Condition.Function.IsPowerAttacking => new IsPowerAttackingConditionData(),
            Condition.Function.IsLastHostileActor => new IsLastHostileActorConditionData(),
            Condition.Function.GetGraphVariableInt => new GetGraphVariableIntConditionData(),
            Condition.Function.GetCurrentShoutVariation => new GetCurrentShoutVariationConditionData(),
            Condition.Function.ShouldAttackKill => new ShouldAttackKillConditionData(),
            Condition.Function.GetActivatorHeight => new GetActivatorHeightConditionData(),
            Condition.Function.EPMagic_IsAdvanceSkill => new EPMagic_IsAdvanceSkillConditionData(),
            Condition.Function.WornHasKeyword => new WornHasKeywordConditionData(),
            Condition.Function.GetPathingCurrentSpeed => new GetPathingCurrentSpeedConditionData(),
            Condition.Function.GetPathingCurrentSpeedAngle => new GetPathingCurrentSpeedAngleConditionData(),
            Condition.Function.EPModSkillUsage_AdvanceObjectHasKeyword => new EPModSkillUsage_AdvanceObjectHasKeywordConditionData(),
            Condition.Function.EPModSkillUsage_IsAdvanceAction => new EPModSkillUsage_IsAdvanceActionConditionData(),
            Condition.Function.EPMagic_SpellHasKeyword => new EPMagic_SpellHasKeywordConditionData(),
            Condition.Function.GetNoBleedoutRecovery => new GetNoBleedoutRecoveryConditionData(),
            Condition.Function.EPMagic_SpellHasSkill => new EPMagic_SpellHasSkillConditionData(),
            Condition.Function.IsAttackType => new IsAttackTypeConditionData(),
            Condition.Function.IsAllowedToFly => new IsAllowedToFlyConditionData(),
            Condition.Function.HasMagicEffectKeyword => new HasMagicEffectKeywordConditionData(),
            Condition.Function.IsCommandedActor => new IsCommandedActorConditionData(),
            Condition.Function.IsStaggered => new IsStaggeredConditionData(),
            Condition.Function.IsRecoiling => new IsRecoilingConditionData(),
            Condition.Function.IsExitingInteractionQuick => new IsExitingInteractionQuickConditionData(),
            Condition.Function.IsPathing => new IsPathingConditionData(),
            Condition.Function.GetShouldHelp => new GetShouldHelpConditionData(),
            Condition.Function.HasBoundWeaponEquipped => new HasBoundWeaponEquippedConditionData(),
            Condition.Function.GetCombatTargetHasKeyword => new GetCombatTargetHasKeywordConditionData(),
            Condition.Function.GetCombatGroupMemberCount => new GetCombatGroupMemberCountConditionData(),
            Condition.Function.IsIgnoringCombat => new IsIgnoringCombatConditionData(),
            Condition.Function.GetLightLevel => new GetLightLevelConditionData(),
            Condition.Function.SpellHasCastingPerk => new SpellHasCastingPerkConditionData(),
            Condition.Function.IsBeingRidden => new IsBeingRiddenConditionData(),
            Condition.Function.IsUndead => new IsUndeadConditionData(),
            Condition.Function.GetRealHoursPassed => new GetRealHoursPassedConditionData(),
            Condition.Function.IsUnlockedDoor => new IsUnlockedDoorConditionData(),
            Condition.Function.IsHostileToActor => new IsHostileToActorConditionData(),
            Condition.Function.GetTargetHeight => new GetTargetHeightConditionData(),
            Condition.Function.IsPoison => new IsPoisonConditionData(),
            Condition.Function.WornApparelHasKeywordCount => new WornApparelHasKeywordCountConditionData(),
            Condition.Function.GetItemHealthPercent => new GetItemHealthPercentConditionData(),
            Condition.Function.EffectWasDualCast => new EffectWasDualCastConditionData(),
            Condition.Function.GetKnockedStateEnum => new GetKnockedStateEnumConditionData(),
            Condition.Function.DoesNotExist => new DoesNotExistConditionData(),
            Condition.Function.IsOnFlyingMount => new IsOnFlyingMountConditionData(),
            Condition.Function.CanFlyHere => new CanFlyHereConditionData(),
            Condition.Function.IsFlyingMountPatrolQueud => new IsFlyingMountPatrolQueudConditionData(),
            Condition.Function.IsFlyingMountFastTravelling => new IsFlyingMountFastTravellingConditionData(),
            Condition.Function.IsOverEncumbered => new IsOverEncumberedConditionData(),
            Condition.Function.GetActorWarmth => new GetActorWarmthConditionData(),
            Condition.Function.GetSKSEVersion => new GetSKSEVersionConditionData(),
            Condition.Function.GetSKSEVersionMinor => new GetSKSEVersionMinorConditionData(),
            Condition.Function.GetSKSEVersionBeta => new GetSKSEVersionBetaConditionData(),
            Condition.Function.GetSKSERelease => new GetSKSEReleaseConditionData(),
            Condition.Function.ClearInvalidRegistrations => new ClearInvalidRegistrationsConditionData(),
            _ => throw new ArgumentOutOfRangeException(nameof(function), function, null),
        };
    }
}
