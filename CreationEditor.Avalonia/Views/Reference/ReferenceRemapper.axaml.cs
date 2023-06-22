using System.Reactive.Disposables;
using Avalonia;
using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Reference;
using CreationEditor.Avalonia.Views.Record.Picker;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views.Reference;

public partial class ReferenceRemapper : ReactiveUserControl<ReferenceRemapperVM> {
    public ReferenceRemapper() {
        InitializeComponent();

        this.WhenActivated(disposables => {
            this.WhenAnyValue(x => x.ViewModel)
                .Subscribe(vm => {
                    vm?.ShowReferenceRemapDialog
                        .Subscribe(_ => {
                            var formKeyPicker = new FormKeyPicker {
                                [!AFormKeyPicker.LinkCacheProperty] = vm.EditorEnvironment.LinkCacheChanged.ToBinding(),
                                [!AFormKeyPicker.ScopedTypesProperty] = vm.WhenAnyValue(x => x.ScopedTypes).ToBinding(),
                            };

                            var dialog = new TaskDialog {
                                Header = "Remap References",
                                Content = formKeyPicker,
                                XamlRoot = this,
                                Buttons = {
                                    new TaskDialogButton {
                                        Text = "Remap",
                                        DialogResult = "OK",
                                        Command = vm.RemapReferences,
                                        [!TaskDialogButton.CommandParameterProperty] = formKeyPicker.FormKeyChanged.ToBinding()
                                    },
                                    TaskDialogButton.CancelButton
                                }
                            };

                            dialog.ShowAsync();
                        })
                        .DisposeWith(disposables);
                });
        });
    }
}
