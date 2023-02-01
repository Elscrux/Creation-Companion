using Autofac;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Avalonia.Views.Record;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Provider;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.List; 

public class PlacedListVM {
    public PlacedProvider PlacedProvider { get; }
    public RecordList? PlacedList { get; }

    public PlacedListVM(
        IComponentContext componentContext,
        IExtraColumnsBuilder extraColumnsBuilder,
        PlacedProvider placedProvider) {
        PlacedProvider = placedProvider;

        var columns = extraColumnsBuilder
            .AddRecordType<IPlacedGetter>()
            .Build();
        
        PlacedList = new RecordList(columns) {
            DataContext = componentContext.Resolve<IRecordListVM>(TypedParameter.From<IRecordProvider>(PlacedProvider))
        };
    }
}
