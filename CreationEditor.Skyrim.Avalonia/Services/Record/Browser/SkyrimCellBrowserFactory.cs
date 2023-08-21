using System;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.Browser;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Skyrim.Avalonia.Views.Record.Browser;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser;

public sealed class SkyrimCellBrowserFactory : ICellBrowserFactory {
    private readonly Func<ICellBrowserVM> _cellBrowserVMFactory;

    public SkyrimCellBrowserFactory(
        Func<ICellBrowserVM> cellBrowserVMFactory) {
        _cellBrowserVMFactory = cellBrowserVMFactory;
    }

    public Control GetBrowser() {
        return new CellBrowser(_cellBrowserVMFactory());
    }
}
