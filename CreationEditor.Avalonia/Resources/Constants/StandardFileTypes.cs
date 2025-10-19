using Avalonia.Platform.Storage;
namespace CreationEditor.Avalonia.Constants;

public static class StandardFileTypes {
    public static readonly IReadOnlyList<FilePickerFileType> ImageAll = [FilePickerFileTypes.ImageAll];
    public static readonly IReadOnlyList<FilePickerFileType> Csv = [
        new("Csv file") {
            Patterns = ["*.csv"],
            AppleUniformTypeIdentifiers = ["public.comma-separated-values-text"],
            MimeTypes = ["text/csv"]
        }
    ];
}
