using CreationEditor.Services.Mutagen.Mod;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Mod;

public sealed partial class SkyrimModGetterVM : ViewModel, IModGetterVM {
    [Reactive] public partial bool IsReadOnly { get; set; }

    [Reactive] public partial string Name { get; set; }
    [Reactive] public partial ModType Type { get; set; }
    [Reactive] public partial string Author { get; set; }
    [Reactive] public partial string Description { get; set; }
    [Reactive] public partial bool Localization { get; set; }
    [Reactive] public partial int FormVersion { get; set; }
    public IObservableCollection<ModKey> Masters { get; set; } = new ObservableCollectionExtended<ModKey>();

    public SkyrimModGetterVM() {
        IsReadOnly = true;
        Name = string.Empty;
        Author = string.Empty;
        Description = string.Empty;
    }

    public void SetTo(ModInfo modInfo) {
        Name = modInfo.ModKey.Name;
        Type = modInfo.ModKey.Type;

        Author = modInfo.Author ?? string.Empty;
        Description = modInfo.Description ?? string.Empty;

        Localization = modInfo.Localization;
        FormVersion = modInfo.FormVersion;

        Masters.LoadOptimized(modInfo.Masters);
    }
}
