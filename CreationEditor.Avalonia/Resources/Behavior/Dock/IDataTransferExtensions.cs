using System.Diagnostics.CodeAnalysis;
using Avalonia.Input;
namespace CreationEditor.Avalonia.Behavior;

// TODO remove when there is an official custom data transfer implementation in Avalonia
public static class IDataTransferExtensions {
    private sealed class DataTransfer<T>(T value) : IDataTransfer
        where T : class {
        public T Value { get; } = value;

        IReadOnlyList<DataFormat> IDataTransfer.Formats => [];

        IReadOnlyList<IDataTransferItem> IDataTransfer.Items => [];

        public void Dispose() {}
    }

    extension(IDataTransfer) {
        public static IDataTransfer Create<T>(T data)
            where T : class => new DataTransfer<T>(data);
    }

    public static bool TryGet<T>(this IDataTransfer data, [NotNullWhen(true)] out T? value)
        where T : class {
        value = null;

        if (data is DataTransfer<T> wrapper) {
            value = wrapper.Value;
        }

        return value != null;
    }
}
