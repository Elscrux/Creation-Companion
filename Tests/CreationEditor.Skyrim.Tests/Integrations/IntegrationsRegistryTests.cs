using System.IO.Abstractions.TestingHelpers;
using Autofac;
using CreationEditor.Services.Integrations;
using CreationEditor.Services.Serialization.Json;
using CreationEditor.Services.Settings;
using Newtonsoft.Json.Serialization;
using Serilog.Core;
using ConfigValues = System.Collections.Generic.Dictionary<string, object>;

namespace CreationEditor.Skyrim.Tests.Integrations;

file interface ITestCapability : IIntegration {}
file interface IOtherCapability : IIntegration {}

sealed file class TestIntegration(ConfigValues? configValues = null) : ITestCapability {
    public IntegrationDescriptor Descriptor { get; set; } = null!;
    public ConfigValues? ReceivedConfigValues { get; } = configValues;
}

sealed file class OtherIntegration(IntegrationDescriptor descriptor) : IOtherCapability {
    public IntegrationDescriptor Descriptor { get; } = descriptor;
}

public sealed class IntegrationProviderTests {
    private static readonly Guid TestId = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid OtherId = new("22222222-2222-2222-2222-222222222222");

    private static IntegrationDescriptor CreateDescriptor(Guid id, Type capabilityType) => new(id, "Name", "Description", "Category", capabilityType, null, null, []);

    private static IntegrationProvider CreateProvider() => new([
        CreateDescriptor(TestId, typeof(ITestCapability)),
        CreateDescriptor(OtherId, typeof(IOtherCapability)),
    ]);

    [Fact]
    public void GetDescriptor_KnownId_ReturnsDescriptor() {
        var provider = CreateProvider();

        var descriptor = provider.GetDescriptor(TestId);

        Assert.Equal(TestId, descriptor.Id);
        Assert.Equal(typeof(ITestCapability), descriptor.CapabilityType);
    }

    [Fact]
    public void GetDescriptor_UnknownId_ThrowsArgumentException() {
        var provider = CreateProvider();

        Assert.Throws<ArgumentException>(() => provider.GetDescriptor(Guid.NewGuid()));
    }

    [Fact]
    public void GetDescriptors_ByCapabilityType_FiltersDescriptors() {
        var provider = CreateProvider();

        var descriptors = provider.GetDescriptors(typeof(ITestCapability));

        var descriptor = Assert.Single(descriptors);
        Assert.Equal(TestId, descriptor.Id);
    }

    [Fact]
    public void IntegrationTypes_AreDistinctCapabilityTypes() {
        var provider = CreateProvider();

        Assert.Equal([typeof(ITestCapability), typeof(IOtherCapability)], provider.IntegrationTypes);
    }
}

public sealed class IntegrationFactoryTests {
    private static readonly Guid TestId = new("33333333-3333-3333-3333-333333333333");

    private static IntegrationDescriptor CreateDescriptor() => new(TestId, "Name", "Description", "Category", typeof(ITestCapability), null, null, []);

    [Fact]
    public void CreateIntegration_ResolvesKeyedRegistration() {
        var builder = new ContainerBuilder();
        builder.RegisterType<TestIntegration>().Keyed<IIntegration>(TestId);
        var scope = builder.Build();
        var factory = new IntegrationFactory(scope);

        var integration = factory.CreateIntegration(CreateDescriptor());

        Assert.IsType<TestIntegration>(integration);
    }

    [Fact]
    public void CreateIntegration_WithConfigValues_PassesConfigToIntegration() {
        var builder = new ContainerBuilder();
        builder.RegisterType<TestIntegration>().Keyed<IIntegration>(TestId);
        var scope = builder.Build();
        var factory = new IntegrationFactory(scope);
        var configValues = new ConfigValues { ["key"] = "value" };

        var integration = factory.CreateIntegration(CreateDescriptor(), configValues);

        var testIntegration = Assert.IsType<TestIntegration>(integration);
        Assert.Same(configValues, testIntegration.ReceivedConfigValues);
    }

    [Fact]
    public void CreateIntegration_UnregisteredId_Throws() {
        using var scope = new ContainerBuilder().Build();
        var factory = new IntegrationFactory(scope);

        Assert.Throws<InvalidOperationException>(() => factory.CreateIntegration(CreateDescriptor()));
    }
}

public sealed class IntegrationsRegistryTests {
    private static readonly Guid TestId = new("44444444-4444-4444-4444-444444444444");
    private static readonly Guid OtherId = new("55555555-5555-5555-5555-555555555555");

    private static IntegrationDescriptor CreateDescriptor(Guid id, Type capabilityType) => new(id, "Name", "Description", "Category", capabilityType, null, null, []);

