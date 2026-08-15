using Avalonia.Controls;
using LipGenerator.ViewModels;
namespace LipGenerator.Views;

public sealed partial class LipGeneratorView : UserControl {
    public LipGeneratorView() {
        InitializeComponent();
    }

    public LipGeneratorView(LipGeneratorVM viewModel) {
        InitializeComponent();
        DataContext = viewModel;
    }
}
