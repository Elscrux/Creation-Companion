using System.IO.Abstractions;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Avalonia.Services.Record.Browser.Filter;

public abstract class DirectoryFilter<T> : IRecordFilter {
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IFileSystem _fileSystem;

    public Type Type => typeof(T);

    public DirectoryFilter(
        IEditorEnvironment editorEnvironment,
        IFileSystem fileSystem) {
        _editorEnvironment = editorEnvironment;
        _fileSystem = fileSystem;
    }

    public IEnumerable<RecordFilterListing> GetListings(Type type) {
        var recordFilterListings = _editorEnvironment.LinkCache.PriorityOrder.WinningOverrides(type)
            .OfType<T>()
            .GetRecursiveListings(_fileSystem.Path.DirectorySeparatorChar, GetPath)
            .ToList();

        // Trim standalone folders
        while (recordFilterListings.Count == 1) {
            recordFilterListings = recordFilterListings.First().RecordFilters.ToList();
        }

        return recordFilterListings;
    }

    protected abstract string? GetPath(T record);
}
