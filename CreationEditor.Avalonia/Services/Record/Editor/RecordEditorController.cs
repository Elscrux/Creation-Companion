using Autofac;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Docking;
using CreationEditor.Avalonia.ViewModels.Record.Editor;
using CreationEditor.Extension;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Serilog;
namespace CreationEditor.Avalonia.Services.Record.Editor;

public sealed class RecordEditorController : IRecordEditorController {
    private readonly ILogger _logger;
    private readonly ILifetimeScope _lifetimeScope;
    private readonly IDockingManagerService _dockingManagerService;
    private readonly IRecordEditorFactory _recordEditorFactory;

    private record EditorControl(Control Control, IRecordEditorVM EditorVM);

    private readonly Dictionary<FormKey, EditorControl> _openRecordEditors = new();

    public RecordEditorController(
        ILogger logger,
        ILifetimeScope lifetimeScope,
        IDockingManagerService dockingManagerService,
        IRecordEditorFactory recordEditorFactory) {
        _logger = logger;
        _lifetimeScope = lifetimeScope;
        _dockingManagerService = dockingManagerService;
        _recordEditorFactory = recordEditorFactory;

        _dockingManagerService.Closed.Subscribe(OnClosed);
    }
    
    public bool AnyEditorsOpen() => _openRecordEditors.Count > 0;

    public void OpenEditor<TMajorRecord, TMajorRecordGetter>(TMajorRecord record)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter {
        
        if (_openRecordEditors.TryGetValue(record.FormKey, out var editorControl)) {
            // Select editor as active
            _dockingManagerService.Focus(editorControl.Control);
        } else {
            // Open new editor
            if (_lifetimeScope.TryResolve<IRecordEditorVM<TMajorRecord, TMajorRecordGetter>>(out var recordEditorVM)) {
                var control = recordEditorVM.CreateControl(record);

                _dockingManagerService.AddControl(
                    control, 
                    new DockConfig {
                        DockInfo = new DockInfo {
                            Header = record.EditorID ?? record.FormKey.ToString(),
                        },
                        Dock = Dock.Right,
                        DockMode = DockMode.Document,
                        GridSize = new GridLength(2, GridUnitType.Star),
                    });
                _openRecordEditors.Add(record.FormKey, new EditorControl(control, recordEditorVM));
            } else {
                _logger.Here().Warning("Cannot open record editor of type {Type} because no such editor is available", typeof(TMajorRecord));
            }
        }
    }
    
    public void OpenEditor(IMajorRecord record) {
        if (_openRecordEditors.TryGetValue(record.FormKey, out var editorControl)) {
            // Select editor as active
            _dockingManagerService.Focus(editorControl.Control);
        } else {
            // Open new editor
            if (_recordEditorFactory.FromType(record, out var control, out var editor)) {
                _dockingManagerService.AddControl(
                    control, 
                    new DockConfig {
                        DockInfo = new DockInfo {
                            Header = record.EditorID ?? record.FormKey.ToString(),
                        },
                        Dock = Dock.Right,
                        DockMode = DockMode.Document,
                        GridSize = new GridLength(2, GridUnitType.Star),
                    });
                _openRecordEditors.Add(record.FormKey, new EditorControl(control, editor));
            } else {
                _logger.Here().Warning("Cannot open record editor of type {Type} because no such editor is available", record.GetType());
            }
        }
    }
    
    public void CloseEditor(IMajorRecord record) {
        if (_openRecordEditors.TryGetValue(record.FormKey, out var editorControl)) {
            _dockingManagerService.RemoveControl(editorControl.Control);
            
            RemoveEditorCache(editorControl.Control);
        }
    }

    private void OnClosed(IDockedItem dockedItem) {
        RemoveEditorCache(dockedItem.Control);
    }
    
    private void RemoveEditorCache(Control editor) {
        var editorsToRemove = _openRecordEditors
            .Where(x => ReferenceEquals(x.Value, editor))
            .Select(x => x.Key)
            .ToList();

        foreach (var key in editorsToRemove) {
            _openRecordEditors.Remove(key);
        }
    }
}
