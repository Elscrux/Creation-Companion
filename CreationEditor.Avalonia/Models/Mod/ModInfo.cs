using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Avalonia.Models.Mod;

public sealed record ModInfo(
    ModKey ModKey,
    string? Author,
    string? Description,
    bool Localization,
    int FormVersion,
    ModKey[] Masters);
