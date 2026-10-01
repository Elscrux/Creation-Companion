namespace BuildStripper.Models.FeatureFlag;

public enum ExteriorCellRetainReason {
    /// <summary>
    /// The cell is retained because it is within the region border of the worldspace which defines the playable area of the worldspace.
    /// </summary>
    WithinRegionBorder,

    /// <summary>
    /// The cell is retained because it is within the view distance of some retained cell.
    /// </summary>
    WithinViewDistanceOfRetainedCell,
}
