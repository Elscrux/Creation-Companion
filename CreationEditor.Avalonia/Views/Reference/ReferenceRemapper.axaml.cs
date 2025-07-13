using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using Avalonia.Xaml.Interactions.DragAndDrop;
using Avalonia.Xaml.Interactivity;
using CreationEditor.Avalonia.Attached.DragDrop;
using CreationEditor.Avalonia.ViewModels.Reference;
using CreationEditor.Avalonia.Views.Asset.Picker.File;
using CreationEditor.Avalonia.Views.Record.Picker;
using FluentAvalonia.UI.Controls;
using Noggog;
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
                            if (vm.DataSourceLink is not null && vm.AssetType is not null) {
                                var filePicker = new TextFilePicker {
                                    [AFilePicker.FilterProperty] = new FilePickerFileType(vm.AssetType.BaseFolder) {
                                        Patterns = vm.AssetType.FileExtensions.Select(extension => "*" + extension).ToArray(),
                                    }.AsEnumerable().ToArray(),
                                    [Interaction.BehaviorsProperty] = new BehaviorCollection {
                                        new ContextDropBehavior { Handler = new CustomDragDropDataHandler<AssetLinkDragDrop, AssetDataSourceLink>() }
                                    },
                                    [DragDropExtended.AllowDropProperty] = true,
                                    [AssetLinkDragDrop.CanSetAssetLinkProperty]
                                        = new Func<AssetDataSourceLink, bool>((link => link.AssetLink.AssetTypeInstance == vm.AssetType)),
                                };
                                filePicker[AssetLinkDragDrop.SetAssetLinkProperty] = new Action<AssetDataSourceLink?>((link => {
                                    if (link is not null) {
                                        filePicker.FilePath = link.DataSourceLink.DataRelativePath.Path;
                                    }
                                }));

                                var dialog = new TaskDialog {
                                    Header = "Remap References for " + vm.DataSourceLink.Name,
                                    Content = filePicker,
                                    XamlRoot = this,
                                    Buttons = {
                                        new TaskDialogButton {
                                            Text = "Remap",
                                            DialogResult = "OK",
                                            Command = vm.RemapAssetReferences,
                                            [!TaskDialogButton.CommandParameterProperty] = filePicker
                                                .GetObservable(AFilePicker.FilePathProperty)
                                                .NotNull()
                                                .Select(path => vm.DataSourceService.GetFileLink(path))
                                                .NotNull()
                                                .ToBinding(),
                                        },
                                        TaskDialogButton.CancelButton,
                                    },
                                };

                                dialog.ShowAsync();
                            } else if (vm.ReferencedRecordContext is not null) {
                                var formKeyPicker = new FormKeyPicker {
                                    [!AFormKeyPicker.LinkCacheProperty] = vm.LinkCacheProvider.LinkCacheChanged.ToBinding(),
                                    [!AFormKeyPicker.ScopedTypesProperty] = vm.WhenAnyValue(x => x.ScopedTypes).ToBinding(),
                                };

                                var dialog = new TaskDialog {
                                    Header = "Remap References for " + (vm.ReferencedRecordContext?.Record.GetHumanReadableName() ?? "Unknown"),
                                    Content = formKeyPicker,
                                    XamlRoot = this,
                                    Buttons = {
                                        new TaskDialogButton {
                                            Text = "Remap",
                                            DialogResult = "OK",
                                            Command = vm.RemapRecordReferences,
                                            [!TaskDialogButton.CommandParameterProperty] = formKeyPicker.FormKeyChanged.ToBinding(),
                                        },
                                        TaskDialogButton.CancelButton,
                                    },
                                };

                                dialog.ShowAsync();
                            }
                        })
                        .DisposeWith(disposables);
                });
        });
    }
}
