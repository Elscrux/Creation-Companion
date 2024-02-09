using System.Reactive;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order.DI;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
namespace CreationEditor.Avalonia.ViewModels.Mod;

public sealed class ModCreationVM : ViewModel, IValidatableViewModel {
    public ValidationContext ValidationContext { get; } = new();

    private readonly IListingsProvider _listingsProvider;

    private const string WatermarkBase = "NewMod";
    private static string WatermarkName(int index) => $"{WatermarkBase} ({index})";
    [Reactive] public string ModNameWatermark { get; set; } = WatermarkBase;

    [Reactive] public string? NewModName { get; set; }
    [Reactive] public ModType NewModType { get; set; } = ModType.Plugin;

    public string ModNameOrBackup => NewModName ?? ModNameWatermark;
    public ModKey? NewModKey => new ModKey(ModNameOrBackup, NewModType);

    public ReactiveCommand<Unit, Unit> CreateModCommand { get; }

    public ModCreationVM(IEditorEnvironment editorEnvironment, ILoadOrderListingsProvider listingsProvider) {
        _listingsProvider = listingsProvider;

        editorEnvironment
            .LoadOrderChanged
            .Subscribe(loadOrder => {
                // Assign new watermark if the name is already taken
                if (NameIsFree(loadOrder, ModNameOrBackup)) return;

                var counter = 2;
                ModNameWatermark = WatermarkName(counter);
                for (var i = 0; i < loadOrder.Count; i++) {
                    if (!string.Equals(loadOrder[i].Name, ModNameWatermark, StringComparison.OrdinalIgnoreCase)) continue;

                    counter++;
                    ModNameWatermark = WatermarkName(counter);
                    i = 0;
                }
            })
            .DisposeWith(this);

        CreateModCommand = ReactiveCommand.Create(() => {
            var modKey = string.IsNullOrWhiteSpace(NewModName) ? new ModKey(ModNameWatermark, NewModType) : new ModKey(NewModName, NewModType);
            NewModName = null;
            editorEnvironment.AddNewMutableMod(modKey);
        }, this.IsValid());

        this.ValidationRule(
            x => x.NewModName,
            name => NameIsFree(editorEnvironment.GameEnvironment.LinkCache.ListedOrder.Select(x => x.ModKey), name),
            "Name is already taken");
    }

    private bool NameIsFree(IEnumerable<ModKey> loadOrder, string? name) {
        if (string.IsNullOrWhiteSpace(name)) return true;

        // Check if the name is already taken in the load order or the listings in the data directory
        return loadOrder.All(modKey => !string.Equals(modKey.Name, name, StringComparison.OrdinalIgnoreCase))
         && _listingsProvider.Get().Select(x => x.ModKey).All(modKey => !string.Equals(modKey.Name, name, StringComparison.OrdinalIgnoreCase));
    }
}
