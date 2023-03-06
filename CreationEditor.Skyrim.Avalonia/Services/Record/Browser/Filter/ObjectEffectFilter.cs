using System.Collections.Generic;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public class ObjectEffectFilter : SimpleRecordFilter<IObjectEffectGetter> {
    public ObjectEffectFilter() : base(new List<SimpleRecordFilterEntry> {
        new("Generic Enchantment", record => record.EnchantType == ObjectEffect.EnchantTypeEnum.Enchantment),
        new("Staff Enchantment", record => record.EnchantType == ObjectEffect.EnchantTypeEnum.StaffEnchantment),
    }) {}
}
