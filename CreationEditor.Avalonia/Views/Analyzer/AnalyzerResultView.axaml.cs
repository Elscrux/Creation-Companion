using CreationEditor.Avalonia.ViewModels.Asset.Browser;
using ReactiveUI.Avalonia;
namespace CreationEditor.Avalonia.Views.Analyzer;

public partial class AnalyzerResultView : ReactiveUserControl<IAssetBrowserVM> {
    public AnalyzerResultView() {
        InitializeComponent();
    }
}
