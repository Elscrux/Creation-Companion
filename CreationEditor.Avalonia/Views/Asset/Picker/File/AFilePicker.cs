using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views.Asset.Picker.File;

public class AFilePicker : ActivatableTemplatedControl {
    public static readonly StyledProperty<string> TitleProperty
        = AvaloniaProperty.Register<AFilePicker, string>(nameof(Title), "Pick");

    public string Title {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<IReadOnlyList<FilePickerFileType>?> FilterProperty
        = AvaloniaProperty.Register<AFilePicker, IReadOnlyList<FilePickerFileType>?>(nameof(Filter));

    public IReadOnlyList<FilePickerFileType>? Filter {
        get => GetValue(FilterProperty);
        set => SetValue(FilterProperty, value);
    }

    public static readonly StyledProperty<string?> FilePathProperty
        = AvaloniaProperty.Register<AFilePicker, string?>(nameof(FilePath));

    public string? FilePath {
        get => GetValue(FilePathProperty);
        set => SetValue(FilePathProperty, value);
    }

    public static readonly StyledProperty<ReactiveCommand<Unit, Unit>> PickProperty
        = AvaloniaProperty.Register<AFilePicker, ReactiveCommand<Unit, Unit>>(nameof(Pick));

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
        var filePickerOpenOptions = new FilePickerOpenOptions {
            Title = Title,
            SuggestedStartLocation = _startFolder,
            SuggestedFileName = null,
            FileTypeFilter = Filter,
        };

        // Open dialog
        var pickedFiles = await storageProvider.OpenFilePickerAsync(filePickerOpenOptions);

        // Process results
        var pickedFile = pickedFiles.FirstOrDefault();
        if (pickedFile is not null) _startFolder = await pickedFile.GetParentAsync();
        FilePath = pickedFile?.Path.LocalPath;
    }
}
