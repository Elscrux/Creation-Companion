using System;
using System.Collections.ObjectModel;
using System.Reactive;
using CreationEditor.GUI.Models.Record;
using CreationEditor.GUI.ViewModels.Record.RecordList;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.GUI.ViewModels.Record;

public interface IRecordBrowserVM {
    public ObservableCollection<RecordTypeGroup> RecordTypeGroups { get; }
    [Reactive] public IRecordListVM RecordList { get; set; }
    public ReactiveCommand<RecordTypeListing, Unit> SelectRecordType { get; }
    public BrowserScope BrowserScope { get; set; }
}

public class SkyrimRecordBrowserVM : ViewModel, IRecordBrowserVM {
    public BrowserScope BrowserScope { get; set; } = BrowserScope.Environment;
    
    public ObservableCollection<RecordTypeGroup> RecordTypeGroups { get; }
    [Reactive] public IRecordListVM? RecordList { get; set; }
    public ReactiveCommand<RecordTypeListing, Unit> SelectRecordType { get; }

    public SkyrimRecordBrowserVM() {
        SelectRecordType = ReactiveCommand.Create((RecordTypeListing recordType) => {
            if (RecordList != null && RecordList.Type == recordType.Registration.GetterType) return;
            
            var linkCache = GetScopedLinkCache();

            RecordList = recordType.Registration.GetterType.Name switch {
                nameof(INpcGetter) => new SkyrimNpcListVM(linkCache),
                // nameof(IActionRecordGetter) => new MajorRecordListVM(recordType.Registration.GetterType),
                // nameof(IBodyPartDataGetter) => new MajorRecordListVM(recordType.Registration.GetterType),
                // nameof(ILeveledNpcGetter) => new MajorRecordListVM(recordType.Registration.GetterType),
                // nameof(IPerkGetter) => new MajorRecordListVM(recordType.Registration.GetterType),
                // nameof(ITalkingActivatorGetter) => new MajorRecordListVM(recordType.Registration.GetterType),
                
                // nameof(IAssociationTypeGetter) => new MajorRecordListVM(recordType.Registration.GetterType),
                // nameof(IClassGetter) => new MajorRecordListVM(recordType.Registration.GetterType),
                // nameof(IEquipTypeGetter) => new MajorRecordListVM(recordType.Registration.GetterType),
                // nameof(IFactionGetter) => new MajorRecordListVM(recordType.Registration.GetterType),
                // nameof(IHeadPartGetter) => new MajorRecordListVM(recordType.Registration.GetterType),
                // nameof(IMovementTypeGetter) => new MajorRecordListVM(recordType.Registration.GetterType),
                // nameof(IPackageGetter) => new MajorRecordListVM(recordType.Registration.GetterType),
                // nameof(IQuestGetter) => new MajorRecordListVM(recordType.Registration.GetterType),
                // nameof(IRaceGetter) => new MajorRecordListVM(recordType.Registration.GetterType),
                // nameof(IRelationshipGetter) => new MajorRecordListVM(recordType.Registration.GetterType),
                // nameof(IStoryManagerEventNodeGetter) => new MajorRecordListVM(recordType.Registration.GetterType),
                // nameof(IVoiceTypeGetter) => new MajorRecordListVM(recordType.Registration.GetterType),
                _ => new MajorRecordListVM(linkCache, recordType.Registration.GetterType),
            };
        });
        
        RecordTypeGroups = new ObservableCollection<RecordTypeGroup> {
            new("Actors",
                new ObservableCollection<RecordTypeListing> {
                    new(INpcGetter.StaticRegistration),
                    new(IActionRecordGetter.StaticRegistration),
                    new(IBodyPartDataGetter.StaticRegistration),
                    new(ILeveledNpcGetter.StaticRegistration),
                    new(IPerkGetter.StaticRegistration),
                    new(ITalkingActivatorGetter.StaticRegistration),
                }),
            new("Audio",
                new ObservableCollection<RecordTypeListing> {
                    new(IAcousticSpaceGetter.StaticRegistration),
                    new(IMusicTrackGetter.StaticRegistration),
                    new(IMusicTypeGetter.StaticRegistration),
                    new(IReverbParametersGetter.StaticRegistration),
                    new(ISoundCategoryGetter.StaticRegistration),
                    new(ISoundDescriptorGetter.StaticRegistration),
                    new(ISoundMarkerGetter.StaticRegistration),
                    new(ISoundOutputModelGetter.StaticRegistration),
                }),
            new("Character",
                new ObservableCollection<RecordTypeListing> {
                    new(IAssociationTypeGetter.StaticRegistration),
                    new(IClassGetter.StaticRegistration),
                    new(IEquipTypeGetter.StaticRegistration),
                    new(IFactionGetter.StaticRegistration),
                    new(IHeadPartGetter.StaticRegistration),
                    new(IMovementTypeGetter.StaticRegistration),
                    new(IPackageGetter.StaticRegistration),
                    new(IQuestGetter.StaticRegistration),
                    new(IRaceGetter.StaticRegistration),
                    new(IRelationshipGetter.StaticRegistration),
                    new(IStoryManagerEventNodeGetter.StaticRegistration),
                    new(IVoiceTypeGetter.StaticRegistration),
                }),
        };
        
        Editor.EditorInitialized += (_, _) => UpdateScope();
    }
    
    private ILinkCache GetScopedLinkCache() {
        return BrowserScope switch {
            BrowserScope.Environment => Editor.Instance.LinkCache,
            BrowserScope.ActiveMod => Editor.Instance.ActiveMod.ToImmutableLinkCache(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void UpdateScope() {
        if (RecordList == null) return;

        RecordList.Scope = GetScopedLinkCache();
    }
}
