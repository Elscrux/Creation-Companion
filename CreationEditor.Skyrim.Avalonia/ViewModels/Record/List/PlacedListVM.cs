using System;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Skyrim.Avalonia.Services.Record.Actions;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;

public sealed class PlacedListVM : ViewModel {
    public PlacedProvider PlacedProvider { get; }

    public IRecordListVM RecordListVM { get; }

    public PlacedListVM(
        IRecordListVMBuilder recordListVMBuilder,
        IExtraColumnsBuilder extraColumnsBuilder,
        PlacedProvider placedProvider,
        Func<IObservable<IPlacedGetter?>, PlacedContextMenuProvider> cellContextMenuProviderFactory) {
        PlacedProvider = placedProvider.DisposeWith(this);

        RecordListVM = recordListVMBuilder
            .WithExtraColumns(extraColumnsBuilder.AddRecordType<IPlacedGetter>())
            .WithContextMenuProviderFactory(CreateContextMenuProvider)
            .BuildWithSource(PlacedProvider)
            .DisposeWith(this);

        IRecordContextMenuProvider CreateContextMenuProvider(IObservable<IMajorRecordGetter?> selectedRecords)
            => cellContextMenuProviderFactory(selectedRecords!.OfType<IPlacedGetter>());
    }
}
