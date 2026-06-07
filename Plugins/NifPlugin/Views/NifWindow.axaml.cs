using FluentAvalonia.UI.Windowing;
namespace NifPlugin.Views;

public partial class NifWindow : FAAppWindow {
    public NifWindow() {
        InitializeComponent();

        TitleBar.ExtendsContentIntoTitleBar = false;
        TitleBar.ShowFullScreenButton = true;
    }
}
