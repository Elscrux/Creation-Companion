using System.Collections.ObjectModel;
using System.Reactive;
using CreationEditor.GUI.Models.Record.Browser;
using CreationEditor.GUI.ViewModels.Record;
using Mutagen.Bethesda.Skyrim;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.GUI.Skyrim.ViewModels.Record;

public class SkyrimRecordBrowserVM : ViewModel, IRecordBrowserVM {
    private readonly IRecordListFactory _recordListFactory;
    public IRecordBrowserSettings RecordBrowserSettings { get; }

    public ObservableCollection<RecordTypeGroup> RecordTypeGroups { get; }
    public ReactiveCommand<RecordTypeListing, Unit> SelectRecordType { get; }

    [Reactive] public IRecordListVM? RecordList { get; set; }
    

    public SkyrimRecordBrowserVM(
        IRecordListFactory recordListFactory,
        IRecordBrowserSettings recordBrowserSettings) {
        _recordListFactory = recordListFactory;
        RecordBrowserSettings = recordBrowserSettings;

        SelectRecordType = ReactiveCommand.Create((RecordTypeListing recordTypeListing) => {
            var recordType = recordTypeListing.Registration.GetterType;
            if (RecordList != null && RecordList.Type == recordType) return;

            RecordList = _recordListFactory.FromType(recordType, RecordBrowserSettings);
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
