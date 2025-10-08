using CreationEditor.Services.Mutagen.Mod;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Mod;

public interface IModGetterVM {
    [Reactive] bool IsReadOnly { get; set; }

    [Reactive] string Name { get; set; }
    [Reactive] ModType Type { get; set; }
    [Reactive] string Author { get; set; }
    [Reactive] string Description { get; set; }
    [Reactive] bool Localization { get; set; }
    [Reactive] int FormVersion { get; set; }
    IObservableCollection<ModKey> Masters { get; set; }

    void SetTo(ModInfo modInfo);
}
