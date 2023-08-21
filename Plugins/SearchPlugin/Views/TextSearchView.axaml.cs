using Avalonia.Controls;
namespace SearchPlugin.Views;

public partial class TextSearchView : UserControl {
    public TextSearchView() {
        InitializeComponent();
    }

    public TextSearchView(object? vm) : this() {
        DataContext = vm;
    }
}
