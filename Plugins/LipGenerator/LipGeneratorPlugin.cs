using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Plugin;
using LipGenerator.ViewModels;
using LipGenerator.Views;
using Mutagen.Bethesda.Skyrim;
namespace LipGenerator;

public sealed class LipGeneratorPlugin(
    Func<LipGeneratorVM> lipGeneratorVM)
    : IMenuPlugin<ISkyrimMod, ISkyrimModGetter> {
    public string Name => "Lip File Generator";
    public string Description => "Generate .lip and .fuz files from .wav voice files for Skyrim mods.";
    public Guid Guid => new("f8f8616a-993d-4512-838b-c08108450c3f");

    public Control GetControl() => new LipGeneratorView(lipGeneratorVM());
    public DockMode DockMode { get; set; } = DockMode.Side;
    public Dock Dock { get; set; } = Dock.Left;
    public double? Size { get; set; } = 700;
    public object GetIcon() => new TextBlock { Text = "👄" };
}
