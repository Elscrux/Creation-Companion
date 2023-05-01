using Autofac;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.Browser;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Skyrim.Avalonia.Views.Record.Browser;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser;

public sealed class SkyrimCellBrowserFactory : ICellBrowserFactory {
    private readonly IComponentContext _componentContext;

    public SkyrimCellBrowserFactory(
        IComponentContext componentContext) {
        _componentContext = componentContext;
    }

    public Control GetBrowser() {
        return new CellBrowser(_componentContext.Resolve<ICellBrowserVM>());
    }
}
