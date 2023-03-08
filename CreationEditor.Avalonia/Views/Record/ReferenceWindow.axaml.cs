using FluentAvalonia.UI.Windowing;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationEditor.Avalonia.Views.Record;

public partial class ReferenceWindow : AppWindow {
    public ReferenceWindow() {
        InitializeComponent();
    }

    public ReferenceWindow(IMajorRecordIdentifier record) : this() {
        var editorId = record.EditorID;
        Title = $"References of {record.FormKey}" + (editorId == null ? string.Empty : $" - {editorId}");
    }
}
