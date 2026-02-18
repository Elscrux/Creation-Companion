using AnalyzerPlugin.ViewModels;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;

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
}
