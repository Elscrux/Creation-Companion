using ReleasePipeline.ViewModels;
namespace ReleasePipeline.Services.ConfigEditor;

/// <summary>
/// Produces editable field wrappers for an action's configuration object by reflecting over
/// its public properties. Used by the generic config editor in the pipeline editor.
/// </summary>
public interface IConfigEditorService {
    /// <summary>Creates a field VM for each editable public property of the given config object.</summary>
    IReadOnlyList<ConfigFieldVM> CreateFields(object config);
}
