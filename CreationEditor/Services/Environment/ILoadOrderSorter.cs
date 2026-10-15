using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Services.Environment;

public interface ILoadOrderSorter {
    /// <summary>
    /// Sorts mod keys so that master plugins are loaded before other plugins
    /// and the masters of every mod are loaded before the mod itself.
    /// Mods that cannot be ordered keep their relative order and are reported as issues.
    /// </summary>
    /// <param name="modKeys">Mod keys to sort, in their current load order.</param>
    /// <param name="getDirectMasters">Returns the direct masters of a mod. Masters not contained in <paramref name="modKeys"/> are ignored.</param>
    /// <returns>The sorted load order and the issues detected while sorting.</returns>
    LoadOrderSortResult Sort(IReadOnlyList<ModKey> modKeys, Func<ModKey, IEnumerable<ModKey>> getDirectMasters);
}
