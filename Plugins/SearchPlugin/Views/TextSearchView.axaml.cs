using Avalonia.ReactiveUI;
using SearchPlugin.ViewModels;
namespace SearchPlugin.Views;

public partial class TextSearchView : ReactiveUserControl<TextSearchVM> {
    public TextSearchView() {
        InitializeComponent();
    }

    public TextSearchView(TextSearchVM textSearchVM) : this() {
        ViewModel = textSearchVM;
    }
}
