using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Services.Mutagen.Mod;

public record struct ModInfo(
    ModKey ModKey,
    string? Author,
    string? Description,
    bool Localization,
    int FormVersion,
    ModKey[] DirectMasters) {
    public ModInfo(ModKey modKey) : this(modKey, null, null, false, -1, []) {}
}
