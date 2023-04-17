using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.ReactiveUI;
using Avalonia.Styling;
using CreationEditor.Avalonia.ViewModels.Mod;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views.Mod;

public partial class ModSelectionView : ReactiveUserControl<ModSelectionVM> {
    public ModSelectionView() {
        InitializeComponent();
    }

    public ModSelectionView(ModSelectionVM modSelectionVM) : this() {
        DataContext = modSelectionVM;
    }

    public static void ShowAsContentDialog(ModSelectionVM modSelectionVM) {
        var contentDialog = new ContentDialog {
            Styles = {
                new Style(x => x
                    .OfType<ContentDialog>()
                    .Class(":fullsize")
                    .Template()
                    .OfType<Border>()
                    .Name("BackgroundElement")
                ) {
                    Setters = {
                        new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Stretch),
                        new Setter(MinWidthProperty, 750.0),
                    }
                },
            },
            DataContext = modSelectionVM,
            [!ContentDialog.IsPrimaryButtonEnabledProperty] = new Binding($"{nameof(ModSelectionVM.AnyModsLoaded)}^"),
            Title = "Select Mods",
            Content = new ModSelectionView(modSelectionVM),
            FullSizeDesired = true,
            PrimaryButtonText = "Load",
            PrimaryButtonCommand = ReactiveCommand.Create(modSelectionVM.LoadMods),
            CloseButtonText = "Cancel",
            KeyBindings = { new KeyBinding { Command = ReactiveCommand.Create(modSelectionVM.LoadMods), Gesture = new KeyGesture(Key.Escape) } },
            DefaultButton = ContentDialogButton.Primary,
        };
        contentDialog.ShowAsync();
    }
}
