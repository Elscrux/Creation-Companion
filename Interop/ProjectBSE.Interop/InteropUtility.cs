using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using Mutagen.Bethesda.Plugins;
namespace ProjectBSE.Interop;

public static class InteropUtility {
    public static FormKey[] ToFormKeyArray(this IntPtr pointer, int count) {
        var managedArray = new FormKey[count];
        for (var i = 0; i < count; i++) {
            var ptr = Marshal.ReadIntPtr(pointer, i * IntPtr.Size);
            var str = Marshal.PtrToStringAnsi(ptr);
            managedArray[i] = string.IsNullOrWhiteSpace(str) ? FormKey.Null : FormKey.Factory(str);
        }
        return managedArray;
    }

    public static string?[] ToStringArray(this IntPtr pointer, int count) {
        var result = new string?[count];

        for (var i = 0; i < count; i++) {
            var ptr = Marshal.ReadIntPtr(pointer, i * IntPtr.Size);
            result[i] = Marshal.PtrToStringAnsi(ptr);
        }

        return result;
    }

    public static string[] ToStringArray(this IList<FormKey> formKeys) {
        var strings = new string[formKeys.Count];
        for (var i = 0; i < formKeys.Count; i++) {
            strings[i] = formKeys[i].ToString();
        }
        return strings;
    }

    public static T?[] ToArray<T>(this IntPtr pointer, int count) {
        var result = new T?[count];

        var sizeOf = Marshal.SizeOf<T>();
        for (var i = 0; i < count; i++) {
            var ptr = Marshal.ReadIntPtr(pointer, i * sizeOf);
            result[i] = Marshal.PtrToStructure<T>(ptr);
        }

        return result;
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

    public static IntPtr ToUnmanagedMemory<T>(this IList<T> list)
        where T : notnull {
        var sizeOf = Marshal.SizeOf<T>();
        var pointer = Marshal.AllocCoTaskMem(sizeOf * list.Count);
        for (var i = 0; i < list.Count; i++) {
            var itemPtr = new IntPtr(pointer + i * sizeOf);
            Marshal.StructureToPtr(list[i], itemPtr, false);
        }
        return pointer;
    }

    public static IDisposable ToUnmanagedMemory<T>(this IList<T> list, out IntPtr pointer)
        where T : notnull {
        var sizeOf = Marshal.SizeOf<T>();
        pointer = Marshal.AllocCoTaskMem(sizeOf * list.Count);
        for (var i = 0; i < list.Count; i++) {
            var itemPtr = new IntPtr(pointer + i * sizeOf);
            Marshal.StructureToPtr(list[i], itemPtr, false);
        }

        var clearPointer = pointer;
        return Disposable.Create(() => Marshal.FreeCoTaskMem(clearPointer));
    }
}
