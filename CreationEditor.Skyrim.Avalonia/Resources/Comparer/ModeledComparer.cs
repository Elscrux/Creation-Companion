using System;
using System.Collections.Generic;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Resources.Comparer;

public class ModeledComparer : IComparer<IModeledGetter> {
    public static readonly ModeledComparer Instance = new();
    
    public int Compare(IModeledGetter? x, IModeledGetter? y) {
        return StringComparer.OrdinalIgnoreCase.Compare(x?.Model?.File.DataRelativePath, y?.Model?.File.DataRelativePath);
    }
}
