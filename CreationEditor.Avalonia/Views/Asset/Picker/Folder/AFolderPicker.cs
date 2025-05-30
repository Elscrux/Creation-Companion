using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views.Asset.Picker.Folder;

public class AFolderPicker : ActivatableTemplatedControl {
    public static readonly StyledProperty<string> TitleProperty
        = AvaloniaProperty.Register<AFolderPicker, string>(nameof(Title), "Pick");

    public string Title {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<string?> FolderPathProperty
        = AvaloniaProperty.Register<AFolderPicker, string?>(nameof(FolderPath));

    public string? FolderPath {
        get => GetValue(FolderPathProperty);
        set => SetValue(FolderPathProperty, value);
    }

    public static readonly StyledProperty<ReactiveCommand<Unit, Unit>> PickProperty
        = AvaloniaProperty.Register<AFolderPicker, ReactiveCommand<Unit, Unit>>(nameof(Pick));

    public ReactiveCommand<Unit, Unit> Pick {
        get => GetValue(PickProperty);
        set => SetValue(PickProperty, value);
    }

    private IStorageFolder? _startFolder;

    protected override void WhenActivated() {
        base.WhenActivated();

        Pick = ReactiveCommand.CreateFromTask(PickImpl);
    }

    public async Task PickImpl() {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null) return;

        var storageProvider = topLevel.StorageProvider;
        var folderPickerOpenOptions = new FolderPickerOpenOptions {
            Title = Title,
            SuggestedStartLocation = _startFolder,
        };

        // Open dialog
        var pickedFolders = await storageProvider.OpenFolderPickerAsync(folderPickerOpenOptions);

        // Process results
        var pickedFolder = pickedFolders.FirstOrDefault();
        if (pickedFolder is not null) _startFolder = pickedFolder;
        FolderPath = pickedFolder?.Path.LocalPath.TrimEnd('\\');
    }
}
