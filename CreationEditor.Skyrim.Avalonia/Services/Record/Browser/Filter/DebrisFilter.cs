using System.IO.Abstractions;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class DebrisFilter : AssetFilter<IDebrisGetter> {
    public DebrisFilter(
        ILinkCacheProvider linkCacheProvider,
        IFileSystem fileSystem)
        : base(linkCacheProvider, fileSystem) {}
}
