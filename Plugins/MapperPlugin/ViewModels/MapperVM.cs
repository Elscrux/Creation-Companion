using System.Collections;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CreationEditor;
using CreationEditor.Avalonia;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Query;
using CreationEditor.Core;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Services.Mutagen.Type;
using CreationEditor.Services.Query.From;
using CreationEditor.Services.State;
using CreationEditor.Skyrim.Avalonia.Resources.Constants;
using CreationEditor.Skyrim.Avalonia.Services.Map;
using DynamicData.Binding;
using MapperPlugin.Model;
using MapperPlugin.Services;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
namespace MapperPlugin.ViewModels;

public sealed partial class MapperVM : ViewModel, IMementoProvider<MapperMemento>, IMementoReceiver<MapperMemento> {
    private readonly Func<QueryVM> _queryVMFactory;
    private readonly IList<QueryFromItem> _placeableQueryFromItems;

    public HeatmapCreator HeatmapCreator { get; }
    public VertexColorMapCreator VertexColorMapCreator { get; }
    public HeightmapCreator HeightmapCreator { get; }

    [Reactive] public partial int BusyTasks { get; set; }

    public ILinkCacheProvider LinkCacheProvider { get; }

    [Reactive] public partial FormKey WorldspaceFormKey { get; set; }

    [Reactive] public partial bool ShowRegions { get; set; }
    [Reactive] public partial bool ShowVertexColor { get; set; }
    [Reactive] public partial bool ShowHeightmap { get; set; }

    [Reactive] public partial int TopCell { get; set; }
    [Reactive] public partial int BottomCell { get; set; }
    [Reactive] public partial int LeftCell { get; set; }
    [Reactive] public partial int RightCell { get; set; }

    [Reactive] public partial int MarkingSize { get; set; }

    [Reactive] public partial DrawingImage? DrawingsImage { get; set; }
    [Reactive] public partial DrawingImage? RegionsImage { get; set; }
    [Reactive] public partial IImage? VertexColorImage { get; set; }
    [Reactive] public partial IImage? HeightmapImage { get; set; }
    [Reactive] public partial IImage? ImageSource { get; set; }
    [Reactive] public partial string? ImageFilePath { get; set; }

    public IObservableCollection<MarkingMapping> Mappings { get; }

    public ReactiveCommand<Unit, Unit> AddMapping { get; }
    public ReactiveCommand<IList, Unit> RemoveMapping { get; }
    public ReactiveCommand<string, Unit> SaveMap { get; }
    public ReactiveCommand<NamedGuid, Unit> LoadMap { get; }
    public ReactiveCommand<NamedGuid, Unit> DeleteMap { get; }
    public IObservableCollection<NamedGuid> SavedMaps { get; } = new ObservableCollectionExtended<NamedGuid>();

