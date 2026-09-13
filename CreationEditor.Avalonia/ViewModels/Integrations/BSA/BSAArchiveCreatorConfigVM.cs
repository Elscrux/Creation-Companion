using CreationEditor.Services.Integrations.BSA;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Integrations.BSA;

/// <summary>
/// View model for editing a <see cref="BSAArchiveCreatorConfig"/>. Manages the settings model
/// object so the tool can be run from CLI without UI.
/// </summary>
public sealed partial class BSAArchiveCreatorConfigVM : ViewModel {
    public BSAArchiveCreatorConfig Config { get; }

    [Reactive] public partial int CompressionLevel { get; set; }
    [Reactive] public partial bool Compress { get; set; }
    [Reactive] public partial bool IncludeSubdirectories { get; set; }

    public BSAArchiveCreatorConfigVM(BSAArchiveCreatorConfig config) {
        Config = config;
        CompressionLevel = config.CompressionLevel;
        Compress = config.Compress;
        IncludeSubdirectories = config.IncludeSubdirectories;

        this.WhenAnyValue(
                x => x.CompressionLevel,
                x => x.Compress,
                x => x.IncludeSubdirectories)
            .Subscribe(_ => {
                config.CompressionLevel = CompressionLevel;
                config.Compress = Compress;
                config.IncludeSubdirectories = IncludeSubdirectories;
            });
    }
}
