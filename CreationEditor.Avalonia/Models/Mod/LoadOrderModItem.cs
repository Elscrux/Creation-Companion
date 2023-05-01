using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Mod;

public sealed class LoadOrderModItem : ModItem, IDisposableDropoff {
    public uint LoadOrderIndex { get; }
    private readonly IDisposableDropoff _disposables = new DisposableBucket();
    [Reactive] public bool IsActive { get; set; }
    [Reactive] public bool MastersValid { get; set; }

    public LoadOrderModItem(ModKey modKey, bool mastersValid, uint loadOrderIndex) : base(modKey) {
        MastersValid = mastersValid;
        LoadOrderIndex = loadOrderIndex;

        this.WhenAnyValue(x => x.IsSelected)
            .ObserveOnGui()
            .Subscribe(isSelected => {
                if (!isSelected) IsActive = false;
            })
            .DisposeWith(this);

        this.WhenAnyValue(x => x.IsActive)
            .ObserveOnGui()
            .Subscribe(isActive => {
                if (isActive) IsSelected = true;
            })
            .DisposeWith(this);
    }

    public void Dispose() => _disposables.Dispose();
    public void Add(IDisposable disposable) => _disposables.Add(disposable);
}