    public MapperVM(
        Func<QueryVM> queryVMFactory,
        IStateRepositoryFactory<MapperMemento, MapperMemento, NamedGuid> stateRepositoryFactory,
        ILogger logger,
        RegionMapCreator regionMapCreator,
        IGameReleaseContext gameReleaseContext,
        IReferenceService referenceService,
        IMutagenTypeProvider mutagenTypeProvider,
        ILinkCacheProvider linkCacheProvider) {
        _queryVMFactory = queryVMFactory;
        HeatmapCreator = new HeatmapCreator();
        VertexColorMapCreator = new VertexColorMapCreator();
        HeightmapCreator = new HeightmapCreator();
        LinkCacheProvider = linkCacheProvider;
        var stateRepository = stateRepositoryFactory.Create("Heatmap");
        TopCell = 64;
        BottomCell = -64;
        LeftCell = -64;
        RightCell = 64;
        MarkingSize = 10;
        SavedMaps.AddRange(stateRepository.LoadAllIdentifiers());

        var registrationsByGetterType = mutagenTypeProvider
            .GetRegistrations(gameReleaseContext.Release)
            .ToDictionary(x => x.GetterType, x => x);

        _placeableQueryFromItems = RecordTypeConstants.PlaceableTypes
            .Select(type => registrationsByGetterType[type])
            .Select(registration => new QueryFromItem(registration.Name, registration.GetterType))
            .ToList();

        Mappings = new ObservableCollectionExtended<MarkingMapping> { CreateMapping() };

        AddMapping = ReactiveCommand.Create(() => Mappings.Add(CreateMapping()));
        RemoveMapping = ReactiveCommand.Create<IList>(removeMappings => {
            foreach (var removeMapping in removeMappings.OfType<MarkingMapping>().ToList()) {
                Mappings.Remove(removeMapping);
            }
        });

        SaveMap = ReactiveCommand.Create<string>(name => {
            var memento = CreateMemento();
            var newGuid = Guid.NewGuid();
            stateRepository.Save(memento, new NamedGuid(newGuid, name));
            foreach (var id in SavedMaps.Where(x => x.Name == name).ToList()) {
                SavedMaps.Remove(id);
                stateRepository.Delete(id);
            }
            SavedMaps.Add(new NamedGuid(newGuid, name));
        });

        LoadMap = ReactiveCommand.Create<NamedGuid>(id => {
            var memento = stateRepository.Load(id);
            if (memento is not null) {
                RestoreMemento(memento);
            }
        });

        DeleteMap = ReactiveCommand.Create<NamedGuid>(id => {
            stateRepository.Delete(id);
            SavedMaps.RemoveWhere(x => x.Id == id.Id);
        });

        // Logical update
        var logicalMappingUpdates = Mappings
            .WhenCollectionChanges()
            .Select(_ => Mappings.Select(m => m.LogicalUpdates).CombineLatest())
            .Switch();

        var gridUpdates = this.WhenAnyValue(
            x => x.LeftCell,
            x => x.TopCell,
            x => x.RightCell,
            x => x.BottomCell,
            (left, top, right, bottom) => (Left: left, Top: top, Right: right, Bottom: bottom))
            .ThrottleMedium();

        this.WhenAnyValue(x => x.WorldspaceFormKey)
            .Where(worldspace => !worldspace.IsNull)
            .CombineLatest(
                logicalMappingUpdates,
                LinkCacheProvider.LinkCacheChanged.Do(_ => HeatmapCreator.ClearCache()),
                (x, _, _) => x)
            .ThrottleMedium()
            .ObserveOnGui()
            .Do(_ => BusyTasks++)
            .ObserveOnTaskpool()
            .DoTask(async worldspace => await HeatmapCreator.CalculateSpots(
                Mappings,
                LinkCacheProvider.LinkCache,
                referenceService,
                worldspace))
            .ObserveOnGui()
            .Do(_ => DrawingsImage = HeatmapCreator.GetDrawing(ImageSource!.Size, MarkingSize, LeftCell, RightCell, TopCell, BottomCell))
            .Do(_ => BusyTasks--)
            .Subscribe()
            .DisposeWith(this);

        this.WhenAnyValue(
                x => x.ShowRegions,
                x => x.WorldspaceFormKey,
                (showRegions, worldspace) => (Show: showRegions, WorldspaceFormKey: worldspace))
            .CombineLatest(gridUpdates, (x, grid) => (x.Show, x.WorldspaceFormKey, Grid: grid))
            .ObserveOnGui()
            .Do(_ => BusyTasks++)
            .ObserveOnTaskpool()
            .Select(x => {
                if (!x.Show || !LinkCacheProvider.LinkCache.TryResolve<IWorldspaceGetter>(x.WorldspaceFormKey, out var worldspace)) {
                    return Task.FromCanceled<DrawingGroup>(CancellationToken.None);
                }

                var cellSize = ImageSource!.Size.Width / (RightCell - LeftCell);
                return regionMapCreator.GetRegionMap(worldspace, LeftCell, TopCell, RightCell, BottomCell, (int) cellSize);
            })
            .ObserveOnGui()
            .Do(async void (newImage) => {
                try {
                    if (newImage.IsCanceled) return;

                    try {
                        var drawingGroup = await newImage;
                        drawingGroup.Opacity = 0.5;
                        RegionsImage = new DrawingImage(drawingGroup);
                    } catch (Exception e) {
                        logger.Here().Error(e, "Failed to create region map: {Exception}", e.Message);
                    }
                } catch (Exception e) {
                    logger.Here().Error(e, "Failed to create region map: {Exception}", e.Message);
                }
            })
            .Do(_ => BusyTasks--)
            .Subscribe()
            .DisposeWith(this);

        this.WhenAnyValue(
                x => x.ShowVertexColor,
                x => x.WorldspaceFormKey,
                (showVertexColor, worldspace) => (Show: showVertexColor, WorldspaceFormKey: worldspace))
            .CombineLatest(gridUpdates, (x, grid) => (x.Show, x.WorldspaceFormKey, Grid: grid))
            .ObserveOnGui()
            .Do(_ => BusyTasks++)
            .ObserveOnTaskpool()
            .Select(x => {
                if (x.Show && LinkCacheProvider.LinkCache.TryResolve<IWorldspaceGetter>(x.WorldspaceFormKey, out var worldspace)) {
                    return VertexColorMapCreator.GetVertexColorMap(worldspace, ImageSource!.Size, x.Grid.Left, x.Grid.Right, x.Grid.Top, x.Grid.Bottom);
                }

                return null;
            })
            .ObserveOnGui()
            .Do(newImage => VertexColorImage = newImage)
            .Do(_ => BusyTasks--)
            .Subscribe()
            .DisposeWith(this);

        this.WhenAnyValue(
                x => x.ShowHeightmap,
                x => x.WorldspaceFormKey,
                (showHeightmap, worldspace) => (Show: showHeightmap, WorldspaceFormKey: worldspace))
            .CombineLatest(gridUpdates, (x, grid) => (x.Show, x.WorldspaceFormKey, Grid: grid))
            .ObserveOnGui()
            .Do(_ => BusyTasks++)
            .ObserveOnTaskpool()
            .Select(x => {
                if (x.Show && LinkCacheProvider.LinkCache.TryResolve<IWorldspaceGetter>(x.WorldspaceFormKey, out var worldspace)) {
                    return HeightmapCreator.GetHeightmap(worldspace, ImageSource!.Size, x.Grid.Left, x.Grid.Right, x.Grid.Top, x.Grid.Bottom);
                }

                return null;
            })
            .ObserveOnGui()
            .Do(newImage => HeightmapImage = newImage)
            .Do(_ => BusyTasks--)
            .Subscribe()
            .DisposeWith(this);

        // Visual  update
        var visualMappingUpdates = Mappings
            .WhenCollectionChanges()
            .Select(_ => Mappings.Select(m => m.VisualUpdates).CombineLatest())
            .Switch();

        this.WhenAnyValue(x => x.ImageSource)
            .NotNull()
            .CombineLatest(this.WhenAnyValue(x => x.MarkingSize), (image, markingSize) => (Image: image, MarkingSize: markingSize))
            .CombineLatest(gridUpdates, (x, grid) => (x.Image, x.MarkingSize, Grid: grid))
            .CombineLatest(visualMappingUpdates, (x, y) => x)
            .ThrottleMedium()
            .ObserveOnGui()
            .Do(_ => BusyTasks++)
            .Do(x => DrawingsImage = HeatmapCreator.GetDrawing(x.Image.Size, x.MarkingSize, x.Grid.Left, x.Grid.Right, x.Grid.Top, x.Grid.Bottom))
            .Do(_ => BusyTasks--)
            .Subscribe()
            .DisposeWith(this);

        this.WhenAnyValue(x => x.ImageFilePath)
            .Subscribe(path => {
                if (path is null) {
                    ImageSource = null;
                    return;
                }

                try {
                    ImageSource = new Bitmap(path);
                } catch (Exception e) {
                    logger.Here().Error(e, "Failed to load image: {Exception}", e.Message);
                }
            })
            .DisposeWith(this);
    }

