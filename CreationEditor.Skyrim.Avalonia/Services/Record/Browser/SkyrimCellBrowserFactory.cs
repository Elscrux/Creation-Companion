using System;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.Browser;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Skyrim.Avalonia.Views.Record.Browser;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser;

public sealed class SkyrimCellBrowserFactory(Func<ICellBrowserVM> cellBrowserVMFactory) : ICellBrowserFactory {
    public Control GetBrowser() {
        return new CellBrowser(cellBrowserVMFactory());
    }
}
