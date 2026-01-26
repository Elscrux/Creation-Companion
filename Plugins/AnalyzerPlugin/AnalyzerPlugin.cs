using AnalyzerPlugin.ViewModels;
using AnalyzerPlugin.Views;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Plugin;
using Mutagen.Bethesda.Plugins.Records;
namespace AnalyzerPlugin;

public sealed class AnalyzerPlugin<TMod, TModGetter>(
    Func<AnalyzerVM> analyzerVM)
    : IMenuPlugin<TMod, TModGetter>
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    where TMod : class, TModGetter, IContextMod<TMod, TModGetter> {
    public string Name => "Analyzer";
    public string Description => "Analyzes mods to report problems";
    public Guid Guid => new("86bddebe-5335-4dd0-9783-5858a2d7ba9d");

    public Control GetControl() => new AnalyzerView(analyzerVM());
    public DockMode DockMode { get; set; } = DockMode.Document;
    public Dock Dock { get; set; }
    public object GetIcon() => new TextBlock { Text = "✅️️️" };
}
