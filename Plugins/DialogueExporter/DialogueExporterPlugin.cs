using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Plugin;
using DialogueExporter.ViewModels;
using DialogueExporter.Views;
using Mutagen.Bethesda.Skyrim;
namespace DialogueExporter;

public sealed class DialogueExporterPlugin(
    Func<DialogueExporterVM> dialogueExporterVM)
    : IMenuPlugin<ISkyrimMod, ISkyrimModGetter> {
    public string Name => "Dialogue Exporter";
    public string Description => "Creates leveled lists based on item tiers and list configurations.";
    public Guid Guid => new("3a4f9caf-cc84-4259-9c97-f1eee38afeb7");

    public Control GetControl() => new DialogueExporterView(dialogueExporterVM());
    public DockMode DockMode { get; set; } = DockMode.Side;
    public Dock Dock { get; set; } = Dock.Left;
    public object GetIcon() => new TextBlock { Text = "💬️" };
}
