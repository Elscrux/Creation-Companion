using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using Mutagen.Bethesda.Plugins;
namespace ProjectBSE.Interop;

public static class InteropUtility {
    extension(IntPtr pointer) {
        public FormKey[] ToFormKeyArray(int count) {
            var managedArray = new FormKey[count];
            for (var i = 0; i < count; i++) {
                var ptr = Marshal.ReadIntPtr(pointer, i * IntPtr.Size);
                var str = Marshal.PtrToStringAnsi(ptr);
                managedArray[i] = string.IsNullOrWhiteSpace(str) ? FormKey.Null : FormKey.Factory(str);
            }
            return managedArray;
        }
        public string[] ToStringArray(int count) {
            var result = new string[count];

            for (var i = 0; i < count; i++) {
                var ptr = Marshal.ReadIntPtr(pointer, i * IntPtr.Size);
                result[i] = Marshal.PtrToStringAnsi(ptr) ?? string.Empty;
            }

            return result;
        }
        public T?[] ToArray<T>(int count) {
            var result = new T?[count];

            var sizeOf = Marshal.SizeOf<T>();
            for (var i = 0; i < count; i++) {
                var ptr = pointer + i * sizeOf;
                result[i] = Marshal.PtrToStructure<T>(ptr);
            }

            return result;
        }
    }

    extension(IReadOnlyList<FormKey> formKeys) {
        public string[] ToStringArray() {
            var strings = new string[formKeys.Count];
            for (var i = 0; i < formKeys.Count; i++) {
                strings[i] = formKeys[i].ToString();
            }
            return strings;
        }
    }

    public static unsafe T[] ToArray<T>(T* pointer, int count)
        where T : unmanaged {
        var result = new T[count];
        for (var i = 0; i < count; i++) {
            result[i] = pointer[i];
        }

        return result;
    }

    public static unsafe TManaged[] ToArray<TUnmanaged, TManaged>(TUnmanaged* pointer, int count, Func<TUnmanaged, TManaged> conv)
        where TUnmanaged : unmanaged {
        var result = new TManaged[count];
        for (var i = 0; i < count; i++) {
            result[i] = conv(pointer[i]);
        }

        return result;
    }

    extension<T>(IList<T> list)
        where T : notnull {
        public IntPtr ToUnmanagedMemory() {
            var sizeOf = Marshal.SizeOf<T>();
            var pointer = Marshal.AllocCoTaskMem(sizeOf * list.Count);
            for (var i = 0; i < list.Count; i++) {
                var itemPtr = new IntPtr(pointer + i * sizeOf);
                Marshal.StructureToPtr((object) list[i], itemPtr, false);
            }
            return pointer;
        }
        public IDisposable ToUnmanagedMemory(out IntPtr pointer) {
            var sizeOf = Marshal.SizeOf<T>();
            pointer = Marshal.AllocCoTaskMem(sizeOf * list.Count);
            for (var i = 0; i < list.Count; i++) {
                var itemPtr = new IntPtr(pointer + i * sizeOf);
                Marshal.StructureToPtr((object) list[i], itemPtr, false);
            }

            var clearPointer = pointer;
            return Disposable.Create(() => Marshal.FreeCoTaskMem(clearPointer));
        }
    }

}
