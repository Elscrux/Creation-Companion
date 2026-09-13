using ReactiveUI.Avalonia;
using ReleasePipeline.ViewModels;
namespace ReleasePipeline.Views;

public partial class BSAArchiveActionConfigView : ReactiveUserControl<BSAArchiveActionConfigVM> {
    public BSAArchiveActionConfigView() {
        InitializeComponent();
    }

    public BSAArchiveActionConfigView(BSAArchiveActionConfigVM vm) : this() {
        DataContext = vm;
    }
}
