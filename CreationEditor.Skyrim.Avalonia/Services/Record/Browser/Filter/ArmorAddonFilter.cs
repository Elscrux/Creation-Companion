using System.IO.Abstractions;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public class ArmorAddonFilter : DirectoryFilter<IArmorAddonGetter> {
    public ArmorAddonFilter(IEditorEnvironment editorEnvironment, IFileSystem fileSystem)
        : base(editorEnvironment, fileSystem) {}

    protected override string? GetPath(IArmorAddonGetter record) => record.WorldModel?.Male?.File.DataRelativePath;
}
