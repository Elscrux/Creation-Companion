using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Services.Filter;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Avalonia.Services.Record.Provider;

public sealed class RecordListVMBuilder(
    IRecordProviderFactory recordProviderFactory,
    Func<IRecordProvider, Func<IObservable<IMajorRecordGetter?>, IRecordContextMenuProvider>, IRecordListVM> recordListVMFactory,
    Func<IObservable<IMajorRecordGetter?>, UntypedRecordContextMenuProvider> defaultContextMenuProviderFactory,
    IRecordBrowserSettings defaultRecordBrowserSettings)
    : IRecordListVMBuilder {

    private Func<IObservable<IMajorRecordGetter?>, IRecordContextMenuProvider> _contextMenuProviderFactory = defaultContextMenuProviderFactory;
    private IExtraColumnsBuilder? _extraColumnsBuilder;

    public IRecordListVMBuilder WithExtraColumns(IExtraColumnsBuilder extraColumnsBuilder) {
        _extraColumnsBuilder = extraColumnsBuilder;
        return this;
    }

    public IRecordListVMBuilder WithContextMenuProviderFactory(Func<IObservable<IMajorRecordGetter?>, IRecordContextMenuProvider> contextMenuProviderFactory) {
        _contextMenuProviderFactory = contextMenuProviderFactory;
        return this;
    }

    public IRecordListVMBuilder WithBrowserSettings(IRecordBrowserSettings browserSettings) {
        defaultRecordBrowserSettings = browserSettings;
        return this;
    }

    public IRecordListVM BuildWithSource(IEnumerable<IFormLinkIdentifier> identifiers) {
        var recordProvider = recordProviderFactory.FromIdentifiers(identifiers, defaultRecordBrowserSettings);
        return BuildWithSource(recordProvider);
    }

    public IRecordListVM BuildWithSource(Type type) {
        var recordProvider = recordProviderFactory.FromType(type, defaultRecordBrowserSettings);
        return BuildWithSource(recordProvider);
    }

    public IRecordListVM BuildWithSource(IRecordProvider recordProvider) {
        var recordListVM = recordListVMFactory(recordProvider, _contextMenuProviderFactory);
        recordProvider.DisposeWith(recordListVM);

        if (_extraColumnsBuilder is not null) {
            recordListVM.Columns.AddRange(_extraColumnsBuilder.Build());
        }

        return recordListVM;
    }
}
