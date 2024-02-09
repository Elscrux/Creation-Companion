using System.Reactive.Linq;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Mod;
namespace CreationEditor.Avalonia.Views.Mod;

public partial class ModCreation : ReactiveUserControl<ModCreationVM> {
    public static readonly StyledProperty<bool> ShowButtonProperty
        = AvaloniaProperty.Register<ModCreation, bool>(nameof(ShowButton));

    public bool ShowButton {
        get => GetValue(ShowButtonProperty);
        set => SetValue(ShowButtonProperty, value);
    }

    public ModCreation() {
        InitializeComponent();
    }

    public ModCreation(ModCreationVM viewModel) : this() {
        ViewModel = viewModel;
    }

    private async void AddButton_Click(object sender, RoutedEventArgs e) {
        if (ViewModel is null) return;

        StatusMessage.Text = ViewModel.NewModKey + " added";
        StatusMessage.IsVisible = true;

        await ViewModel.CreateModCommand.Execute();

        await Task.Delay(TimeSpan.FromSeconds(3));

        StatusMessage.IsVisible = false;
        StatusMessage.Text = string.Empty;
    }
}
