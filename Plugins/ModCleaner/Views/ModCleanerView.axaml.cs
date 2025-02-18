using Avalonia.ReactiveUI;
using BSAssetsTrimmer.ViewModels;
namespace BSAssetsTrimmer.Views;

public partial class BSAssetsTrimmerView : ReactiveUserControl<BSAssetsTrimmerVM> {
    public BSAssetsTrimmerView() {
        InitializeComponent();
    }

    public BSAssetsTrimmerView(BSAssetsTrimmerVM vm) : this() {
        DataContext = vm;
    }
}
