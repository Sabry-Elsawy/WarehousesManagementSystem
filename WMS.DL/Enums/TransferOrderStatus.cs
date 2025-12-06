namespace WMS.DAL;

public enum TransferOrderStatus
{
    Pending,
    Approved,
    Issued,    // Products picked and shipped from Source
    Received,  // Products received at Destination
    Cancelled
}
