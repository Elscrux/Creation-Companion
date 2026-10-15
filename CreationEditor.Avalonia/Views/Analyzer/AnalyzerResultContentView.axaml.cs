using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using CreationEditor.Avalonia.ViewModels.Analyzer;
using ReactiveUI.Avalonia;
namespace CreationEditor.Avalonia.Views.Analyzer;

public partial class AnalyzerResultContentView : ReactiveUserControl<AnalyzerResultVM> {
    public AnalyzerResultContentView() {
        InitializeComponent();
    }

    private async void Copy_PointerPressed(object? sender, RoutedEventArgs e) {
        if (sender is not ContentControl control) return;

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is not { Clipboard: {} clipboard }) return;

        await clipboard.SetTextAsync(control.Content?.ToString());
    }

    private void InputElement_OnRightTapped(object? sender, TappedEventArgs e) {
        if (ViewModel is null) return;
        if (e.Source is not Control control) return;

        var contextFlyout = new MenuFlyout {
            ItemsSource = ViewModel.GetContextMenuItems(),
        };

        contextFlyout.ShowAt(control, true);

        e.Handled = true;
    }
}
