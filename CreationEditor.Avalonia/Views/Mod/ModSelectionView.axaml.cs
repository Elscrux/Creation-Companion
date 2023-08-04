using Avalonia.Controls.Presenters;
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

    public static void ShowAsContentDialog(ModSelectionVM modSelectionVM, bool allowLoading = true) {
        var contentDialog = new ContentDialog {
            Styles = {
                new Style(x => x
                    .OfType<ContentDialog>()
                    .Class(":fullsize")
                    .Template()
                    .OfType<FABorder>()
                    .Name("BackgroundElement")
                ) {
                    Setters = {
                        new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Stretch),
                        new Setter(MinWidthProperty, 750.0),
                    }
                },
            },
            DataContext = modSelectionVM,
            [!ContentDialog.IsPrimaryButtonEnabledProperty] = new Binding($"{nameof(ModSelectionVM.CanLoad)}^"),
            [ContentDialog.IsSecondaryButtonEnabledProperty] = allowLoading,
            Title = "Select Mods",
            Content = new ModSelectionView(modSelectionVM) {
                [!MaxHeightProperty] = new Binding("Bounds.Height") {
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor) {
                        AncestorType = typeof(ContentPresenter),
                        Tree = TreeType.Visual
                    },
                }
            },
            FullSizeDesired = true,
            PrimaryButtonText = "Load",
            PrimaryButtonCommand = ReactiveCommand.Create(modSelectionVM.LoadMods),
            SecondaryButtonText = "Cancel",
            KeyBindings = {
                new KeyBinding {
                    Command = ReactiveCommand.Create(() => {
                        if (allowLoading) modSelectionVM.LoadMods();
                    }),
                    Gesture = new KeyGesture(Key.Enter)
                }
            },
            DefaultButton = ContentDialogButton.Primary,
        };
        contentDialog.SecondaryButtonCommandParameter = ReactiveCommand.Create(() => contentDialog.Hide());

        contentDialog.ShowAsync();
    }
}
