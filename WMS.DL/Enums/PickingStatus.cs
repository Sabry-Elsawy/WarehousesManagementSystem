namespace WMS.DAL;

public enum PickingStatus
{
    Pending = 0,
    InProgress = 1,
    Picked = 2,
    PartiallyPicked = 3, // NEW: Picked less than required
    Backorder = 4,       // NEW: Awaiting restocking
    Cancelled = 5        // Renumbered from 3
}
