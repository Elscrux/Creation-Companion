using System.Collections.ObjectModel;
using System.Reactive;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Browser;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.Views.Record;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.Browser;

public sealed class RecordBrowserVM : ViewModel, IRecordBrowserVM {
    private readonly IRecordListFactory _recordListFactory;
    public IRecordBrowserSettingsVM RecordBrowserSettingsVM { get; }

    public ObservableCollection<RecordTypeGroup> RecordTypeGroups { get; }

    public ReactiveCommand<RecordTypeGroup, Unit> SelectRecordTypeGroup { get; }
    public ReactiveCommand<RecordTypeListing, Unit> SelectRecordType { get; }
    public ReactiveCommand<RecordFilterListing, Unit> SelectRecordFilter { get; }

    private Type? _recordListType;
    [Reactive] public RecordList? RecordList { get; set; }

    public RecordBrowserVM(
        IRecordBrowserGroupProvider recordBrowserGroupProvider,
        IRecordListFactory recordListFactory,
        IRecordBrowserSettingsVM recordBrowserSettingsVM) {
        _recordListFactory = recordListFactory;
        RecordBrowserSettingsVM = recordBrowserSettingsVM;
        RecordTypeGroups = new ObservableCollection<RecordTypeGroup>(recordBrowserGroupProvider.GetRecordGroups());

        SelectRecordTypeGroup = ReactiveCommand.Create<RecordTypeGroup>(group => group.Activate());

        SelectRecordType = ReactiveCommand.Create<RecordTypeListing>(recordTypeListing => {
            var recordType = recordTypeListing.Registration.GetterType;
            RecordBrowserSettingsVM.RecordFilter = null;

            if (_recordListType == recordType) return;
            SetRecordList(recordType);
        });

        SelectRecordFilter = ReactiveCommand.Create<RecordFilterListing>(recordFilterListing => {
            var parent = recordFilterListing.Parent;
            while (parent is RecordFilterListing { Parent: {} grandparent }) {
                parent = grandparent;
            }

            if (parent is RecordTypeListing recordTypeListing) {
                if (recordTypeListing.Registration.GetterType != _recordListType) SetRecordList(recordTypeListing.Registration.GetterType);
                RecordBrowserSettingsVM.RecordFilter = recordFilterListing.Filter;
            }
        });
    }
    private void SetRecordList(Type recordType) {
        RecordList?.ViewModel?.Dispose();
        RecordList = _recordListFactory.FromType(recordType, RecordBrowserSettingsVM);
        _recordListType = recordType;
    }
}