    private static (IntegrationsRegistry Registry, MockFileSystem FileSystem) CreateRegistry() {
        var fileSystem = new MockFileSystem();
        var pathProvider = new SettingPathProvider(fileSystem);
        var jsonSettings = new JsonSerializerSettingsProvider(new DefaultContractResolver());
        var exporter = new JsonSettingExporter(Logger.None, fileSystem, pathProvider, jsonSettings);
        var importer = new JsonSettingImporter<IntegrationsDto>(Logger.None, fileSystem, pathProvider, jsonSettings);

        var provider = new IntegrationProvider([
            CreateDescriptor(TestId, typeof(ITestCapability)),
            CreateDescriptor(OtherId, typeof(IOtherCapability)),
        ]);

        var builder = new ContainerBuilder();
        builder.Register(_ => new TestIntegration { Descriptor = provider.GetDescriptor(TestId) }).Keyed<IIntegration>(TestId);
        builder.Register(_ => new OtherIntegration(provider.GetDescriptor(OtherId))).Keyed<IIntegration>(OtherId);
        var scope = builder.Build();
        var factory = new IntegrationFactory(scope);

        return (new IntegrationsRegistry(importer, provider, factory, exporter), fileSystem);
    }

    [Fact]
    public void Constructor_WithoutSavedSettings_HasNoIntegrations() {
        var (registry, _) = CreateRegistry();

        Assert.Empty(registry.Integrations);
        Assert.Empty(registry.IntegrationRegistrations);
    }

    [Fact]
    public void Register_AddsIntegrationAndRegistration() {
        var (registry, fileSystem) = CreateRegistry();

        registry.Register(new IntegrationRegistration { IntegrationId = TestId });

        var integration = Assert.Single(registry.Integrations);
        Assert.Equal(TestId, integration.Descriptor.Id);
        Assert.NotNull(registry.TryGetIntegration(TestId));
        Assert.True(fileSystem.File.Exists(fileSystem.Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Settings",
            "Integrations.json")));
    }

    [Fact]
    public void Register_SameIdTwice_ReplacesRegistration() {
        var (registry, _) = CreateRegistry();

        registry.Register(new IntegrationRegistration { IntegrationId = TestId, InstalledVersion = "1.0" });
        registry.Register(new IntegrationRegistration { IntegrationId = TestId, InstalledVersion = "2.0" });

        Assert.Single(registry.Integrations);
        var registration = Assert.Single(registry.IntegrationRegistrations);
        Assert.Equal("2.0", registration.InstalledVersion);
    }

    [Fact]
    public void Unregister_RemovesIntegration() {
        var (registry, _) = CreateRegistry();
        registry.Register(new IntegrationRegistration { IntegrationId = TestId });

        registry.Unregister(TestId);

        Assert.Empty(registry.Integrations);
        Assert.Null(registry.TryGetIntegration(TestId));
    }

    [Fact]
    public void GetIntegrationsForCapability_ReturnsMatchingIntegrations() {
        var (registry, _) = CreateRegistry();
        registry.Register(new IntegrationRegistration { IntegrationId = TestId });
        registry.Register(new IntegrationRegistration { IntegrationId = OtherId });

        var integrations = registry.GetIntegrationsForCapability(typeof(ITestCapability));

        var integration = Assert.Single(integrations);
        Assert.Equal(TestId, integration.Descriptor.Id);
    }

    [Fact]
    public void IntegrationsChanged_FiresOnRegisterAndUnregister() {
        var (registry, _) = CreateRegistry();
        var changeCount = 0;
        using var subscription = registry.IntegrationsChanged.Subscribe(_ => changeCount++);

        registry.Register(new IntegrationRegistration { IntegrationId = TestId });
        registry.Unregister(TestId);

        Assert.Equal(2, changeCount);
    }

    [Fact]
    public void Constructor_WithSavedSettings_RehydratesIntegrations() {
        var (registry, fileSystem) = CreateRegistry();
        registry.Register(new IntegrationRegistration { IntegrationId = TestId, InstalledVersion = "1.0" });

        // A second registry over the same settings store rehydrates the registered integration
        var pathProvider = new SettingPathProvider(fileSystem);
        var jsonSettings = new JsonSerializerSettingsProvider(new DefaultContractResolver());
        var exporter = new JsonSettingExporter(Logger.None, fileSystem, pathProvider, jsonSettings);
        var importer = new JsonSettingImporter<IntegrationsDto>(Logger.None, fileSystem, pathProvider, jsonSettings);
        var provider = new IntegrationProvider([
            CreateDescriptor(TestId, typeof(ITestCapability)),
            CreateDescriptor(OtherId, typeof(IOtherCapability)),
        ]);
        var builder = new ContainerBuilder();
        builder.Register(_ => new TestIntegration { Descriptor = provider.GetDescriptor(TestId) }).Keyed<IIntegration>(TestId);
        builder.Register(_ => new OtherIntegration(provider.GetDescriptor(OtherId))).Keyed<IIntegration>(OtherId);
        var scope = builder.Build();
        var secondRegistry = new IntegrationsRegistry(importer, provider, new IntegrationFactory(scope), exporter);

        var integration = Assert.Single(secondRegistry.Integrations);
        Assert.Equal(TestId, integration.Descriptor.Id);
        var registration = Assert.Single(secondRegistry.IntegrationRegistrations);
        Assert.Equal("1.0", registration.InstalledVersion);
    }
}
