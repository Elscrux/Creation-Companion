using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.Mod;

public interface IModInfoProvider<in TModGetter>
    where TModGetter : IModGetter {

    public uint GetRecordCount(TModGetter mod);
}
