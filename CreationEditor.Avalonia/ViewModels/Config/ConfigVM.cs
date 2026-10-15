using System.Reactive;
using System.Reactive.Linq;
using Autofac;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using CreationEditor.Services.Integrations;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Config;

public sealed partial class ConfigVM : ViewModel {
    private readonly ILifetimeScope _lifetimeScope;

    public IObservable<Unit> ConfigChanged => OptionVMs
        .Select(vm => vm.WhenAnyValue(x => x.IsActive).Unit()
            .Merge(vm.ValueChanged.Unit()))
        .Merge();

    public IReadOnlyList<IConfigOptionVM> OptionVMs { get; }

    public ConfigVM(
        ILifetimeScope lifetimeScope,
        IConfigOptionAutoDetector autoDetector,
        IConfigDefinition configDefinition,
        ConfigValues configValues) {
        _lifetimeScope = lifetimeScope;

        // Pre-populate auto-detectable file/folder options that are not set already
        autoDetector.AutoDetect(configDefinition, configValues);

        OptionVMs = configDefinition.Options
            .Select(option => CreateConfigOptionVM(option, configValues))
            .ToArray();
    }

    public IConfigOptionVM CreateConfigOptionVM(IConfigOption configOption, ConfigValues configValues) {
        var optionType = configOption.GetType();

        // Generic option types resolve their VM via the open generic definition's key
        var key = optionType.IsGenericType
            ? optionType.GetGenericTypeDefinition().FullName
            : optionType.FullName;

        // Generic list options are edited through their non-generic list interface
        var parameterType = configOption is IListConfigOption ? typeof(IListConfigOption) : optionType;

        ConfigValues? nestedConfigValues = null;
        if (configValues.TryGet(configOption) is IntegrationConfiguration configuration) {
            nestedConfigValues = configuration.ConfigValues;
        }

        var configOptionVM = _lifetimeScope.ResolveOptionalKeyed<IConfigOptionVM>(
                key ?? throw new InvalidOperationException($"Config option type '{optionType.FullName}' does not have a full name"),
                new TypedParameter(parameterType, configOption),
                new TypedParameter(typeof(ConfigValues), nestedConfigValues))
         ?? throw new InvalidOperationException($"Config option type '{optionType.FullName}' does not have a registered config option VM");

        if (configValues.Has(configOption)) {
            configOptionVM.Value = configValues.Get(configOption);
            configOptionVM.IsActive = true;
        }

        return configOptionVM;
    }

    public ConfigValues GetConfigValues() =>
        OptionVMs
            .Where(vm => vm.IsActive)
            .Where(vm => vm.Value is not null)
            .ToDictionary(vm => vm.Option.Name, vm => vm.Value!);
}
