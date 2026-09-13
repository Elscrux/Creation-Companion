using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Plugin;
using CreationEditor.Services.Plugin;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Skyrim;
using ReleasePipeline.ViewModels;
using ReleasePipeline.Views;
namespace ReleasePipeline;

public sealed class ReleasePipelinePlugin(
    Func<ReleasePipelineWizardVM> releasePipelineFactory,
    PluginContext pluginContext)
    : IMenuPlugin<ISkyrimMod, ISkyrimModGetter> {

    public string Name => "Release Pipeline";
    public string Description => "Prepare a mod for release: run pre-release checks and a pipeline of configurable actions.";
    public Guid Guid => new("975ac16e-2b0c-425f-a69e-d57836ebe740");

    public Control GetControl() => new ReleasePipelineView(releasePipelineFactory());

    public DockMode DockMode { get; set; } = DockMode.Document;
    public Dock Dock { get; set; }
    public double? Size { get; set; }
    public object GetIcon() => new FAFontIcon { Glyph = "🚀" };
}
