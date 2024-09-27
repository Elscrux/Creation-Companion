using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace VanillaDuplicateCleaner.Models;

internal sealed record RecordEqualsMask(IMajorRecordGetter Record) {
    public bool Equals(RecordEqualsMask? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return GetHashCode() == other.GetHashCode();
    }

    public override int GetHashCode() {
        var hashCode = HashCode.Combine(
            Record.EditorID?
                .ToLower()
                .TrimStart("x", StringComparison.OrdinalIgnoreCase)
                .TrimStart("bsk", StringComparison.OrdinalIgnoreCase)
                .TrimStart("bsm", StringComparison.OrdinalIgnoreCase)
                .TrimStart("bsh", StringComparison.OrdinalIgnoreCase)
                .TrimStart("rtm", StringComparison.OrdinalIgnoreCase)
                .TrimStart("byoh", StringComparison.OrdinalIgnoreCase)
                .TrimStart("dlc1", StringComparison.OrdinalIgnoreCase)
                .TrimStart("dlc01", StringComparison.OrdinalIgnoreCase)
                .TrimStart("dlc2", StringComparison.OrdinalIgnoreCase)
                .TrimStart("dlc02", StringComparison.OrdinalIgnoreCase)
                .TrimStart("db", StringComparison.OrdinalIgnoreCase));

        // Check modeled
        if (Record is IModeledGetter { Model: {} model }) {
            hashCode = HashCode.Combine(
                hashCode,
                model.File.GivenPath
                    .ToLower()
                    .Replace('/', '\\')
                    .TrimStart("meshes\\", StringComparison.OrdinalIgnoreCase)
                    .TrimStart("bstamriel\\", StringComparison.OrdinalIgnoreCase)
                    .TrimStart("bsmorrowind\\", StringComparison.OrdinalIgnoreCase)
                    .TrimStart("bscyrodiil\\", StringComparison.OrdinalIgnoreCase)
                    .TrimStart("bshighrock\\", StringComparison.OrdinalIgnoreCase)
                    .TrimStart("bshammerfell\\", StringComparison.OrdinalIgnoreCase)
                    .TrimStart("_byoh\\", StringComparison.OrdinalIgnoreCase)
                    .TrimStart("dlc01\\", StringComparison.OrdinalIgnoreCase)
                    .TrimStart("dlc02\\", StringComparison.OrdinalIgnoreCase),
                model.AlternateTextures?.Count,
                model.AlternateTextures?.Select(x => HashCode.Combine(x.Name, x.NewTexture.FormKey.ID, x.Index)));
        }

        // todo check files by content

        // Check scripted
        if (Record is IHaveVirtualMachineAdapterGetter { VirtualMachineAdapter: not null } scripted) {
            hashCode = HashCode.Combine(
                hashCode,
                scripted.VirtualMachineAdapter.Scripts.Select(x => HashCode.Combine(x.Name)));
        }

        // Check container items
        if (Record is IContainerGetter { Items: not null } container) {
            hashCode = HashCode.Combine(
                hashCode,
                container.Items.Count,
                container.Items.Select(x => HashCode.Combine(x.Item.Item.FormKey.ID, x.Item.Count)));
        }

        return hashCode;
    }
}
