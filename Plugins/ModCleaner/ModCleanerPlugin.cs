using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Plugin;
using CreationEditor.Services.Plugin;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Skyrim;
using ModCleaner.ViewModels;
using ModCleaner.Views;
namespace ModCleaner;

public sealed class ModCleanerPlugin(
    Func<ModCleanerVM> modCleanerVMFactory,
    PluginContext pluginContext)
    : IMenuPlugin<ISkyrimMod, ISkyrimModGetter> {

    public string Name => "Mod Cleaner";
    public string Description => "Clean a mod by removing records that are not referenced in itself or dependent mods";
    public Guid Guid => new("9a9dd271-2c7c-466b-9f24-a0d311a60f47");

    public Control GetControl() => new ModCleanerView(modCleanerVMFactory());

    public DockMode DockMode { get; set; } = DockMode.Document;
    public Dock Dock { get; set; }
    public object GetIcon() => new SymbolIcon { Symbol = Symbol.Filter };
}
