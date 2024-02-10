using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Avalonia.Models.Mod;

public sealed class OrderedModItem(ModKey modKey, uint loadOrderIndex) : ModItem(modKey) {
    public uint LoadOrderIndex { get; } = loadOrderIndex;
}
