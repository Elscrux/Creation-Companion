using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Plugin;
using CreationEditor.Services.Plugin;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Skyrim;
using VanillaDuplicateCleaner.ViewModels;
using VanillaDuplicateCleaner.Views;
namespace VanillaDuplicateCleaner;

public sealed class VanillaDuplicateCleanerPlugin(
    Func<VanillaDuplicateCleanerVM> vanillaDuplicateCleanerVMFactory,
    PluginContext pluginContext)
    : IMenuPlugin<ISkyrimMod, ISkyrimModGetter> {

    private readonly PluginContext _pluginContext = pluginContext;

    public string Name => "Vanilla Duplicate Cleaner";
    public string Description => "Cleans mods from vanilla duplicates.";
    public Guid Guid => new("2179f861-1934-41e7-b612-b80484542c2c");

    public Control GetControl() => new VanillaDuplicateCleanerView(vanillaDuplicateCleanerVMFactory());

    public DockMode DockMode { get; set; } = DockMode.Side;
    public Dock Dock { get; set; } = Dock.Left;
    public object GetIcon() => new FontIcon { Glyph = "" };
}
