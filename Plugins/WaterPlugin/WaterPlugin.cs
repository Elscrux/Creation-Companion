using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Plugin;
using FluentAvalonia.UI.Controls;
using WaterPlugin.ViewModels;
using Mutagen.Bethesda.Plugins.Records;
using WaterPlugin.Views;
namespace WaterPlugin;

public sealed class WaterPlugin<TMod, TModGetter>(
    Func<WaterMapVM> waterMapVMFactory)
    : IMenuPlugin<TMod, TModGetter>
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    where TMod : class, TModGetter, IContextMod<TMod, TModGetter> {

    public string Name => "Water Gradient Generator";
    public string Description => "Generate gradients of water in a worldspace that seamlessly blend into each other.";
    public Guid Guid => new("254dd5c7-c942-43ec-8b1d-5c3e8d8baec9");
    public Control GetControl() => new WaterMapView(waterMapVMFactory());

    public DockMode DockMode { get; set; } = DockMode.Side;
    public Dock Dock { get; set; } = Dock.Left;
    public object GetIcon() => new SymbolIcon { Symbol = Symbol.Globe };
}
