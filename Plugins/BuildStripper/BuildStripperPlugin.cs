using Avalonia.Controls;
using BuildStripper.ViewModels;
using BuildStripper.Views;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Plugin;
using CreationEditor.Services.Plugin;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Skyrim;
namespace BuildStripper;

public sealed class BuildStripperPlugin(
    Func<BuildStripperVM> buildStripperFactory,
    PluginContext pluginContext)
    : IMenuPlugin<ISkyrimMod, ISkyrimModGetter> {

    public string Name => "Build Stripper";
    public string Description => "Strip a mod from unused records and assets that are not referenced in itself or dependent mods";
    public Guid Guid => new("9a9dd271-2c7c-466b-9f24-a0d311a60f47");

    public Control GetControl() => new BuildStripperView(buildStripperFactory());

    public DockMode DockMode { get; set; } = DockMode.Document;
    public Dock Dock { get; set; }
    public object GetIcon() => new FAFontIcon { Glyph = "🧹" };
}
