using System.Collections.ObjectModel;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Mod;

public interface IModGetterVM {
    [Reactive] public bool IsReadOnly { get; set; }

    [Reactive] public string Name { get; set; }
    [Reactive] public ModType Type { get; set; }
    [Reactive] public string Author { get; set; }
    [Reactive] public string Description { get; set; }
    [Reactive] public bool Localization { get; set; }
    [Reactive] public int FormVersion { get; set; }
    public ObservableCollection<string> Masters { get; set; }

    public void SetTo(IModGetter mod);
}

public interface IModGetterVM<in TModGetter> : IModGetterVM
    where TModGetter : class, IModGetter {
    public void SetTo(TModGetter mod);
}
