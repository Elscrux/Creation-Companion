using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Plugin;
using LeveledList.ViewModels;
using LeveledList.Views;
using Mutagen.Bethesda.Skyrim;
namespace LeveledList;

public sealed class LeveledListPlugin(
    Func<LeveledListVM> leveledListVM)
    : IMenuPlugin<ISkyrimMod, ISkyrimModGetter> {
    public string Name => "Leveled List Generator";
    public string Description => "Creates leveled lists based on item tiers and list configurations.";
    public Guid Guid => new("f75e5cda-7bdd-42c2-b777-eac180442416");

    public Control GetControl() => new LeveledListView(leveledListVM());
    public DockMode DockMode { get; set; } = DockMode.Document;
    public Dock Dock { get; set; }
    public object GetIcon() => new TextBlock { Text = "🎚️️️" };
}
