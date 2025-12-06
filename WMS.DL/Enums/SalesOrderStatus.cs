namespace WMS.DAL;

public enum SalesOrderStatus
{
    Draft = 0,
    Submitted = 1,
    Processing = 2,      // NEW: Picking tasks allocated
    Picked = 3,          // Renumbered from 2
    PartiallyPicked = 4, // NEW: Some items short-picked
    Shipped = 5,         // Renumbered from 3
    Cancelled = 6        // NEW: Order cancelled
}
