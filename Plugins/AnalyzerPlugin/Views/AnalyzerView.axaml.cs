using AnalyzerPlugin.ViewModels;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Mutagen.Bethesda.Analyzers.Reporting.Handlers;
using ReactiveUI.Avalonia;
namespace AnalyzerPlugin.Views;

public partial class AnalyzerView : ReactiveUserControl<AnalyzerVM> {
    public AnalyzerView() {
        InitializeComponent();
    }

    public AnalyzerView(AnalyzerVM vm) : this() {
        DataContext = ViewModel = vm;
    }

    public async Task SetupFilePickerFunction() {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel?.StorageProvider is null) return;

        var filePickerOptions = new FilePickerSaveOptions {
            Title = "Export Topics as CSV",
            SuggestedFileName = "",
            DefaultExtension = ".csv",
            FileTypeChoices = [
                new FilePickerFileType("CSV Files") { Patterns = ["*.csv"] },
            ]
        };

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(filePickerOptions);

        var path = file?.Path.LocalPath;
        if (string.IsNullOrEmpty(path)) return;

        ViewModel?.ExportCsv(path);
    }

    private void ResultsTree_OnContextRequested(object? sender, ContextRequestedEventArgs e) {
        if (e.Source is not Control { DataContext: AnalyzerResult result } control) return;

        var contextFlyout = new MenuFlyout {
            ItemsSource = ViewModel?.GetContextMenuItems(result),
        };

        contextFlyout.ShowAt(control, true);
    }
}
