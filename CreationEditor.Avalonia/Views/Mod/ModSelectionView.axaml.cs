﻿using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Mod;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
namespace CreationEditor.Avalonia.Views.Mod;

public partial class ModSelectionView : ReactiveUserControl<ModSelectionVM> {
    public ModSelectionView() {
        InitializeComponent();
    }

    public ModSelectionView(ModSelectionVM modSelectionVM) : this() {
        DataContext = modSelectionVM;
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        if (DataContext is ModSelectionVM modSelectionVM) {
            if (modSelectionVM.MissingPluginsFile) {
                var messageBox = MessageBoxManager.GetMessageBoxStandard(
                    "Warning",
                    $"Make sure {modSelectionVM.PluginsFilePath} exists.",
                    ButtonEnum.Ok,
                    Icon.Warning,
                    WindowStartupLocation.CenterOwner);

                messageBox.ShowWindowDialogAsync(this.FindLogicalAncestorOfType<Window>());
            }

            modSelectionVM.RefreshListings();
        }
    }
}
