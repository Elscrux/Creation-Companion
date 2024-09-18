using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class CollisionLayerFilter : SimpleRecordFilter<ICollisionLayerGetter> {
    public CollisionLayerFilter() : base(new List<SimpleRecordFilterEntry> {
        new("Sensor", record => (record.Flags & CollisionLayer.Flag.Sensor) != 0),
        new("Navmesh Obstacle", record => (record.Flags & CollisionLayer.Flag.NavmeshObstacle) != 0),
        new("Trigger Volume", record => (record.Flags & CollisionLayer.Flag.TriggerVolume) != 0),
    }) {}
}
