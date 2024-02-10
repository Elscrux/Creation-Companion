using CreationEditor.Avalonia.Models.Mod;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI.Fody.Helpers;
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
    IEnumerable<ModInfo> GetModInfos(IEnumerable<IModGetter> mods);
    ModInfo GetModInfo(ModKey modKey, MutagenFrame modHeaderFrame);
    ModInfo? GetModInfo(IModGetter mod);
}

public interface IModGetterVM<in TModGetter> : IModGetterVM
    where TModGetter : class, IModGetter {
    IEnumerable<ModInfo> GetModInfos(IEnumerable<TModGetter> mods);
}
