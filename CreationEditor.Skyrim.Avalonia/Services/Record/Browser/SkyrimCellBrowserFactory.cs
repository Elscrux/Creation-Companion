using Autofac;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.Browser;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Skyrim.Avalonia.Views.Record.Browser;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser;

public sealed class SkyrimCellBrowserFactory : ICellBrowserFactory {
    private readonly ILifetimeScope _lifetimeScope;

    public SkyrimCellBrowserFactory(
        ILifetimeScope lifetimeScope) {
        _lifetimeScope = lifetimeScope;
    }

    public Control GetBrowser() {
        var newScope = _lifetimeScope.BeginLifetimeScope();
        var cellBrowserVM = newScope.Resolve<ICellBrowserVM>();
        newScope.DisposeWith(cellBrowserVM);

        return new CellBrowser(cellBrowserVM);
    }
}
