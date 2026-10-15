using System.Reactive.Linq;
using CreationEditor.Avalonia.ViewModels.Integrations;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using CreationEditor.Services.Integrations;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Config.Options;

public sealed partial class IntegrationConfigOptionVM : ViewModel, IConfigOptionVM {
    private readonly IIntegrationRegistry _registry;
    private readonly IConfigVMFactory _configVMFactory;

    public IConfigOption Option => IntegrationOption;
    public IntegrationConfigOption IntegrationOption { get; }

    [Reactive] public partial bool IsActive { get; set; }

    [Reactive] public partial IntegrationConfiguration? Integration { get; set; }

    public IObservable<object?> ValueChanged => this.WhenAnyValue(x => x.Integration);

    public object? Value {
        get => Integration;
        set {
            if (value is null or IntegrationConfiguration) {
                Integration = (IntegrationConfiguration?) value;
                SelectedIntegration = AvailableIntegrations.FirstOrDefault(i => i.Descriptor.Id == Integration?.IntegrationId);
            }
        }
    }

    public IObservable<ConfigOptionValidationError?> ValidationErrorChanged => this.WhenAnyValue(x => x.Integration)
        .Select(s => {
            if (s is null) return null;

            return IntegrationOption?.Validate?.Invoke(s);
        });

    public ObservableCollectionExtended<IntegrationItemVM> AvailableIntegrations { get; } = [];

    [Reactive] public partial IntegrationItemVM? SelectedIntegration { get; set; }
    [Reactive] public partial ConfigVM? IntegrationConfigVM { get; set; }

    public IntegrationConfigOptionVM(
        IntegrationConfigOption option,
        IIntegrationRegistry registry,
        IConfigVMFactory configVMFactory,
        ConfigValues? configValues = null) {
        _registry = registry;
        _configVMFactory = configVMFactory;
        IntegrationOption = option;
        IsActive = !option.IsOptional;

        var selectedIntegrationId = (option.DefaultObjectValue as IntegrationConfiguration)?.IntegrationId;
        RefreshIntegrations(selectedIntegrationId);

        registry.IntegrationsChanged
            .Subscribe(_ => RefreshIntegrations(SelectedIntegration?.Descriptor.Id))
            .DisposeWith(this);

        this.WhenAnyValue(x => x.SelectedIntegration)
            .Subscribe(integration => {
                UpdateIntegrationConfigVM(integration, Integration?.ConfigValues ?? configValues);
                Integration = integration is null
                    ? null
                    : new IntegrationConfiguration {
                        IntegrationId = integration.Descriptor.Id,
                        ConfigValues = IntegrationConfigVM?.GetConfigValues() ?? new ConfigValues(),
                    };
            })
            .DisposeWith(this);
    }

    private void RefreshIntegrations(Guid? selectedIntegrationId = null) {
        AvailableIntegrations.Clear();

        foreach (var integration in _registry.GetIntegrationsForCapability(IntegrationOption.IntegrationCapabilityType)) {
            AvailableIntegrations.Add(new IntegrationItemVM(integration.Descriptor, null, _registry));
        }

        SelectedIntegration = AvailableIntegrations.FirstOrDefault(i => i.Descriptor.Id == selectedIntegrationId) ?? AvailableIntegrations.FirstOrDefault();
    }

    private void UpdateIntegrationConfigVM(IntegrationItemVM? item, ConfigValues? configValues) {
        if (item?.Descriptor.RuntimeConfigDefinition is null) return;

        IntegrationConfigVM?.Dispose();

        IntegrationConfigVM = _configVMFactory.Create(item.Descriptor.RuntimeConfigDefinition, configValues);
        IntegrationConfigVM.ConfigChanged
            .Subscribe(_ => {
                Integration = new IntegrationConfiguration {
                    IntegrationId = item.Descriptor.Id,
                    ConfigValues = IntegrationConfigVM?.GetConfigValues() ?? new ConfigValues(),
                };
            })
            .DisposeWith(IntegrationConfigVM);
    }
}
