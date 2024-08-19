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
                .TrimStart("x")
                .TrimStart("bsk")
                .TrimStart("bsm")
                .TrimStart("bsh")
                .TrimStart("rtm")
                .TrimStart("byoh")
                .TrimStart("dlc1")
                .TrimStart("dlc01")
                .TrimStart("dlc2")
                .TrimStart("dlc02")
                .TrimStart("db"));

        // Check modeled
        if (Record is IModeledGetter { Model: {} model }) {
            hashCode = HashCode.Combine(
                hashCode,
                model.File.GivenPath
                    .ToLower()
                    .Replace('/', '\\')
                    .TrimStart("meshes\\")
                    .TrimStart("bstamriel\\")
                    .TrimStart("bsmorrowind\\")
                    .TrimStart("bscyrodiil\\")
                    .TrimStart("bshighrock\\")
                    .TrimStart("bshammerfell\\")
                    .TrimStart("_byoh\\")
                    .TrimStart("dlc01\\")
                    .TrimStart("dlc02\\"),
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
