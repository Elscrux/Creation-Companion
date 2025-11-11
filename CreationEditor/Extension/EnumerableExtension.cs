using System.Collections;
using Noggog;
namespace CreationEditor;

public static class EnumerableExtension {
    extension(IEnumerable enumerable) {
        public bool CountIsExactly(int count) {
            var counter = 0;
            foreach (var _ in enumerable) {
                counter++;
                if (counter > count) return false;
            }

            return counter == count;
        }
        public bool CountIsLessThan(int count) {
            var counter = 0;
            foreach (var _ in enumerable) {
                counter++;
                if (counter >= count) return false;
            }

            return true;
        }
        public bool CountIsGreaterThan(int count) {
            var counter = 0;
            foreach (var _ in enumerable) {
                counter++;
                if (counter > count) return true;
            }

            return false;
        }
    }

    extension<T1, T2>(IEnumerable<IKeyValue<T1, T2>> enumerable) {
        public IEnumerable<T1> Keys() {
            return enumerable.Select(x => x.Key);
        }
        public IEnumerable<T2> Values() {
            return enumerable.Select(x => x.Value);
        }
    }
}
