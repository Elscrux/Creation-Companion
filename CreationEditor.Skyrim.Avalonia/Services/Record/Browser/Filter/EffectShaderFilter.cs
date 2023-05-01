using System.Collections.Generic;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class EffectShaderFilter : SimpleRecordFilter<IEffectShaderGetter> {
    public EffectShaderFilter() : base(new List<SimpleRecordFilterEntry> {
        new("Particle Shader", record => (record.Flags & EffectShader.Flag.NoParticleShader) == 0),
        new("Membrane Shader", record => (record.Flags & EffectShader.Flag.NoMembraneShader) == 0),
    }) {}
}
