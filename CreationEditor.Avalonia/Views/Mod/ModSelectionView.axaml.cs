using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Avalonia.Views.Dialog;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
namespace CreationEditor.Avalonia.Views.Mod;

public partial class ModSelectionView : ReactiveUserControl<ModSelectionVM> {
    public static readonly StyledProperty<SaveDialog> DataSourceSelectionPopupProperty
        = AvaloniaProperty.Register<ModSelectionView, SaveDialog>(nameof(DataSourceSelectionPopup));

    public SaveDialog DataSourceSelectionPopup {
        get => GetValue(DataSourceSelectionPopupProperty);
        set => SetValue(DataSourceSelectionPopupProperty, value);
    }

    public ModSelectionView() {
        InitializeComponent();
    }

    public ModSelectionView(ModSelectionVM modSelectionVM) : this() {
        DataContext = modSelectionVM;
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        if (DataContext is not ModSelectionVM modSelectionVM) return;

        var window = this.FindLogicalAncestorOfType<Window>();
        if (window is null) return;

        if (modSelectionVM.MissingPluginsFile) {
            var messageBox = MessageBoxManager.GetMessageBoxStandard(
                "Warning",
                $"Make sure {modSelectionVM.PluginsFilePath} exists.",
                ButtonEnum.Ok,
                Icon.Warning,
                WindowStartupLocation.CenterOwner);

            messageBox.ShowWindowDialogAsync(window);
        }

        modSelectionVM.RefreshListings();
    }

    private void OpenDataSourceSelectionView(object? sender, RoutedEventArgs e) {
        DataSourceSelectionPopup.Show();
    }
}
