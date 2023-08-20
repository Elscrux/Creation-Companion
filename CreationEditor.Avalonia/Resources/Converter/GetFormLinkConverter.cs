using Avalonia.Data.Converters;
using CreationEditor.Services.Mutagen.References.Record;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Avalonia.Converter; 

public static class FormLinkConverter {
    public static readonly Func<object?, IFormLinkIdentifier> ToFormLinkFunc
        = obj => obj is not IReferencedRecord referencedRecord
            ? FormLinkInformation.Null
            : FormLinkInformation.Factory(referencedRecord.Record);

    public static readonly FuncValueConverter<object?, IFormLinkIdentifier> ToFormLink
        = new(obj => ToFormLinkFunc(obj));
}
