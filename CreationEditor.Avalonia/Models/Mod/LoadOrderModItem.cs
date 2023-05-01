using Mutagen.Bethesda.Plugins;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Mod;

public sealed class LoadOrderModItem : ModItem, IDisposableDropoff {
    public uint LoadOrderIndex { get; }
    [Reactive] public bool IsActive { get; set; }
    [Reactive] public bool MastersValid { get; set; }

    public LoadOrderModItem(ModKey modKey, bool mastersValid, uint loadOrderIndex) : base(modKey) {
        MastersValid = mastersValid;
        LoadOrderIndex = loadOrderIndex;

        this.WhenAnyValue(x => x.IsSelected)
            .ObserveOnGui()
            .Subscribe(isSelected => {
                if (!isSelected) IsActive = false;
            });

        this.WhenAnyValue(x => x.IsActive)
            .ObserveOnGui()
            .Subscribe(isActive => {
                if (isActive) IsSelected = true;
            });
    }
}
