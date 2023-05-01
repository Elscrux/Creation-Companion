using Autofac;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Avalonia.Views.Record;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;

public sealed class InteriorCellsVM : ViewModel {
    public InteriorCellsProvider InteriorCellsProvider { get; }
    public RecordList InteriorList { get; }

    public InteriorCellsVM(
        ILifetimeScope lifetimeScope,
        IExtraColumnsBuilder extraColumnsBuilder,
        InteriorCellsProvider interiorCellsProvider) {
        InteriorCellsProvider = interiorCellsProvider;

        var columns = extraColumnsBuilder
            .AddRecordType<ICellGetter>()
            .Build();

        var newScope = lifetimeScope.BeginLifetimeScope();
        var recordListVM = newScope.Resolve<IRecordListVM>(TypedParameter.From<IRecordProvider>(InteriorCellsProvider));
        newScope.DisposeWith(recordListVM);

        InteriorList = new RecordList(columns) { DataContext = recordListVM };
    }

    public override void Dispose() {
        base.Dispose();

        InteriorList?.ViewModel?.Dispose();
    }
}
