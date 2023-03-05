using System.IO.Abstractions;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public class ArmorFilter : DirectoryFilter<IArmorGetter> {
    public ArmorFilter(IEditorEnvironment editorEnvironment, IFileSystem fileSystem)
        : base(editorEnvironment, fileSystem) {}

    protected override string? GetPath(IArmorGetter record) => record.WorldModel?.Male?.Model?.File.DataRelativePath;
}
