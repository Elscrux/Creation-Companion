using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Plugin;
using FluentAvalonia.UI.Controls;
using MapperPlugin.ViewModels;
using MapperPlugin.Views;
using Mutagen.Bethesda.Skyrim;
namespace MapperPlugin;

public sealed class MapperPlugin(
    Func<MapperVM> mapperVM)
    : IMenuPlugin<ISkyrimMod, ISkyrimModGetter> {
    public string Name => "Mapper";
    public string Description => "Creates maps.";
    public Guid Guid => new("f75e5cda-7bdd-42c2-b777-eac180442416");

    public Control GetControl() => new MapperView(mapperVM());
    public DockMode DockMode { get; set; } = DockMode.Document;
    public Dock Dock { get; set; }
    public object GetIcon() => new FontIcon { Glyph = "🗺️️" };
}
