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

    /// <summary>
    /// The cell is retained because it is within the landscape range of some retained cell to make sure higher uGridsToLoad settings
    /// don't directly lead to crashes and to give players exploring beyond the region border a warning that they are exiting the playable area.
    /// </summary>
    WithinLandscapeRangeOfRetainedCell
}
