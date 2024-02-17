using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.Constants;
using CreationEditor.Avalonia.ViewModels.Mod;
namespace CreationEditor.Avalonia.Views.Mod;

public partial class ModCreation : ReactiveUserControl<ModCreationVM> {
    public static readonly StyledProperty<bool> ShowAddButtonProperty
        = AvaloniaProperty.Register<ModCreation, bool>(nameof(ShowAddButton), true);

    public bool ShowAddButton {
        get => GetValue(ShowAddButtonProperty);
        set => SetValue(ShowAddButtonProperty, value);
    }

    public ModCreation() {
        InitializeComponent();
    }

    public ModCreation(ModCreationVM viewModel) : this() {
        ViewModel = viewModel;
    }

    private async void AddButton_Click(object sender, RoutedEventArgs e) {
        if (ViewModel is null) return;

        Popup.IsOpen = false;
        Popup.Child = new Border {
            Child = new TextBlock {
                Text = ViewModel.NewModKey + " added",
                Margin = new Thickness(10)
            },
            Background = StandardBrushes.BackgroundBrush,
            CornerRadius = new CornerRadius(10),
            Margin = new Thickness(5),
        };
        Popup.IsLightDismissEnabled = false;
        Popup.IsOpen = true;

        await ViewModel.CreateModCommand.Execute();

        await Task.Delay(TimeSpan.FromSeconds(3));

        Popup.IsOpen = false;
        Popup.Child = null;
    }
}
