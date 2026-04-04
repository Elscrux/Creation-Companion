using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using ReactiveUI.Avalonia;
using Avalonia.Xaml.Interactions.DragAndDrop;
using Avalonia.Xaml.Interactivity;
using CreationEditor.Avalonia.Attached.DragDrop;
using CreationEditor.Avalonia.ViewModels.Reference;
using CreationEditor.Avalonia.Views.Asset.Picker.File;
using CreationEditor.Avalonia.Views.Record.Picker;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Assets;
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
                            if (vm is { DataSourceLink: {} dataSourceLink, AssetType: {} assetType }) {
                                RemapAssets(vm, assetType, dataSourceLink);
                            } else if (vm.ReferencedRecordContext is {} referencedRecord) {
                                RemapRecords(vm, referencedRecord);
                            }
                        })
                        .DisposeWithComposite(disposables);
                });
        });
    }

    private void RemapAssets(ReferenceRemapperVM vm, IAssetType assetType, DataSourceFileLink dataSourceLink) {
        var filePicker = new TextFilePicker {
            [AFilePicker.FilterProperty] = new FilePickerFileType(assetType.BaseFolder) {
                Patterns = assetType.FileExtensions.Select(extension => "*" + extension).ToArray(),
            }.AsEnumerable().ToArray(),
            [Interaction.BehaviorsProperty] = new BehaviorCollection {
                new ContextDropBehavior { Handler = new CustomDragDropDataHandler<AssetLinkDragDrop, AssetDataSourceLink>() }
            },
            [DragDropExtended.AllowDropProperty] = true,
            [AssetLinkDragDrop.CanSetAssetLinkProperty]
                = new Func<AssetDataSourceLink, bool>((link => link.AssetLink.AssetTypeInstance == assetType)),
        };
        filePicker[AssetLinkDragDrop.SetAssetLinkProperty] = new Action<AssetDataSourceLink?>((link => {
            if (link is not null) {
                filePicker.FilePath = link.DataSourceLink.DataRelativePath.Path;
            }
        }));

        var dialog = new TaskDialog {
            Header = "Remap References for " + dataSourceLink.Name,
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
    }

    private void RemapRecords(ReferenceRemapperVM vm, IReferencedRecord referencedRecord) {
        var formKeyPicker = new FormKeyPicker {
            [!AFormKeyPicker.LinkCacheProperty] = vm.LinkCacheProvider.LinkCacheChanged.ToBinding(),
            [!AFormKeyPicker.ScopedTypesProperty] = vm.WhenAnyValue(x => x.ScopedTypes).ToBinding(),
        };
        var stackPanel = new StackPanel { Children = { formKeyPicker } };

        var preprocessorCheckboxes = new List<(CheckBox CheckBox, IRecordRemappingPreprocessor Preprocessor)>();
        foreach (var preprocessor in vm.RecordRemappingPreprocessors) {
            if (preprocessor.IsApplicable(referencedRecord)) {
                var checkBox = new CheckBox { Content = preprocessor.Description };
                stackPanel.Children.Add(checkBox);
                preprocessorCheckboxes.Add((checkBox, preprocessor));
            }
        }

        var dialog = new TaskDialog {
            Header = "Remap References for " + (referencedRecord.Record.GetHumanReadableName() ?? "Unknown"),
            Content = stackPanel,
            XamlRoot = this,
            Buttons = {
                new TaskDialogButton {
                    Text = "Remap",
                    DialogResult = "OK",
                    Command = ReactiveCommand.CreateFromTask(async () => {
                        foreach (var preprocessorCheckbox in preprocessorCheckboxes) {
                            if (preprocessorCheckbox.CheckBox.IsChecked is true) {
                                preprocessorCheckbox.Preprocessor
                                    .PreprocessRemapping(referencedRecord, formKeyPicker.FormKey);
                            }
                        }

                        await vm.RemapRecordReferences.Execute(formKeyPicker.FormKey);
                    }),
                },
                TaskDialogButton.CancelButton,
            },
        };

        dialog.ShowAsync();
    }
}
