using System;
using Autofac;
using CreationEditor.GUI.Models.Record.Browser;
using CreationEditor.GUI.ViewModels.Record;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.GUI.Skyrim.ViewModels.Record;

public class SkyrimRecordListFactory : IRecordListFactory {
    private readonly ILifetimeScope _lifetimeScope;
    private readonly IRecordBrowserSettings _defaultRecordBrowserSettings;

    public SkyrimRecordListFactory(
        ILifetimeScope lifetimeScope,
        IRecordBrowserSettings defaultRecordBrowserSettings) {
        _lifetimeScope = lifetimeScope;
        _defaultRecordBrowserSettings = defaultRecordBrowserSettings;
    }
    
    public IRecordListVM FromType(Type type, IRecordBrowserSettings? browserSettings = null) {
        browserSettings ??= _defaultRecordBrowserSettings;
        var browserSettingsParam = TypedParameter.From(browserSettings);
        
        return type.Name switch {
            nameof(INpcGetter) => _lifetimeScope.Resolve<NpcListVM>(browserSettingsParam),
            // nameof(IActionRecordGetter) => _lifetimeScope.Resolve<ActionRecordListVM>(browserSettingsParam),
            // nameof(IBodyPartDataGetter) => _lifetimeScope.Resolve<BodyPartDataListVM>(browserSettingsParam),
            // nameof(ILeveledNpcGetter) => _lifetimeScope.Resolve<LeveledNpcListVM>(browserSettingsParam),
            // nameof(IPerkGetter) => _lifetimeScope.Resolve<PerkListVM>(browserSettingsParam),
            // nameof(ITalkingActivatorGetter) => _lifetimeScope.Resolve<TalkingActivatorListVM>(browserSettingsParam),
                
            // nameof(IAssociationTypeGetter) => _lifetimeScope.Resolve<AssociationTypeListVM>(browserSettingsParam),
            // nameof(IClassGetter) => _lifetimeScope.Resolve<ClassListVM>(browserSettingsParam),
            // nameof(IEquipTypeGetter) => _lifetimeScope.Resolve<EquipTypeListVM>(browserSettingsParam),
            nameof(IFactionGetter) => _lifetimeScope.Resolve<FactionListVM>(browserSettingsParam),
            // nameof(IHeadPartGetter) => _lifetimeScope.Resolve<HeadPartListVM>(browserSettingsParam),
            // nameof(IMovementTypeGetter) => _lifetimeScope.Resolve<MovementTypeListVM>(browserSettingsParam),
            // nameof(IPackageGetter) => _lifetimeScope.Resolve<PackageListVM>(browserSettingsParam),
            // nameof(IQuestGetter) => _lifetimeScope.Resolve<QuestListVM>(browserSettingsParam),
            // nameof(IRaceGetter) => _lifetimeScope.Resolve<RaceListVM>(browserSettingsParam),
            // nameof(IRelationshipGetter) => _lifetimeScope.Resolve<RelationshipListVM>(browserSettingsParam),
            // nameof(IStoryManagerEventNodeGetter) => _lifetimeScope.Resolve<StoryManagerEventNodeListVM>(browserSettingsParam),
            // nameof(IVoiceTypeGetter) => _lifetimeScope.Resolve<VoiceTypeListVM>(browserSettingsParam),
            
            _ => _lifetimeScope.Resolve<MajorRecordListVM>(TypedParameter.From(type), browserSettingsParam)
        };
    }
}
