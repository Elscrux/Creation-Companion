using System.IO.Abstractions;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public class ModeledFilter : DirectoryFilter<IModeledGetter> {
    public ModeledFilter(IEditorEnvironment editorEnvironment, IFileSystem fileSystem)
        : base(editorEnvironment, fileSystem) {}

    protected override string? GetPath(IModeledGetter record) => record.Model?.File.DataRelativePath;
}
