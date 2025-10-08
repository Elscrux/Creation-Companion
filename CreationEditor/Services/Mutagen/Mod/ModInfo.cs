using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Services.Mutagen.Mod;

public sealed record ModInfo(
    ModKey ModKey,
    string? Author,
    string? Description,
    bool Localization,
    int FormVersion,
    ModKey[] Masters) {
    public ModInfo(ModKey modKey) : this(modKey, null, null, false, -1, []) {}
}
