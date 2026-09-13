using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.Integrations;
using ReactiveUI;
using ReleasePipeline.Models.Actions;
using ReleasePipeline.Services.Actions;
using ReleasePipeline.Services.ConfigEditor;
namespace ReleasePipeline.ViewModels;

/// <summary>
/// UI-editable wrapper around a <see cref="IPipelineAction"/>. Exposes the action's
/// configuration for editing inline via an expander: either a custom config VM (e.g. the BSA
/// archive action) or reflection-based config fields.
/// </summary>
public sealed partial class PipelineActionVM : ViewModel {
    public IPipelineAction Action { get; }

    public string DisplayName => Action.DisplayName;
    public string Description => Action.Description;
    public Guid Id => Action.Id;

    /// <summary>The action's configuration object, edited via the generic config editor.</summary>
    public object Config => GetConfig(Action);

    /// <summary>Custom config view model (e.g. BSA archive action), or null for reflection-based editing.</summary>
    public object? ConfigVM { get; }

    /// <summary>Reflection-based config fields, used when <see cref="ConfigVM"/> is null.</summary>
    public IReadOnlyList<ConfigFieldVM> ConfigFields { get; }

    /// <summary>True when the action exposes any editable configuration.</summary>
    public bool HasConfig => ConfigVM is not null || ConfigFields.Count > 0;

    /// <summary>Emits this VM when the remove command is executed.</summary>
    public IObservable<PipelineActionVM> RemoveRequested { get; }

    public ReactiveCommand<Unit, Unit> RemoveCommand { get; }

    public PipelineActionVM(
        IPipelineAction action,
        IConfigEditorService configEditorService,
        IIntegrationRegistry integrationRegistry,
        IIntegrationProvider integrationProvider) {
        Action = action;

        if (action is BSAArchiveAction bsaAction) {
            ConfigVM = new BSAArchiveActionConfigVM(
                bsaAction.Config,
                integrationRegistry,
                integrationProvider);
            ConfigFields = [];
        } else {
            ConfigVM = null;
            ConfigFields = configEditorService.CreateFields(Config);
        }

        RemoveCommand = ReactiveCommand.Create(() => { });
        RemoveRequested = RemoveCommand.Select(_ => this);
    }

    /// <summary>
    /// Reads the strongly-typed <c>Config</c> from the action's generic
    /// <see cref="IPipelineAction{TConfig}"/> implementation.
    /// </summary>
    private static object GetConfig(IPipelineAction action) {
        var configInterface = action.GetType()
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineAction<>));

        if (configInterface is null) return new object();

        // Use the closed generic interface so the PropertyInfo is bound to a concrete TConfig.
        var configProperty = configInterface.GetProperty(nameof(IPipelineAction<object>.Config));
        return configProperty?.GetValue(action) ?? new object();
    }
}
