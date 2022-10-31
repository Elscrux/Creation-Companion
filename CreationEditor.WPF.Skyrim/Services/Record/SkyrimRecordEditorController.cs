using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Autofac;
using CreationEditor.WPF.Services.Docking;
using CreationEditor.WPF.Services.Record;
using CreationEditor.WPF.ViewModels.Docking;
using CreationEditor.WPF.ViewModels.Record;
using Elscrux.Logging;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Serilog;
using DocumentClosedEventArgs = AvalonDock.DocumentClosedEventArgs;
namespace CreationEditor.WPF.Skyrim.Services.Record;

public class SkyrimRecordEditorController : IRecordEditorController {
    private readonly ILogger _logger;
    private readonly ILifetimeScope _lifetimeScope;
    private readonly IDockingManagerService _dockingManagerService;

    private readonly Dictionary<FormKey, UserControl> _recordEditors = new();

    public SkyrimRecordEditorController(
        ILogger logger,
        ILifetimeScope lifetimeScope,
        IDockingManagerService dockingManagerService) {
        _logger = logger;
        _lifetimeScope = lifetimeScope;
        _dockingManagerService = dockingManagerService;
        
        _dockingManagerService.DockingManager.DocumentClosed += OnClosed;
    }

    public void OpenEditor<TMajorRecord, TMajorRecordGetter>(TMajorRecord record)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter {
        
        if (_recordEditors.TryGetValue(record.FormKey, out var editor)) {
            // Select editor as active
            _dockingManagerService.SetActiveControl(editor);
        } else {
            // Open new editor
            if (_lifetimeScope.TryResolve<IRecordEditorVM<TMajorRecord, TMajorRecordGetter>>(out var recordEditorVM)) {
                var editorControl = recordEditorVM.CreateUserControl(record);

                _dockingManagerService.AddDocumentControl(editorControl, record.EditorID ?? record.FormKey.ToString());
                _recordEditors.Add(record.FormKey, editorControl);
            } else {
                _logger.Here().Warning("Cannot open record editor of type {Type} because no such editor is available", typeof(TMajorRecord));
            }
        }
    }
    
    public void CloseEditor(IMajorRecord record) {
        if (_recordEditors.TryGetValue(record.FormKey, out var editor)) {
            _dockingManagerService.RemoveControl(editor);

            RemoveEditorCache(editor);
        }
    }

    private void OnClosed(object? sender, DocumentClosedEventArgs e) {
        if (e.Document.Content is PaneVM paneVM) {
            RemoveEditorCache(paneVM.Control);
        }
    }
    
    private void RemoveEditorCache(UserControl editor) {
        var editorsToRemove = _recordEditors
            .Where(x => x.Value == editor)
            .Select(x => x.Key)
            .ToList();

        foreach (var key in editorsToRemove) {
            _recordEditors.Remove(key);
        }
    }
}
