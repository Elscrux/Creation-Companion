using DynamicData.Binding;
using NiflySharp;
namespace NifPlugin.Models;

public sealed class NifBlock(int id, INiObject niObject) {
    public int Id { get; } = id;
    public INiObject NiObject { get; } = niObject;
    public IObservableCollection<NifBlock> Children { get; } = new ObservableCollectionExtended<NifBlock>();
}
