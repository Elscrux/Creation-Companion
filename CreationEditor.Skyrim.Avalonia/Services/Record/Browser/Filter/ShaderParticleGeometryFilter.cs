using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class ShaderParticleGeometryFilter : SimpleRecordFilter<IShaderParticleGeometryGetter> {
    public ShaderParticleGeometryFilter() : base(new List<SimpleRecordFilterEntry> {
        new("Rain", record => record.Type == ShaderParticleGeometry.TypeEnum.Rain),
        new("Snow", record => record.Type == ShaderParticleGeometry.TypeEnum.Snow),
    }) {}
}
