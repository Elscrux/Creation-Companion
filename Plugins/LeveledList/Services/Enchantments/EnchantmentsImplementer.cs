using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using LeveledList.Model.Enchantments;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace LeveledList.Services.Enchantments;

public sealed class EnchantmentsImplementer(
    IEditorEnvironment editorEnvironment,
    ITierController tierController,
    IRecordController recordController) {

    public IReadOnlyList<IEnchantable> ImplementEnchantments(IMod mod, IReadOnlyList<EnchantedItem> enchantedItems) {
        var enchantables = new List<IEnchantable>();

        foreach (var enchantedItem in enchantedItems) {
            IEnchantable enchantable;
            if (editorEnvironment.LinkCache.TryResolve<IEnchantableGetter>(enchantedItem.EditorID, out var enchantableGetter)) {
                enchantable = recordController.GetOrAddOverride<IEnchantable, IEnchantableGetter>(enchantableGetter, mod);
            } else {
                enchantable = recordController.DuplicateRecord<IEnchantable, IEnchantableGetter>(enchantedItem.Enchantable, mod);
            }

            enchantable.EditorID = enchantedItem.EditorID;
            enchantable.ObjectEffect.SetTo(enchantedItem.Enchantment);

            // Only weapons need to have the enchantment amount set
            if (enchantable is IWeaponGetter) {
                // Hard coding the enchantment amount to 500 per level as per Skyrim standard
                enchantable.EnchantmentAmount = (ushort) (enchantedItem.EnchantmentLevel * 500);
            }

            tierController.SetRecordTier(enchantable, enchantedItem.Tier);

            enchantables.Add(enchantable);
        }

        return enchantables;
    }
}
