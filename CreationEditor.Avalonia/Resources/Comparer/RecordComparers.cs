﻿using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Comparer;

public static class RecordComparers {
    public static readonly FuncComparer<IMajorRecordIdentifier> FormKeyComparer
        = new((x, y) => {
            var modKeyCompare = StringComparer.OrdinalIgnoreCase.Compare(x.FormKey.ModKey.Name, y.FormKey.ModKey.Name);
            if (modKeyCompare != 0) return modKeyCompare;

            return StringComparer.OrdinalIgnoreCase.Compare(x.FormKey.ID, y.FormKey.ID);
        });

    public static readonly FuncComparer<IMajorRecordIdentifier> EditorIDComparer
        = new((x, y) => {
            var xEditorID = x.EditorID;
            var yEditorID = y.EditorID;

            var xIsNullOrEmpty = string.IsNullOrEmpty(xEditorID);
            var yIsNullOrEmpty = string.IsNullOrEmpty(yEditorID);
            if (xIsNullOrEmpty) {
                if (yIsNullOrEmpty) return 0;

                return 1;
            }
            if (yIsNullOrEmpty) return -1;

            var editorIDCompare = StringComparer.OrdinalIgnoreCase.Compare(xEditorID, yEditorID);
            if (editorIDCompare != 0) return editorIDCompare;

            return FormKeyComparer.Compare(x, y);
        });

    public static readonly FuncSelectorComparer<IMajorRecordIdentifier, INamedRequiredGetter> NamedRequiredComparer
        = new(referencedRecord => referencedRecord as INamedRequiredGetter,
            (x, y) => NamedRequiredComparer.Compare(x, y));
}
