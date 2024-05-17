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
using CreationEditor.Services.Mutagen.References.Record.Controller;
using CreationEditor.Services.Mutagen.Type;
using CreationEditor.Services.Query.From;
using CreationEditor.Services.State;
using CreationEditor.Skyrim.Avalonia.Resources.Constants;
using DynamicData.Binding;
using MapperPlugin.Model;
using MapperPlugin.Services;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
namespace MapperPlugin.ViewModels;

public sealed class MapperVM : ViewModel, IMementoProvider<MapperMemento> {
    private readonly Func<QueryVM> _queryVMFactory;
    private readonly IList<QueryFromItem> _placeableQueryFromItems;

    public HeatmapCreator HeatmapCreator { get; }

    [Reactive] public int BusyTasks { get; set; }

    public ILinkCacheProvider LinkCacheProvider { get; }

    [Reactive] public FormKey WorldspaceFormKey { get; set; }

    [Reactive] public int TopCell { get; set; } = 64;
    [Reactive] public int BottomCell { get; set; } = -64;
    [Reactive] public int LeftCell { get; set; } = -64;
    [Reactive] public int RightCell { get; set; } = 64;

    [Reactive] public int MarkingSize { get; set; } = 10;

    [Reactive] public DrawingImage? DrawingsImage { get; set; }
    [Reactive] public IImage? ImageSource { get; set; }
    [Reactive] public string? ImageFilePath { get; set; }

    public IObservableCollection<MarkingMapping> Mappings { get; }

    public ReactiveCommand<Unit, Unit> AddMapping { get; }
    public ReactiveCommand<IList, Unit> RemoveMapping { get; }
    public ReactiveCommand<string, Unit> SaveMap { get; }
    public ReactiveCommand<Guid, Unit> LoadMap { get; }
    public ReactiveCommand<StateIdentifier, Unit> DeleteMap { get; }
    public IObservableCollection<StateIdentifier> SavedMaps { get; } = new ObservableCollectionExtended<StateIdentifier>();

    public MapperVM(
        Func<QueryVM> queryVMFactory,
        Func<string, IStateRepository<MapperMemento>> stateRepositoryFactory,
        ILogger logger,
        IGameReleaseContext gameReleaseContext,
        IRecordReferenceController recordReferenceController,
        IMutagenTypeProvider mutagenTypeProvider,
        ILinkCacheProvider linkCacheProvider) {
        _queryVMFactory = queryVMFactory;
        HeatmapCreator = new HeatmapCreator();
        LinkCacheProvider = linkCacheProvider;
        var stateRepository = stateRepositoryFactory("Heatmap");
        SavedMaps.AddRange(stateRepository.LoadAllStateIdentifiers());

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
            stateRepository.Save(memento, newGuid, name);
            foreach (var removeMap in SavedMaps.Where(x => x.Name == name).ToList()) {
                SavedMaps.Remove(removeMap);
                stateRepository.Delete(removeMap.Id, removeMap.Name);
            }
            SavedMaps.Add(new StateIdentifier(newGuid, name));
        });

        LoadMap = ReactiveCommand.Create<Guid>(id => {
            var memento = stateRepository.Load(id);
            if (memento is not null) {
                RestoreMemento(memento);
            }
        });

        DeleteMap = ReactiveCommand.Create<StateIdentifier>(id => {
            stateRepository.Delete(id.Id, id.Name);
            SavedMaps.RemoveWhere(x => x.Id == id.Id);
        });

        LinkCacheProvider.LinkCacheChanged
            .Subscribe(_ => HeatmapCreator.ClearCache());

        // Logical update
        var logicalMappingUpdates = Mappings
            .WhenCollectionChanges()
            .Select(_ => Mappings.Select(m => m.LogicalUpdates).CombineLatest())
            .Switch();

        this.WhenAnyValue(x => x.WorldspaceFormKey)
            .Where(worldspace => !worldspace.IsNull)
            .CombineLatest(logicalMappingUpdates, (x, _) => x)
            .ThrottleMedium()
            .ObserveOnGui()
            .Do(_ => BusyTasks++)
            .ObserveOnTaskpool()
            .DoTask(async worldspace => await HeatmapCreator.CalculateSpots(Mappings, LinkCacheProvider.LinkCache, recordReferenceController, worldspace))
            .ObserveOnGui()
            .Do(_ => DrawingsImage = HeatmapCreator.GetDrawing(ImageSource!.Size, MarkingSize, LeftCell, RightCell, TopCell, BottomCell))
            .Do(_ => BusyTasks--)
            .Subscribe()
            .DisposeWith(this);

        // Visual  update
        var visualMappingUpdates = Mappings
            .WhenCollectionChanges()
            .Select(_ => Mappings.Select(m => m.VisualUpdates).CombineLatest())
            .Switch();

        this.WhenAnyValue(
                x => x.ImageSource,
                x => x.LeftCell,
                x => x.TopCell,
                x => x.RightCell,
                x => x.BottomCell,
                x => x.MarkingSize)
            .Where(x => x.Item1 is not null)
            .CombineLatest(visualMappingUpdates)
            .ThrottleMedium()
            .ObserveOnGui()
            .Do(_ => BusyTasks++)
            .Do(_ => DrawingsImage = HeatmapCreator.GetDrawing(ImageSource!.Size, MarkingSize, LeftCell, RightCell, TopCell, BottomCell))
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
                    logger.Here().Error("Failed to load image: {Exception}", e.Message);
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
        Mappings.ReplaceWith(memento.MarkingMappings.Select(m => {
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
