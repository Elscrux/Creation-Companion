using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Windowing;
namespace CreationEditor.WPF.Views;

public partial class MainWindow : AppWindow, IMainWindow {
    public DockPanel? DockPanel => this.FindControl<DockPanel>("DockPanel");

    public MainWindow() {
        InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
#if DEBUG
        this.AttachDevTools();
#endif
    }
}
