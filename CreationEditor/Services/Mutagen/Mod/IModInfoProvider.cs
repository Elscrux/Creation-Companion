using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.Mod;

public interface IModInfoProvider<in TModGetter>
    where TModGetter : IModGetter {

    /// <summary>
    /// Returns the number of records in a mod
    /// </summary>
    /// <param name="mod">Mod to get number of records for</param>
    /// <returns>Approximate number of records in a mod - may not be fully accurate!</returns>
    uint GetRecordCount(TModGetter mod);
}
