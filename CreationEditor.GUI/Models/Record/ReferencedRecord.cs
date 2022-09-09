using System.Collections.Generic;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.GUI.Models.Record;

public record ReferencedRecord<TMajorRecordGetter>(TMajorRecordGetter Record, HashSet<IFormLinkIdentifier> References)
    where TMajorRecordGetter : IMajorRecordIdentifier;
