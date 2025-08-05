using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
namespace ProjectBSE.Interop;

public static partial class Interop {
    [LibraryImport(DllName, EntryPoint = "updateKeybindings")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial void UpdateKeybindings_Native(in IOFunctionBinding[] bindings);
    public static void UpdateKeybindings(Dictionary<IOFunction, IOFunctionBinding> keyBindings) {
        // Add missing keys with default value -1 to ensure all IOFunction enum members are represented
        foreach (var ioFunction in Enum.GetValues<IOFunction>()) {
            keyBindings.TryAdd(ioFunction, new IOFunctionBinding { Type = IOFunctionBindingType.Keyboard, Key = -1 });
        }

        var ioFunctionBindings = keyBindings.Select(x => x.Value).ToArray();
        UpdateKeybindings_Native(ioFunctionBindings);
    }

    [LibraryImport(DllName, EntryPoint = "getKeybindings")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial IntPtr GetKeyBindings_Native();
    public static IOFunctionBinding[] GetKeyBindings() {
        var pointer = GetKeyBindings_Native();
        var ioFunctionBindings = pointer.ToArray<IOFunctionBinding>(IOFunctionCount);
        return ioFunctionBindings;
    }

    [LibraryImport(DllName, EntryPoint = "enumerateKeyBindingNames", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial KeyBindingList EnumerateKeyBindingNames_Native();
    public static string[] EnumerateKeyBindingNames() {
        var x = EnumerateKeyBindingNames_Native();
        return x.Names;
    }

    public enum IOFunctionBindingType : uint {
        Keyboard,
        Mouse,
        Scroll,
        None,
    }

    private const int IOFunctionCountConst = 19;

    /// <summary>
    /// Represents the different input/output functions that can be bound to keys or mouse actions.
    /// This enum is used as index in the KeyBindings.BindingList array.
    /// </summary>
    public enum IOFunction : uint {
        RotatingForward,
        RotatingBackwards,
        RotatingSpeedAdd,
        RotatingSpeedReduce,
        RotatingUp,
        RotatingDown,
        RotatingReset,
        FreeForward,
        FreeBackwards,
        FreeLeft,
        FreeRight,
        FreeUp,
        FreeDown,
        FreeSpeedAdd,
        FreeSpeedReduce,
        FreeReset,
        Select,
        MultiSelectModifier,
        MoveCamera
    }

    public enum CameraModel : byte {
        Rotating,
        FreeCam,
    }

    public enum SpecialKeys {
        Shift = 340,
        Ctrl = 341
    }

    public enum RepressChecks : uint {
        Select,
    }

    #region KeyBindingList
    [NativeMarshalling(typeof(KeyBindingListMarshaller))]
    public struct KeyBindingList {
        public string[] Names;
    }

    public static int KeyBindingListSize => Marshal.SizeOf<KeyBindingListMarshaller.KeyBindingListUnmanaged>();

    [CustomMarshaller(typeof(KeyBindingList), MarshalMode.Default, typeof(KeyBindingListMarshaller))]
    internal static unsafe class KeyBindingListMarshaller {
        public static KeyBindingListUnmanaged ConvertToUnmanaged(KeyBindingList managed) {
            return new KeyBindingListUnmanaged {
                Amount = (ulong) managed.Names.Length,
                Names = managed.Names.ToUnmanagedMemory(),
            };
        }

        public static KeyBindingList ConvertToManaged(KeyBindingListUnmanaged unmanaged) {
            return new KeyBindingList {
                Names = unmanaged.Names.ToStringArray((int) unmanaged.Amount) ?? throw new ArgumentException("Names is null"),
            };
        }

        public static void Free(KeyBindingListUnmanaged unmanaged) {
            // Nothing to free
        }

        internal struct KeyBindingListUnmanaged {
            public ulong Amount;
            public IntPtr Names;
        }
    }
    #endregion

    #region IOFunctionBinding
    [NativeMarshalling(typeof(IOFunctionBindingMarshaller))]
    public struct IOFunctionBinding {
        public IOFunctionBindingType Type { get; set; }
        public int Key { get; set; }
    }

    public static int IOFunctionBindingSize => Marshal.SizeOf<IOFunctionBindingMarshaller.IOFunctionBindingUnmanaged>();

    [CustomMarshaller(typeof(IOFunctionBinding), MarshalMode.Default, typeof(IOFunctionBindingMarshaller))]
    internal static class IOFunctionBindingMarshaller {
        public static IOFunctionBindingUnmanaged ConvertToUnmanaged(IOFunctionBinding managed) {
            return new IOFunctionBindingUnmanaged {
                Key = managed.Key,
                Type = managed.Type,
            };
        }

        public static IOFunctionBinding ConvertToManaged(IOFunctionBindingUnmanaged unmanaged) {
            return new IOFunctionBinding {
                Key = unmanaged.Key,
                Type = unmanaged.Type,
            };
        }

        public static void Free(IOFunctionBindingUnmanaged unmanaged) {
            // Nothing to free
        }

        internal struct IOFunctionBindingUnmanaged {
            public IOFunctionBindingType Type;
            public int Key;
        }
    }
    #endregion

    #region KeyBindings
    [NativeMarshalling(typeof(KeyBindingsMarshaller))]
    public struct KeyBindings {
        public IOFunctionBinding[] BindingList { get; set; } // size is count of IOFunction enum members (pass by pointer)
    }

    public static int IOFunctionCount => Enum.GetValues<IOFunction>().Length;
    public static int KeyBindingsSize => IOFunctionBindingSize * IOFunctionCount;

    [CustomMarshaller(typeof(KeyBindings), MarshalMode.Default, typeof(KeyBindingsMarshaller))]
    internal static unsafe class KeyBindingsMarshaller {
        public static KeyBindingsUnmanaged ConvertToUnmanaged(KeyBindings managed) {
            return new KeyBindingsUnmanaged {
                BindingList =  ArrayMarshaller<IOFunctionBinding, IOFunctionBindingMarshaller.IOFunctionBindingUnmanaged>
                    .AllocateContainerForUnmanagedElements(
                        managed.BindingList,
                        out _)
            };
        }

        public static KeyBindings ConvertToManaged(KeyBindingsUnmanaged unmanaged) {
            return new KeyBindings {
                BindingList =
                    ArrayMarshaller<IOFunctionBinding, IOFunctionBindingMarshaller.IOFunctionBindingUnmanaged>.AllocateContainerForManagedElements(
                        unmanaged.BindingList,
                        IOFunctionCount) ?? throw new ArgumentException("BindingList is null"),
            };
        }

        public static void Free(KeyBindingsUnmanaged unmanaged) {
            ArrayMarshaller<AlphaLayer, IOFunctionBindingMarshaller.IOFunctionBindingUnmanaged>.Free(unmanaged.BindingList);
        }

        internal struct KeyBindingsUnmanaged {
            public IOFunctionBindingMarshaller.IOFunctionBindingUnmanaged* BindingList; // size is count of IOFunction enum members (pass by pointer)
        }
    }
    #endregion
}
