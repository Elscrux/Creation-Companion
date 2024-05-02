using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views.Asset.Picker;

public partial class FilePicker : UserControl {
    public static readonly StyledProperty<string?> WatermarkProperty
        = AvaloniaProperty.Register<FilePicker, string?>(nameof(Watermark));

    public string? Watermark {
        get => GetValue(WatermarkProperty);
        set => SetValue(WatermarkProperty, value);
    }

    public static readonly StyledProperty<bool> AllowTextEditProperty
        = AvaloniaProperty.Register<FilePicker, bool>(nameof(AllowTextEdit));

    public bool AllowTextEdit {
        get => GetValue(AllowTextEditProperty);
        set => SetValue(AllowTextEditProperty, value);
    }

    public static readonly StyledProperty<string> TitleProperty
        = AvaloniaProperty.Register<FilePicker, string>(nameof(Title), "Pick");

    public string Title {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<IReadOnlyList<FilePickerFileType>?> FilterProperty
        = AvaloniaProperty.Register<FilePicker, IReadOnlyList<FilePickerFileType>?>(nameof(Filter));

    public IReadOnlyList<FilePickerFileType>? Filter {
        get => GetValue(FilterProperty);
        set => SetValue(FilterProperty, value);
    }

    public static readonly StyledProperty<string?> FilePathProperty
        = AvaloniaProperty.Register<FilePicker, string?>(nameof(FilePath));

    public string? FilePath {
        get => GetValue(FilePathProperty);
        set => SetValue(FilePathProperty, value);
    }

    private IStorageFolder? _startFolder;

    public FilePicker() {
        InitializeComponent();
    }

    public async Task Pick() {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null) return;

        var storageProvider = topLevel.StorageProvider;
        var filePickerOpenOptions = new FilePickerOpenOptions {
            Title = Title,
            SuggestedStartLocation = _startFolder,
            SuggestedFileName = null,
            FileTypeFilter = Filter
        };

        // Open dialog
        var pickedFiles = await storageProvider.OpenFilePickerAsync(filePickerOpenOptions);

        // Process results
        var pickedFile = pickedFiles.FirstOrDefault();
        if (pickedFile is not null) _startFolder = await pickedFile.GetParentAsync();
        FilePath = pickedFile?.Path.LocalPath;
    }
}
