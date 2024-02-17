using CreationEditor.Avalonia.Models.Selectables;
using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Mod;

public sealed class LoadOrderModItem : ReactiveObject, IReactiveSelectable, IModKeyed, IDisposableDropoff {
    private readonly DisposableBucket _disposables = new();

    public uint LoadOrderIndex { get; }
    public ModInfo ModInfo { get; }
    public ModKey ModKey => ModInfo.ModKey;

    [Reactive] public bool IsSelected { get; set; }
    [Reactive] public bool IsActive { get; set; }
    [Reactive] public bool MastersValid { get; set; }

    public LoadOrderModItem(ModInfo modInfo, bool mastersValid, uint loadOrderIndex) {
        ModInfo = modInfo;
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
