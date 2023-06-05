using System.Collections.Generic;
using System.Linq;
using Autofac;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;

public sealed class InteriorCellsVM : ViewModel {
    public InteriorCellsProvider InteriorCellsProvider { get; }

    public IRecordListVM RecordListVM { get; }
    public IList<DataGridColumn> InteriorListColumns { get; }

    public InteriorCellsVM(
        ILifetimeScope lifetimeScope,
        IExtraColumnsBuilder extraColumnsBuilder,
        InteriorCellsProvider interiorCellsProvider) {
        InteriorCellsProvider = interiorCellsProvider.DisposeWith(this);

        InteriorListColumns = extraColumnsBuilder
            .AddRecordType<ICellGetter>()
            .Build()
            .ToList();

        var newScope = lifetimeScope.BeginLifetimeScope();
        RecordListVM = newScope
            .Resolve<IRecordListVM>(TypedParameter.From<IRecordProvider>(InteriorCellsProvider))
            .DisposeWith(this);

        newScope.DisposeWith(this);
    }
}
