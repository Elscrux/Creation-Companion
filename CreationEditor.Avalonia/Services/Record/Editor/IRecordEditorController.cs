using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Services.Record.Editor;

public interface IRecordEditorController {
    /// <summary>
    /// Open the editor for the given record.
    /// </summary>
    /// <param name="record">Record to open the editor for</param>
    /// <typeparam name="TMajorRecord">Setter record type</typeparam>
    /// <typeparam name="TMajorRecordGetter">Getter record type</typeparam>
    void OpenEditor<TMajorRecord, TMajorRecordGetter>(TMajorRecord record)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter;

    /// <summary>
    /// Open the editor for the given record.
    /// </summary>
    /// <param name="record">Record to open the editor for</param>
    void OpenEditor(IMajorRecord record);

    /// <summary>
    /// Close the editor for the given record.
    /// </summary>
    /// <param name="record">Record to close the editor for</param>
    void CloseEditor(IMajorRecord record);

    /// <summary>
    /// Close all record editors.
    /// </summary>
    void CloseAllEditors();

    /// <summary>
    /// Check if any record editors are open.
    /// </summary>
    /// <returns>True if at least one record editor is open</returns>
    bool AnyEditorsOpen();
}
