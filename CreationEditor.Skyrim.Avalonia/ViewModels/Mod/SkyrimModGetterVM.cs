using System.Collections.Generic;
using System.Linq;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Mod;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
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

    public ModInfo GetModInfo(ModKey modKey, ISkyrimModHeaderGetter modHeader) {
        return new ModInfo(
            modKey,
            modHeader.Author,
            modHeader.Description,
            (modHeader.Flags & SkyrimModHeader.HeaderFlag.Localized) != 0,
            modHeader.FormVersion,
            modHeader.MasterReferences.Select(master => master.Master).ToArray());
    }
    public ModInfo GetModInfo(ModKey modKey, MutagenFrame modHeaderFrame) {
        return GetModInfo(modKey, SkyrimModHeader.CreateFromBinary(modHeaderFrame));
    }
    public ModInfo? GetModInfo(IModGetter mod) => mod is ISkyrimModGetter skyrimMod ? GetModInfo(skyrimMod.ModKey, skyrimMod.ModHeader) : null;
    public ModInfo GetModInfo(ISkyrimModGetter mod) => GetModInfo(mod.ModKey, mod.ModHeader);
    public IEnumerable<ModInfo> GetModInfos(IEnumerable<IModGetter> mods) => GetModInfos(mods.OfType<ISkyrimModGetter>());
    public IEnumerable<ModInfo> GetModInfos(IEnumerable<ISkyrimModGetter> mods) {
        return mods.Select(mod => GetModInfo(mod.ModKey, mod.ModHeader));
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
