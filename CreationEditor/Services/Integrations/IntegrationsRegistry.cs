using System.Reactive;
using System.Reactive.Subjects;
using CreationEditor.Services.Settings;
using Noggog;
namespace CreationEditor.Services.Integrations;

/// <summary>
/// Registry for registered integrations, implements <see cref="T:CreationEditor.Services.Integrations.IIntegrationRegistry">IIntegrationRegistry</see>.
/// </summary>
public sealed class IntegrationsRegistry : IIntegrationRegistry, ISetting {
    private readonly IIntegrationProvider _integrationProvider;
    private readonly ISettingExporter _settingExporter;
    private readonly IIntegrationFactory _integrationFactory;
    private readonly List<IntegrationRegistration> _integrationRegistrations;
    private readonly List<IIntegration> _integrations;

    private readonly Subject<Unit> _integrationsChanged = new();

    public IReadOnlyList<IIntegration> Integrations => _integrations;
    public IObservable<Unit> IntegrationsChanged => _integrationsChanged;
    public IReadOnlyList<IntegrationRegistration> IntegrationRegistrations => _integrationRegistrations;

    public string Name => "Integrations";

    public IntegrationsDto Dto { get; }
    public ISettingModel Model => Dto;

    public IntegrationsRegistry(
        ISettingImporter<IntegrationsDto> settingImporter,
        IIntegrationProvider integrationProvider,
        IIntegrationFactory integrationFactory,
        ISettingExporter settingExporter) {
        _integrationFactory = integrationFactory;
        _settingExporter = settingExporter;
        _integrationProvider = integrationProvider;

        Dto = settingImporter.Import(this) ?? new IntegrationsDto();

        _integrations = Dto.Integrations
            .Select(CreateIntegration)
            .ToList();

        _integrationRegistrations = Dto.Integrations;
    }

    private IIntegration CreateIntegration(IntegrationRegistration i) {
        var integrationDescriptor = _integrationProvider.GetDescriptor(i.IntegrationId);
        return _integrationFactory.CreateIntegration(integrationDescriptor, i.RegistrationConfig);
    }

    public void Register(IntegrationRegistration integrationRegistration) {
        // Add integration registration
        var existing = _integrationRegistrations.Find(i => i.IntegrationId == integrationRegistration.IntegrationId);
        if (existing is not null) {
            var index = _integrationRegistrations.IndexOf(existing);
            _integrationRegistrations[index] = integrationRegistration;
        } else {
            _integrationRegistrations.Add(integrationRegistration);
        }

        // Add integration
        var integration = CreateIntegration(integrationRegistration);
        var existingIntegration = _integrations.Find(i => i.Descriptor.Id == integration.Descriptor.Id);
        if (existingIntegration is not null) {
            var index = _integrations.IndexOf(existingIntegration);
            _integrations[index] = integration;
        } else {
            _integrations.Add(integration);
        }

        _integrationsChanged.OnNext(Unit.Default);

        _settingExporter.Export(this);
    }

    public void Unregister(Guid integrationId) {
        _integrationRegistrations.RemoveWhere(i => i.IntegrationId == integrationId);
        _integrations.RemoveWhere(i => i.Descriptor.Id == integrationId);
        _integrationsChanged.OnNext(Unit.Default);

        _settingExporter.Export(this);
    }

    public IIntegration? TryGetIntegration(Guid integrationId) {
        return _integrations.Find(i => i.Descriptor.Id == integrationId);
    }

    public T? TryGetIntegration<T>(Guid integrationId) where T : IIntegration {
        return _integrations.OfType<T>().FirstOrDefault(i => i.Descriptor.Id == integrationId);
    }

    public IReadOnlyList<IIntegration> GetIntegrationsForCapability(Type capabilityType) {
        return _integrations.Where(i => i.Descriptor.CapabilityType == capabilityType).ToArray();
    }

    public void Apply() {
        throw new NotImplementedException();
    }
}
