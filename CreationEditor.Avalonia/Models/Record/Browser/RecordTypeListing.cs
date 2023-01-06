using Loqui;
namespace CreationEditor.Avalonia.Models.Record.Browser; 

public sealed class RecordTypeListing {
    public ILoquiRegistration Registration { get; }
    public string RecordTypeName { get; set; }

    public RecordTypeListing(ILoquiRegistration registration) {
        Registration = registration;
        RecordTypeName = registration.Name;
    }
}
