using System.Reactive.Linq;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using Mutagen.Bethesda.Plugins;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Config.Options;

public sealed partial class ModKeyConfigOptionVM(ModKeyConfigOption option, SingleModPickerVM singleModPickerVM) : ViewModel, IConfigOptionVM {
    public IConfigOption Option => ModKeyOption;
    public ModKeyConfigOption ModKeyOption { get; } = option;

    public SingleModPickerVM ModPicker { get; } = singleModPickerVM;

    [Reactive] public partial bool IsActive { get; set; } = !option.IsOptional;

    [Reactive] public partial ModKey ModKey { get; set; } = option.DefaultValue;

    public IObservable<object?> ValueChanged => this.WhenAnyValue(x => x.ModKey).Select(x => (object?) x);

    public object? Value {
        get => ModKey;
        set {
            if (value is ModKey modKey) {
                ModKey = modKey;
            }
        }
    }

    public IObservable<ConfigOptionValidationError?> ValidationErrorChanged => this.WhenAnyValue(x => x.ModKey)
        .Select(s => ModKeyOption?.Validate?.Invoke(s));
}
