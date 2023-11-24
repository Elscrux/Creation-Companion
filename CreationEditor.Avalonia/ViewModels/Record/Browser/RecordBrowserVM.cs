using System.Reactive;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Browser;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Services.Filter;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.Browser;

public sealed class RecordBrowserVM : ViewModel, IRecordBrowserVM {
    private readonly Func<IExtraColumnsBuilder> _extraColumnsBuilderFactory;
    private readonly IRecordListVMBuilder _recordListVMBuilder;

    public IRecordBrowserSettings RecordBrowserSettings { get; }
    [Reactive] public IRecordListVM? RecordListVM { get; set; }

    public IObservableCollection<RecordTypeGroup> RecordTypeGroups { get; }

    public ReactiveCommand<RecordTypeGroup, Unit> SelectRecordTypeGroup { get; }
    public ReactiveCommand<RecordTypeListing, Unit> SelectRecordType { get; }
    public ReactiveCommand<RecordFilterListing, Unit> SelectRecordFilter { get; }

    private Type? _recordListType;

    public RecordBrowserVM(
        Func<IExtraColumnsBuilder> extraColumnsBuilderFactory,
        IRecordBrowserGroupProvider recordBrowserGroupProvider,
        IRecordListVMBuilder recordListVMBuilder,
        IRecordBrowserSettings recordBrowserSettingsVM) {
        _extraColumnsBuilderFactory = extraColumnsBuilderFactory;
        _recordListVMBuilder = recordListVMBuilder;
        RecordBrowserSettings = recordBrowserSettingsVM;
        RecordTypeGroups = new ObservableCollectionExtended<RecordTypeGroup>(recordBrowserGroupProvider.GetRecordGroups());

        SelectRecordTypeGroup = ReactiveCommand.Create<RecordTypeGroup>(group => group.Activate());

        SelectRecordType = ReactiveCommand.Create<RecordTypeListing>(recordTypeListing => {
            var recordType = recordTypeListing.Registration.GetterType;
            RecordBrowserSettings.CustomFilter = null;

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
                RecordBrowserSettings.CustomFilter = recordFilterListing.Filter;
            }
        });
    }

    private void SetRecordList(Type recordType) {
        // Cleanup
        RecordListVM?.Dispose();

        // Create new record list
        var recordListVM = _recordListVMBuilder
            .WithExtraColumns(_extraColumnsBuilderFactory().AddRecordType(recordType))
            .WithBrowserSettings(RecordBrowserSettings)
            .BuildWithSource(recordType)
            .DisposeWith(this);

        // Complete setup
        _recordListType = recordType;
        RecordListVM = recordListVM;
    }
}
