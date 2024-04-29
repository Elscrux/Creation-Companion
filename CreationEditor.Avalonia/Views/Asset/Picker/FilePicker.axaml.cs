using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
namespace CreationEditor.Avalonia.Views.Asset.Picker;

public partial class FilePicker : UserControl {
    public static readonly StyledProperty<string?> PathStringsProperty
        = AvaloniaProperty.Register<FilePicker, string?>(nameof(PathStrings));

    public string? PathStrings {
        get => GetValue(PathStringsProperty);
        set => SetValue(PathStringsProperty, value);
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

    public static readonly StyledProperty<bool> AllowMultipleProperty
        = AvaloniaProperty.Register<FilePicker, bool>(nameof(AllowMultiple));

    public bool AllowMultiple {
        get => GetValue(AllowMultipleProperty);
        set => SetValue(AllowMultipleProperty, value);
    }

    public static readonly StyledProperty<IReadOnlyList<FilePickerFileType>?> FilterProperty
        = AvaloniaProperty.Register<FilePicker, IReadOnlyList<FilePickerFileType>?>(nameof(Filter));

    public IReadOnlyList<FilePickerFileType>? Filter {
        get => GetValue(FilterProperty);
        set => SetValue(FilterProperty, value);
    }

    public static readonly StyledProperty<IReadOnlyList<IStorageFile>> PickedFilesProperty
        = AvaloniaProperty.Register<FilePicker, IReadOnlyList<IStorageFile>>(nameof(PickedFiles));

    public IReadOnlyList<IStorageFile> PickedFiles {
        get => GetValue(PickedFilesProperty);
        set => SetValue(PickedFilesProperty, value);
    }

    public static readonly StyledProperty<IStorageFile?> PickedFileProperty
        = AvaloniaProperty.Register<FilePicker, IStorageFile?>(nameof(PickedFile));

    public IStorageFile? PickedFile {
        get => GetValue(PickedFileProperty);
        set => SetValue(PickedFileProperty, value);
    }

    private IStorageFolder? _startFolder;

    public FilePicker() {
        InitializeComponent();
    }

    public async Task Pick() {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null) return;

        var filePickerOpenOptions = new FilePickerOpenOptions {
            Title = Title,
            SuggestedStartLocation = _startFolder,
            SuggestedFileName = null,
            AllowMultiple = AllowMultiple,
            FileTypeFilter = Filter
        };
        var storageProvider = topLevel.StorageProvider;
        PickedFiles = await storageProvider.OpenFilePickerAsync(filePickerOpenOptions);
        PickedFile = PickedFiles.FirstOrDefault();
        if (PickedFile is not null) _startFolder = await PickedFile.GetParentAsync();
        PathStrings = string.Join(", ", PickedFiles.Select(x => x.Path.LocalPath));
    }
}
