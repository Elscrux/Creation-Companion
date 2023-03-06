using System.Collections.Generic;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public class MessageFilter : SimpleRecordFilter<IMessageGetter> {
    public MessageFilter() : base(new List<SimpleRecordFilterEntry> {
        new("Message Box", record => (record.Flags & Message.Flag.MessageBox) != 0),
        new("Notification", record => (record.Flags & Message.Flag.MessageBox) == 0),
    }) {}
}
