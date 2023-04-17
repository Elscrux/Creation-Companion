using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Resources.Constants; 

public class ConditionConstants {
    public static readonly IEnumerable<Condition.Function> PerkOnlyFunctions
        = new[] {
            Condition.Function.EPAlchemyEffectHasKeyword,
            Condition.Function.EPAlchemyGetMakingPoison,
            Condition.Function.EPMagic_IsAdvanceSkill,
            Condition.Function.EPMagic_SpellHasKeyword,
            Condition.Function.EPMagic_SpellHasSkill,
            Condition.Function.EPTemperingItemHasKeyword,
            Condition.Function.EPTemperingItemIsEnchanted,
            Condition.Function.EPModSkillUsage_IsAdvanceAction,
            Condition.Function.EPModSkillUsage_AdvanceObjectHasKeyword,
        };

    public static readonly IEnumerable<Condition.Function> CameraPathOnlyFunctions
        = new[] {
            Condition.Function.GetVatsTargetHeight,
            Condition.Function.GetVATSValue,
            Condition.Function.GetVATSMode,
            Condition.Function.GetVATSBackAreaFree,
            Condition.Function.GetVATSBackTargetVisible,
            Condition.Function.GetVATSFrontAreaFree,
            Condition.Function.GetVATSFrontTargetVisible,
            Condition.Function.GetVATSLeftAreaFree,
            Condition.Function.GetVATSLeftTargetVisible,
            Condition.Function.GetVATSRightAreaFree,
            Condition.Function.GetVATSRightTargetVisible,
        };

    public static readonly IEnumerable<Condition.Function> QuestOnlyFunctions
        = new[] {
            Condition.Function.GetInCurrentLocAlias,
            Condition.Function.GetIsAliasRef,
            Condition.Function.GetIsEditorLocAlias,
            Condition.Function.GetKeywordDataForAlias,
            Condition.Function.GetLocAliasRefTypeAliveCount,
            Condition.Function.GetLocAliasRefTypeDeadCount,
            Condition.Function.GetLocationAliasCleared,
            Condition.Function.HasSameEditorLocAsRefAlias,
            Condition.Function.IsInSameCurrentLocAsRefAlias,
            Condition.Function.IsLocAliasLoaded,
            Condition.Function.LocAliasHasKeyword,
            Condition.Function.LocAliasIsLocation,
        };

    public static readonly IEnumerable<Condition.Function> QuestAndStoryManagerOnlyFunctions
        = new[] {
            Condition.Function.GetEventData
        };

    public static readonly IEnumerable<Condition.Function> PackageOnlyFunctions
        = new[] {
            Condition.Function.GetNumericPackageData,
            Condition.Function.GetWithinPackageLocation,
            Condition.Function.IsNullPackageData,
            Condition.Function.IsContinuingPackagePCNear,
        };

    public static readonly IEnumerable<Condition.Function> DisabledFunctions
        = new[] {
            Condition.Function.MenuMode,
            Condition.Function.GetScriptVariable,
            Condition.Function.GetQuestVariable,
            Condition.Function.GetIsCreature,
            Condition.Function.GetIsCreatureType,
            Condition.Function.IsIdlePlaying,
            Condition.Function.GetPersuasionNumber,
            Condition.Function.GetTotalPersuasionNumber,
            Condition.Function.GetClassDefaultMatch,
            Condition.Function.IsHorseStolen,
            Condition.Function.GetHitLocation,
            Condition.Function.GetIsAlignment,
            Condition.Function.GetConcussed,
            Condition.Function.GetKillingBlowLimb,
        };

    public static readonly IEnumerable<Condition.Function> BaseFunctions
        = Enum.GetValues<Condition.Function>()
            .Except(PerkOnlyFunctions)
            .Except(CameraPathOnlyFunctions)
            .Except(QuestAndStoryManagerOnlyFunctions)
            .Except(QuestOnlyFunctions)
            .Except(PackageOnlyFunctions)
            .Except(DisabledFunctions)
            .OrderBy(x => x.ToString());
}
