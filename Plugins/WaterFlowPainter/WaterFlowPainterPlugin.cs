using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Plugin;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Skyrim;
using WaterFlowPainter.ViewModels;
using WaterFlowPainter.Views;
namespace WaterFlowPainter;

public sealed class WaterFlowPainterPlugin(Func<WaterFlowPainterVM> waterFlowPainterVMFactory)
    : IMenuPlugin<ISkyrimMod, ISkyrimModGetter> {
    public string Name => "Water Flow Painter";
    public string Description => "View and paint water flowmaps of cells with flowmap enabled water.";
    public Guid Guid => new("a3f8c2d1-6b4e-4f9a-8c7d-2e5b1a9f4d38");
    public Control GetControl() => new WaterFlowPainterView(waterFlowPainterVMFactory());

    public DockMode DockMode { get; set; } = DockMode.Document;
    public object Icon => new FAFontIconSource { Glyph = "💧" };
}
