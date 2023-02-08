using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord; 

public sealed class EditableCondition : Condition, INotifyPropertyChanged {
    public override ConditionData Data { get; set; }
    
    public bool Or {
        get => (Flags & Flag.OR) != 0;
        set {
            Flags = Flags.SetFlag(Flag.OR, value);
            OnPropertyChanged();
        }
    }
    
    public bool UseGlobal {
        get => (Flags & Flag.UseGlobal) != 0;
        set {
            Flags = Flags.SetFlag(Flag.UseGlobal, value);
            OnPropertyChanged();
        }
    }
    
    public bool ParametersUseAliases {
        get => (Flags & Flag.ParametersUseAliases) != 0;
        set {
            Flags = Flags.SetFlag(Flag.ParametersUseAliases, value);
            OnPropertyChanged();
        }
    }
    
    public bool UsePackData {
        get => (Flags & Flag.UsePackData) != 0;
        set {
            Flags = Flags.SetFlag(Flag.UsePackData, value);
            OnPropertyChanged();
        }
    }
    
    public bool SwapSubjectAndTarget {
        get => (Flags & Flag.SwapSubjectAndTarget) != 0;
        set {
            Flags = Flags.SetFlag(Flag.SwapSubjectAndTarget, value);
            OnPropertyChanged();
        }
    }
    
    public EditableCondition(ICondition parent) {
        Data = parent.Data;
    }

    public void SetCondition(Condition condition) {
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
