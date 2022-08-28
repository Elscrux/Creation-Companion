using System;
using System.Collections.Generic;
using Mutagen.Bethesda.Plugins;
using MutagenLibrary.WPF.UtilityTypes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.GUI.Models; 

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
