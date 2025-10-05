using Avalonia.Media;
using Avalonia.Threading;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Skyrim.Avalonia.Services.Map;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Controls;

public sealed partial class WorldspaceViewVM : ViewModel {
    public IWorldspaceGetter Worldspace { get; }

    [Reactive] public partial IImage? DrawingsImage { get; set; } = null!;
    [Reactive] public partial bool IsBusy { get; set; }

    public WorldspaceViewVM(
        RegionMapCreator regionMapCreator,
        IWorldspaceGetter worldspace,
        Func<ICellGetter, Color>? cellColorProvider = null) {
        Worldspace = worldspace;
        IsBusy = true;

        Task.Run(async () => {
            var regionDrawing = await regionMapCreator.GetRegionMap(worldspace, cellColorProvider: cellColorProvider);
            Dispatcher.UIThread.Post(() => {
                DrawingsImage = new DrawingImage(regionDrawing);
                IsBusy = false;
            });
        });
    }
}
