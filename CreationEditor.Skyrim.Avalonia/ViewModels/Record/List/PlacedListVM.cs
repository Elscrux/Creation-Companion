using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;

public sealed class PlacedListVM : ViewModel {
    public PlacedProvider PlacedProvider { get; }

    public IRecordListVM RecordListVM { get; }
    public IList<DataGridColumn> PlacedListColumns { get; }

    public PlacedListVM(
        Func<IRecordProvider, IRecordListVM> recordListVMFactory,
        IExtraColumnsBuilder extraColumnsBuilder,
        PlacedProvider placedProvider) {
        PlacedProvider = placedProvider.DisposeWith(this);

        PlacedListColumns = extraColumnsBuilder
            .AddRecordType<IPlacedGetter>()
            .Build()
            .ToList();

        RecordListVM = recordListVMFactory(PlacedProvider);
    }
}
