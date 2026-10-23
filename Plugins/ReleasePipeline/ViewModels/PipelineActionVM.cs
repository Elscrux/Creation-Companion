using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Config;
using ReactiveUI;
using ReleasePipeline.Models.Actions;
namespace ReleasePipeline.ViewModels;

/// <summary>
/// UI-editable wrapper around an <see cref="IPipelineAction"/>. Exposes the action's
/// configuration for editing inline via an expander. The config view model is created generically
/// from the action's config type, so no specific action types are referenced here.
/// </summary>
public sealed partial class PipelineActionVM : ViewModel {
    public IPipelineAction Action { get; }

    public string DisplayName => Action.DisplayName;
    public string Description => Action.Description;
    public Guid Id => Action.Id;

    /// <summary>The action's config view model, or null if the action has no editable config</summary>
    public ConfigVM? ConfigVM { get; }

    /// <summary>True when the action exposes an editable configuration</summary>
    public bool HasConfig => ConfigVM is not null;

    /// <summary>Emits this VM when the remove command is executed</summary>
    public IObservable<PipelineActionVM> RemoveRequested { get; }

    public ReactiveCommand<Unit, Unit> RemoveCommand { get; }

    public PipelineActionVM(
        IPipelineAction action,
        IConfigVMFactory configVMFactory) {
        Action = action;

        ConfigVM = configVMFactory.Create(action.ConfigDefinition, Action.ConfigValues);

        RemoveCommand = ReactiveCommand.Create(() => {});
        RemoveRequested = RemoveCommand.Select(_ => this);
    }

    public void Save() {
        if (ConfigVM is not null) {
            Action.ConfigValues = ConfigVM.GetConfigValues();
        }
    }
}
