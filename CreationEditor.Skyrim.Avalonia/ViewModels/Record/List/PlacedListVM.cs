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

public sealed class PlacedListVM : ViewModel {
    public PlacedProvider PlacedProvider { get; }
    public RecordList? PlacedList { get; }

    public PlacedListVM(
        ILifetimeScope lifetimeScope,
        IExtraColumnsBuilder extraColumnsBuilder,
        PlacedProvider placedProvider) {
        PlacedProvider = placedProvider;

        var columns = extraColumnsBuilder
            .AddRecordType<IPlacedGetter>()
            .Build();

        var newScope = lifetimeScope.BeginLifetimeScope();
        var recordListVM = lifetimeScope.Resolve<IRecordListVM>(TypedParameter.From<IRecordProvider>(PlacedProvider));
        newScope.DisposeWith(recordListVM);

        PlacedList = new RecordList(columns) { DataContext = recordListVM };
    }

    public override void Dispose() {
        base.Dispose();

        PlacedProvider.Dispose();
    }
}
