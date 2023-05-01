using System.Collections.Generic;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class VoiceTypeFilter : SimpleRecordFilter<IVoiceTypeGetter> {
    public VoiceTypeFilter() : base(new List<SimpleRecordFilterEntry> {
        new("Default", record => (record.Flags & VoiceType.Flag.AllowDefaultDialog) != 0),
        new("Male", record => (record.Flags & VoiceType.Flag.Female) == 0),
        new("Female", record => (record.Flags & VoiceType.Flag.Female) != 0),
    }) {}
}
