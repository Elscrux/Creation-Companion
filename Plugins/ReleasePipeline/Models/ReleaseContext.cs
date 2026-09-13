using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Skyrim;
namespace ReleasePipeline.Models;

/// <summary>
/// The thing to be released: a data source together with the single mod it contains. The
/// data source holds the mod and all other assets that should be released.
/// </summary>
public sealed record ReleaseContext(
    IDataSource DataSource,
    ISkyrimModGetter Mod);