    private MarkingMapping CreateMapping() {
        var queryVM = _queryVMFactory();
        // Use just placeable types
        queryVM.QueryRunner.QueryFrom.Items.ReplaceWith(_placeableQueryFromItems);
        // Just get the records in any order
        queryVM.QueryRunner.EnableOrderBy = false;
        queryVM.QueryRunner.EnableSelect = false;

        return new MarkingMapping(queryVM) { Color = ColorExtension.RandomColorRgb() };
    }

    public MapperMemento CreateMemento() {
        return new MapperMemento(
            ImageFilePath,
            WorldspaceFormKey,
            LeftCell,
            RightCell,
            TopCell,
            BottomCell,
            MarkingSize,
            Mappings.Select(m => m.CreateMemento()).ToList());
    }

    public void RestoreMemento(MapperMemento memento) {
        Mappings.LoadOptimized(memento.MarkingMappings.Select(m => {
            var markingMapping = CreateMapping();
            markingMapping.RestoreMemento(m);
            return markingMapping;
        }));
        ImageFilePath = memento.ImagePath;
        WorldspaceFormKey = memento.Worldspace;
        LeftCell = memento.LeftCell;
        RightCell = memento.RightCell;
        TopCell = memento.TopCell;
        BottomCell = memento.BottomCell;
        MarkingSize = memento.MarkingSize;
    }
}
