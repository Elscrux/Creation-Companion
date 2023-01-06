using System;
using CreationEditor.Avalonia.Comparer;
using CreationEditor.Avalonia.Models.Record;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Resources.Comparer; 

public static class SkyrimRecordComparers {
    public static readonly FuncSelectorComparer<IReferencedRecord, IModeledGetter> ModeledComparer
        = new(referencedRecord => referencedRecord.Record as IModeledGetter,
            (x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.Model?.File.DataRelativePath, y.Model?.File.DataRelativePath));
}
