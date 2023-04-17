using CreationEditor.Avalonia.Converter;
using CreationEditor.Skyrim.Avalonia.Resources.Constants;
using DynamicData;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Resources.Converter;

public static class SkyrimEnumConverters {
    public static readonly ExtendedFuncValueConverter<CompareOperator, string, string> StringToCompareOperator
        = new((compare, _) => {
            var indexOf = EnumConstants.CompareOperatorTypes.IndexOf(compare);
            if (indexOf == -1) return "Not found";

            return EnumConstants.CompareOperatorTypesString[indexOf];
        }, (s, _) => {
            var indexOf = EnumConstants.CompareOperatorTypesString.IndexOf(s);
            if (indexOf == -1) return default;

            return (CompareOperator) indexOf;
        });
}
