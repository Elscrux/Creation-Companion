using Mutagen.Bethesda.Plugins;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Mod;

public class ActivatableModItem : ModItem {
    [Reactive] public bool IsActive { get; set; }
    [Reactive] public bool MastersValid { get; set; }

    public ActivatableModItem(ModKey modKey, bool mastersValid) : base(modKey) {
        MastersValid = mastersValid;
        
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
