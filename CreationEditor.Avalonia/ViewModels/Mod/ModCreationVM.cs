using System.IO.Abstractions;
using System.Reactive;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ReactiveUI.Validation.Extensions;
using Serilog;
namespace CreationEditor.Avalonia.ViewModels.Mod;

public sealed partial class ModCreationVM : ValidatableViewModel {
    public ILogger Logger { get; }
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IDataSourceService _dataSourceService;
    private readonly IFileSystem _fileSystem;

    private const string WatermarkBase = "NewMod";
    private static string WatermarkName(int index) => $"{WatermarkBase} ({index})";
    [Reactive] public partial string ModNameWatermark { get; set; }

    [Reactive] public partial string? NewModName { get; set; }
    [Reactive] public partial ModType NewModType { get; set; }

    public string ModNameOrBackup => NewModName ?? ModNameWatermark;
    public ModKey? NewModKey => new ModKey(ModNameOrBackup, NewModType);

    public ReactiveCommand<Unit, IMod> CreateModCommand { get; }

    public ModCreationVM(
        IEditorEnvironment editorEnvironment,
        IDataSourceService dataSourceService,
        ILogger logger,
        IFileSystem fileSystem) {
        Logger = logger;
        _editorEnvironment = editorEnvironment;
        _dataSourceService = dataSourceService;
        _fileSystem = fileSystem;

        NewModType = ModType.Plugin;

        ModNameWatermark = GetNewWatermark(_editorEnvironment.LinkCache.ListedOrder.Select(x => x.ModKey).ToList());
        editorEnvironment.LoadOrderChanged
            .Subscribe(loadOrder => ModNameWatermark = GetNewWatermark(loadOrder))
            .DisposeWith(this);

        CreateModCommand = ReactiveCommand.Create(() => {
                var modKey = string.IsNullOrWhiteSpace(NewModName)
                    ? new ModKey(ModNameWatermark, NewModType)
                    : new ModKey(NewModName, NewModType);
                NewModName = null;
                return editorEnvironment.AddNewMutableMod(modKey);
            },
            this.IsValid());

        this.ValidationRule(
            x => x.NewModName,
            NameIsFree,
            "Name is already taken");

        this.ValidationRule(
            x => x.NewModName,
            name => string.IsNullOrWhiteSpace(name) || name.IndexOfAny(_fileSystem.Path.GetInvalidFileNameChars()) == -1,
            name => {
                var index = name!.IndexOfAny(_fileSystem.Path.GetInvalidFileNameChars());
                return $"Name contains invalid character {name[index]}";
            });
    }

    private string GetNewWatermark(IReadOnlyList<ModKey> loadOrder) {
        // Assign new placeholder text if the name is already taken
        var Watermark = WatermarkBase;
        if (NameIsFree(Watermark, loadOrder)) return Watermark;

        var counter = 2;
        while (!NameIsFree(Watermark, loadOrder)) {
            Watermark = WatermarkName(counter);
            counter++;
        }

        return Watermark;
    }

    private bool NameIsFree(string? modName, IEnumerable<ModKey> loadOrder) {
        if (string.IsNullOrWhiteSpace(modName)) return true;
        if (loadOrder.Any(modKey => string.Equals(modKey.Name, modName, StringComparison.OrdinalIgnoreCase))) return false;

        return !ModExistsOnDisk(modName);
    }

    private bool NameIsFree(string? modName) => NameIsFree(modName, _editorEnvironment.LinkCache.ListedOrder.Select(mod => mod.ModKey));

    private bool ModExistsOnDisk(string name) {
        if (name.IndexOfAny(_fileSystem.Path.GetInvalidFileNameChars()) != -1) return false;

        return Enum.GetValues<ModType>()
            .Exists(modType => {
                var fileName = new ModKey(name, modType).FileName;
                var link = _dataSourceService.GetFileLink(new DataRelativePath(fileName));
                return link is not null && link.Exists();
            });
    }
}
