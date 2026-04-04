using CreationEditor.Services.Mutagen.Mod;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Avalonia.ViewModels.Mod;

public interface IModGetterVM {
    bool IsReadOnly { get; set; }

    string Name { get; set; }
    ModType Type { get; set; }
    string Author { get; set; }
    string Description { get; set; }
    bool Localization { get; set; }
    int FormVersion { get; set; }
    IObservableCollection<ModKey> Masters { get; set; }

    void SetTo(ModInfo modInfo);
}
