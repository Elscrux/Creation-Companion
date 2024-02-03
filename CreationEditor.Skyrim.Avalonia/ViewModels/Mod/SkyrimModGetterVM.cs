using System.Collections.Generic;
using System.Linq;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
using DynamicData.Binding;
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
    public IObservableCollection<ModKey> Masters { get; set; } = new ObservableCollectionExtended<ModKey>();

    [Reactive] public ModInfo? ActiveModInfo { get; private set; }

    public IEnumerable<ModInfo> GetModInfos(IEnumerable<IModGetter> mods) => GetModInfos(mods.OfType<ISkyrimModGetter>());
    public IEnumerable<ModInfo> GetModInfos(IEnumerable<ISkyrimModGetter> mods) {
        return mods
            .Select(mod => new ModInfo(
                mod.ModKey,
                mod.ModHeader.Author,
                mod.ModHeader.Description,
                (mod.ModHeader.Flags & SkyrimModHeader.HeaderFlag.Localized) != 0,
                mod.ModHeader.FormVersion,
                mod.ModHeader.MasterReferences.Select(master => master.Master).ToArray()));
    }

    public void SetTo(ModInfo modInfo) {
        Name = modInfo.ModKey.Name;
        Type = modInfo.ModKey.Type;

        Author = modInfo.Author ?? string.Empty;
        Description = modInfo.Description ?? string.Empty;

        Localization = modInfo.Localization;
        FormVersion = modInfo.FormVersion;

        Masters.ReplaceWith(modInfo.Masters);
    }
}
