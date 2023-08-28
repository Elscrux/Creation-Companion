using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor;

public static class GenderedItemExtension {
    public static string GetName<T>(this IGenderedItemGetter<T> genderedItem, Func<T, string?> stringSelector) {
        var maleString = stringSelector(genderedItem.Male);
        var femaleString = stringSelector(genderedItem.Female);

        if (maleString.IsNullOrWhitespace()) {
            if (femaleString.IsNullOrWhitespace()) {
                return string.Empty;
            }
            
            return femaleString;
        }

        if (femaleString.IsNullOrWhitespace()) return maleString;

        return string.Equals(maleString, femaleString)
            ? maleString
            : $"{maleString} / {femaleString}";
    }
}
