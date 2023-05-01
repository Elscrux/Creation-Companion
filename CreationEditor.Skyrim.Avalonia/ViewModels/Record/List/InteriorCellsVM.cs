using Autofac;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Avalonia.Views.Record;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.List;

public sealed class InteriorCellsVM : ViewModel {
    public InteriorCellsProvider InteriorCellsProvider { get; }
    public RecordList InteriorList { get; }

    public InteriorCellsVM(
        IComponentContext componentContext,
        IExtraColumnsBuilder extraColumnsBuilder,
        InteriorCellsProvider interiorCellsProvider) {
        InteriorCellsProvider = interiorCellsProvider;

        var columns = extraColumnsBuilder
            .AddRecordType<ICellGetter>()
            .Build();

        InteriorList = new RecordList(columns) {
            DataContext = componentContext.Resolve<IRecordListVM>(TypedParameter.From<IRecordProvider>(InteriorCellsProvider))
        };
    }
}
