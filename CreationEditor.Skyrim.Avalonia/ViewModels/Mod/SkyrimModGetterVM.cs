using System.Collections.ObjectModel;
using System.Linq;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Mod;

public sealed class SkyrimModGetterVM : ViewModel, IModGetterVM<ISkyrimModGetter> {
    [Reactive] public bool IsReadOnly { get; set; } = true;

    [Reactive] public string Name { get; set; } = string.Empty;
    [Reactive] public ModType Type { get; set; }
    [Reactive] public string Author { get; set; } = string.Empty;
    [Reactive] public string Description { get; set; } = string.Empty;
    [Reactive] public bool Localization { get; set; }
    [Reactive] public int FormVersion { get; set; }
    public ObservableCollection<string> Masters { get; set; } = new();

    public void SetTo(IModGetter mod) {
        if (mod is ISkyrimModGetter skyrimMod) SetTo(skyrimMod);
    }

    public void SetTo(ISkyrimModGetter mod) {
        Name = mod.ModKey.Name;
        Type = mod.ModKey.Type;

        Author = mod.ModHeader.Author ?? string.Empty;
        Description = mod.ModHeader.Description ?? string.Empty;

        Localization = (mod.ModHeader.Flags & SkyrimModHeader.HeaderFlag.Localized) != 0;
        FormVersion = mod.ModHeader.FormVersion;

        Masters.Clear();
        Masters.AddRange(mod.ModHeader.MasterReferences.Select(master => master.Master.FileName.String));
    }
}
