using System.Collections;
using Noggog;
namespace CreationEditor;

public static class EnumerableExtension {
    public static bool CountIsExactly(this IEnumerable enumerable, int count) {
        var counter = 0;
        foreach (var _ in enumerable) {
            counter++;
            if (counter > count) return false;
        }

        return counter == count;
    }

    public static bool CountIsLessThan(this IEnumerable enumerable, int count) {
        var counter = 0;
        foreach (var _ in enumerable) {
            counter++;
            if (counter >= count) return false;
        }

        return true;
    }
    
    public static bool CountIsGreaterThan(this IEnumerable enumerable, int count) {
        var counter = 0;
        foreach (var _ in enumerable) {
            counter++;
            if (counter > count) return true;
        }

        return false;
    }
    
    public static IEnumerable<T1> Keys<T1, T2>(this IEnumerable<IKeyValue<T1,T2>> enumerable) {
        return enumerable.Select(x => x.Key);
    }
    
    public static IEnumerable<T2> Values<T1, T2>(this IEnumerable<IKeyValue<T1,T2>> enumerable) {
        return enumerable.Select(x => x.Value);
    }
}
