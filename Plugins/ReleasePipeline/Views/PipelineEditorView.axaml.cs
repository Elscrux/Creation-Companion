using ReactiveUI.Avalonia;
using ReleasePipeline.ViewModels;
namespace ReleasePipeline.Views;

public partial class PipelineEditorView : ReactiveUserControl<PipelineEditorVM> {
    public PipelineEditorView() {
        InitializeComponent();
    }

    public PipelineEditorView(PipelineEditorVM vm) : this() => DataContext = vm;
}
