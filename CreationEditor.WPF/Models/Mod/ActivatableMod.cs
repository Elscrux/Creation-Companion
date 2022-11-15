using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.WPF.Models.Mod; 

public class ModItem : ReactiveObject, ISelectable {
    public static readonly ModKey NewMod = new();

    public ModItem(ModKey modKey) => ModKey = modKey;

    [Reactive]
    public bool IsSelected { get; set; }
    
    [Reactive]
    public ModKey ModKey { get; protected set; }

    public override string ToString() => ModKey == NewMod ? "< new plugin >" : ModKey.FileName;
}

public class ActivatableModItem : ModItem {
    [Reactive] public bool IsActive { get; set; }
    [Reactive] public bool MastersValid { get; set; }
    [Reactive] public HashSet<ModKey> Masters { get; set; }

    public ActivatableModItem(ModKey modKey, bool mastersValid, HashSet<ModKey> masters) : base(modKey) {
        MastersValid = mastersValid;
        Masters = masters;
        
        this.WhenAnyValue(x => x.IsSelected)
            .Subscribe(_ => {
                if (!IsSelected) IsActive = false;
            });
        
        this.WhenAnyValue(x => x.IsActive)
            .Subscribe(_ => {
                if (IsActive) IsSelected = true;
            });
    }
}
