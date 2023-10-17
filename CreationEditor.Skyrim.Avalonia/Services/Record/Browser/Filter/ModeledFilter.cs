using System.Collections.Generic;
using System.IO.Abstractions;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class ModeledFilter : AssetFilter<IModeledGetter> {
    public ModeledFilter(
        ILinkCacheProvider linkCacheProvider,
        IFileSystem fileSystem)
        : base(linkCacheProvider, fileSystem) {}

    protected override IEnumerable<string> GetPaths(IModeledGetter record) {
        if (record.Model is not null) yield return record.Model.File.DataRelativePath;
    }
}
