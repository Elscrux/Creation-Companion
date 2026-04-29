using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Browser;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Services.Filter;
using DynamicData.Binding;
using Noggog;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Record.Browser;

public sealed partial class RecordBrowserVM : ViewModel, IRecordBrowserVM {
    private readonly Func<IExtraColumnsBuilder> _extraColumnsBuilderFactory;
    private readonly IRecordListVMBuilder _recordListVMBuilder;

    public IRecordBrowserSettings RecordBrowserSettings { get; }
    [Reactive] public partial IRecordListVM? RecordListVM { get; set; }

    public IObservableCollection<RecordTypeGroup> RecordTypeGroups { get; }

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
    }

    [ReactiveCommand]
    private void SelectRecordFilter(RecordFilterListing recordFilterListing) {
        var parent = recordFilterListing.Parent;
        while (parent is RecordFilterListing { Parent: {} grandparent }) {
            parent = grandparent;
        }

        if (parent is RecordTypeListing recordTypeListing) {
            if (recordTypeListing.Registration.GetterType != _recordListType) SetRecordList(recordTypeListing.Registration.GetterType);
            RecordBrowserSettings.CustomFilter = recordFilterListing.Filter;
        }
    }

    [ReactiveCommand]
    private void SelectRecordType(RecordTypeListing recordTypeListing) {
        var recordType = recordTypeListing.Registration.GetterType;
        RecordBrowserSettings.CustomFilter = null;

        if (_recordListType == recordType) return;

        SetRecordList(recordType);
    }

    [ReactiveCommand]
    private void SelectRecordTypeGroup(RecordTypeGroup group) {
        group.Activate();
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
