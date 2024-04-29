using System.Collections;
using System.Reactive;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CreationEditor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Query;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Type;
using CreationEditor.Services.Query.From;
using CreationEditor.Skyrim.Avalonia.Resources.Constants;
using DynamicData.Binding;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;
namespace MapperPlugin.ViewModels;

public sealed class MarkingMapping(QueryVM queryVM) : ReactiveObject {
    [Reactive] public bool UseQuery { get; set; }
    public QueryVM QueryVM { get; set; } = queryVM;
    public IFormLinkGetter Record { get; set; } = FormLinkInformation.Null;
    public Color Color { get; set; }
}

public sealed class MapperVM : ViewModel {
    private readonly Func<QueryVM> _queryVMFactory;
    private readonly ILogger _logger;

    public ILinkCacheProvider LinkCacheProvider { get; }

    [Reactive] public IImage? ImageSource { get; set; }
    [Reactive] public int Resolution { get; set; } = 1;

    [Reactive] public int BottomLeftX { get; set; }
    [Reactive] public int BottomLeftY { get; set; }

    [Reactive] public int TopRightX { get; set; }
    [Reactive] public int TopRightY { get; set; }

    [Reactive] public IStorageFile? PickedImage { get; set; }
    public IReadOnlyList<FilePickerFileType> ImageFilter { get; } = [FilePickerFileTypes.ImageAll];
    public IObservableCollection<MarkingMapping> Mappings { get; }
    [Reactive] public int SelectedMappingIndex { get; set; }

    public ReactiveCommand<Unit, Unit> AddMapping { get; }
    public ReactiveCommand<IList, Unit> RemoveMapping { get; }

    private readonly IList<QueryFromItem> _queryFromItems;

    public MapperVM(
        Func<QueryVM> queryVMFactory,
        ILogger logger,
        IGameReleaseContext gameReleaseContext,
        IMutagenTypeProvider mutagenTypeProvider,
        ILinkCacheProvider linkCacheProvider) {
        _queryVMFactory = queryVMFactory;
        _logger = logger;
        LinkCacheProvider = linkCacheProvider;

        var registrationsByGetterType = mutagenTypeProvider
            .GetRegistrations(gameReleaseContext.Release)
            .ToDictionary(x => x.GetterType, x => x);

        _queryFromItems = RecordTypeConstants.PlaceableTypes
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

        this.WhenAnyValue(x => x.PickedImage)
            .Subscribe(image => {
                if (image is null) return;

                try {
                    ImageSource = new Bitmap(image.Path.LocalPath);
                } catch (Exception e) {
                    _logger.Here().Error("Failed to load image: {Exception}", e.Message);
                }
            })
            .DisposeWith(this);
    }

    public MarkingMapping CreateMapping() {
        var rnd = Random.Shared;
        var randomColor = Color.FromRgb((byte) rnd.Next(256), (byte) rnd.Next(256), (byte) rnd.Next(256));
        var queryVM = _queryVMFactory();
        queryVM.QueryRunner.QueryFrom.Items.ReplaceWith(_queryFromItems);
        return new MarkingMapping(queryVM) { Color = randomColor };
    }
}
