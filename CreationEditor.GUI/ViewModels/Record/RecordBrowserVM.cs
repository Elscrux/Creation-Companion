using System.Collections.ObjectModel;
using System.Reactive;
using CreationEditor.GUI.Models.Record.RecordBrowser;
using CreationEditor.GUI.ViewModels.Record.RecordList;
using Mutagen.Bethesda.Skyrim;
using MutagenLibrary.References.ReferenceCache;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.GUI.ViewModels.Record;

public interface IRecordBrowserVM {
    public ObservableCollection<RecordTypeGroup> RecordTypeGroups { get; }
    [Reactive] public IRecordListVM RecordList { get; set; }
    public ReactiveCommand<RecordTypeListing, Unit> SelectRecordType { get; }
    public IRecordBrowserSettings RecordBrowserSettings { get; }
}

public class SkyrimRecordBrowserVM : ViewModel, IRecordBrowserVM {
    private readonly IReferenceQuery _referenceQuery;
    public IRecordBrowserSettings RecordBrowserSettings { get; }

    public ObservableCollection<RecordTypeGroup> RecordTypeGroups { get; }
    public ReactiveCommand<RecordTypeListing, Unit> SelectRecordType { get; }

    [Reactive] public IRecordListVM? RecordList { get; set; }
    

    public SkyrimRecordBrowserVM(
        IReferenceQuery referenceQuery,
        IRecordBrowserSettings recordBrowserSettings) {
        _referenceQuery = referenceQuery;
        RecordBrowserSettings = recordBrowserSettings;

        SelectRecordType = ReactiveCommand.Create((RecordTypeListing recordType) => {
            if (RecordList != null && RecordList.Type == recordType.Registration.GetterType) return;

            RecordList = recordType.Registration.GetterType.Name switch {
                nameof(INpcGetter) => new SkyrimNpcListVM(RecordBrowserSettings, _referenceQuery),
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
                _ => new MajorRecordListVM(recordType.Registration.GetterType, RecordBrowserSettings, _referenceQuery),
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
    }
}
