﻿using Mutagen.Bethesda.Plugins;
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
