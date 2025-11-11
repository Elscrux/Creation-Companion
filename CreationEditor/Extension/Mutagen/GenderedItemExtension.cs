using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor;

public static class GenderedItemExtension {
    extension<T>(IGenderedItemGetter<T> genderedItem) {
        public string GetName(Func<T, string?> stringSelector) {
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
}
