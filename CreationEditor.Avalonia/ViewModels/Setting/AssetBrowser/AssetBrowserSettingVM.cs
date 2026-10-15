using CreationEditor.Avalonia.Models.Settings.AssetBrowser;
using CreationEditor.Services.Integrations;
using CreationEditor.Services.Integrations.Textures;
using CreationEditor.Services.Lifecycle;
using CreationEditor.Services.Settings;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Setting.AssetBrowser;

public sealed partial class AssetBrowserSettingVM : ViewModel, ISettingNode, ILifecycleTask {
    public string Name => "Asset Browser";
    public Type? Parent => null;
    public List<ISettingNode> Children { get; } = [];

    public ISettingModel Model => Setting;
    public AssetBrowserSetting Setting { get; }

    public IObservableCollection<IIntegration> TextureInfoIntegrations { get; } = new ObservableCollectionExtended<IIntegration>();

    [Reactive] public partial IIntegration? SelectedTextureInfoIntegration { get; set; }

    private readonly IIntegrationRegistry _integrationRegistry;

    public AssetBrowserSettingVM(
        ISettingImporter<AssetBrowserSetting> settingsImporter,
        IIntegrationRegistry integrationRegistry) {
        _integrationRegistry = integrationRegistry;
        Setting = settingsImporter.Import(this) ?? new AssetBrowserSetting();

        RefreshIntegrations();

        SelectedTextureInfoIntegration = TextureInfoIntegrations.FirstOrDefault(i => i.Descriptor.Id == Setting.TextureInfoIntegrationId)
         ?? TextureInfoIntegrations.FirstOrDefault();

        this.WhenAnyValue(x => x.SelectedTextureInfoIntegration)
            .Subscribe(integration => Setting.TextureInfoIntegrationId = integration?.Descriptor.Id)
            .DisposeWith(this);

        _integrationRegistry.IntegrationsChanged
            .Subscribe(_ => RefreshIntegrations())
            .DisposeWith(this);
    }

    private void RefreshIntegrations() {
        var integrations = _integrationRegistry.GetIntegrationsForCapability(typeof(ITextureInfoProvider));
        TextureInfoIntegrations.Load(integrations);

        // Keep the selection valid, fall back to the first available integration
        if (SelectedTextureInfoIntegration is not null && integrations.Any(i => i.Descriptor.Id == SelectedTextureInfoIntegration.Descriptor.Id)) return;

        SelectedTextureInfoIntegration = integrations.FirstOrDefault();
    }

    public void PreStartup() {}

    public void PostStartupAsync(CancellationToken token) {
        Apply();
    }

    public void OnExit() {}

    public void Apply() {}
}
