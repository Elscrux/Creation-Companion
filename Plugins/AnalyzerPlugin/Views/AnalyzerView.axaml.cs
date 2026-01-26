using AnalyzerPlugin.ViewModels;
using Avalonia.Controls;

namespace AnalyzerPlugin.Views;

public partial class AnalyzerView : UserControl {
    public AnalyzerView() {
        InitializeComponent();
    }

    public AnalyzerView(AnalyzerVM vm) : this() {
        DataContext = vm;
    }
}

