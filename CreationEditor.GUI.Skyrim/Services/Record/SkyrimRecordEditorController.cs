using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Autofac;
using CreationEditor.GUI.Services.Docking;
using CreationEditor.GUI.Services.Record;
using CreationEditor.GUI.ViewModels.Record;
using Elscrux.Logging;
using Elscrux.WPF.Extensions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Serilog;
using Syncfusion.Windows.Tools.Controls;
namespace CreationEditor.GUI.Skyrim.Services.Record;

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
        
        _dockingManagerService.GetDockingManager().CloseButtonClick += OnClosed;
    }

    public void OpenEditor<TMajorRecord, TMajorRecordGetter>(TMajorRecord record)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter {
        
        if (_recordEditors.TryGetValue(record.FormKey, out var editor)) {
            // Select editor as active
            _dockingManagerService.GetDockingManager().ActiveWindow = editor;
        } else {
            // Open new editor
            if (_lifetimeScope.TryResolve<IRecordEditorVM<TMajorRecord, TMajorRecordGetter>>(out var recordEditorVM)) {
                var editorControl = recordEditorVM.CreateUserControl(record);
            
            
                _dockingManagerService.GetDockingManager().AddControl(editorControl, record.ToString() ?? string.Empty);
                _recordEditors.Add(record.FormKey, editorControl);
            } else {
                _logger.Here().Warning("Cannot open record editor of type {Type} because no such editor is available", typeof(TMajorRecord));
            }
        }
    }
    
    public void CloseEditor(IMajorRecord record) {
        if (_recordEditors.TryGetValue(record.FormKey, out var editor)) {
            _dockingManagerService.GetDockingManager().ExecuteClose(editor);
            
            RemoveEditorCache(editor);
        }
    }

    private void OnClosed(object sender, CloseButtonEventArgs e) {
        if (e.TargetItem is UserControl editor) {
            RemoveEditorCache(editor);
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
