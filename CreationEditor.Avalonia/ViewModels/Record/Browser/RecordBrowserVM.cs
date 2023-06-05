using System.Reactive;
using Autofac;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Browser;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.ViewModels.Record.List;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.Browser;

public sealed class RecordBrowserVM : ViewModel, IRecordBrowserVM {
    private readonly ILifetimeScope _lifetimeScope;
    private readonly IRecordListFactory _recordListFactory;

    public IRecordBrowserSettingsVM RecordBrowserSettingsVM { get; }
    [Reactive] public IRecordListVM? RecordListVM { get; set; }

    public IObservableCollection<RecordTypeGroup> RecordTypeGroups { get; }

    public ReactiveCommand<RecordTypeGroup, Unit> SelectRecordTypeGroup { get; }
    public ReactiveCommand<RecordTypeListing, Unit> SelectRecordType { get; }
    public ReactiveCommand<RecordFilterListing, Unit> SelectRecordFilter { get; }

    private Type? _recordListType;

    public RecordBrowserVM(
        ILifetimeScope lifetimeScope,
        IRecordBrowserGroupProvider recordBrowserGroupProvider,
        IRecordListFactory recordListFactory,
        IRecordBrowserSettingsVM recordBrowserSettingsVM) {
        _lifetimeScope = lifetimeScope;
        _recordListFactory = recordListFactory;
        RecordBrowserSettingsVM = recordBrowserSettingsVM;
        RecordTypeGroups = new ObservableCollectionExtended<RecordTypeGroup>(recordBrowserGroupProvider.GetRecordGroups());

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
        RecordListVM?.Dispose();
        var recordListVM = _recordListFactory.FromType(recordType, RecordBrowserSettingsVM);

        var newScope = _lifetimeScope.BeginLifetimeScope();
        var extraColumnsBuilder = newScope.Resolve<IExtraColumnsBuilder>();

        recordListVM.Columns.AddRange(extraColumnsBuilder
            .AddRecordType(recordType)
            .Build());

        RecordListVM = recordListVM;

        _recordListType = recordType;
    }

    public override void Dispose() {
        base.Dispose();

        RecordListVM?.Dispose();
    }
}
