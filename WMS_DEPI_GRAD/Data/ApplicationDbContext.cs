

namespace WMS_DEPI_GRAD.Data;

public class ApplicationDbContext : DbContext
{
    
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<SalesOrder> SalesOrders { get; set; }
    public DbSet<SO_Item> SO_Items { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Zone> Zones { get; set; }
    public DbSet<Aisle> Aisles { get; set; }
    public DbSet<Rack> Racks { get; set; }  
    public DbSet<Bin> Bins { get; set; }  
    public DbSet<Picking> Pickings { get; set; }


    public DbSet<PurchaseOrder> POs { get; set; }
    public DbSet<PurchaseOrderItem> PO_Items { get; set; }
    public DbSet<AdvancedShippingNotice> ASNs { get; set; }
    public DbSet<AdvancedShippingNoticeItem> ASN_Items { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<ReceiptItem> ReceiptItems { get; set; }
    public DbSet<Putaway> Putaways { get; set; }
    public DbSet<TransferOrder> TransferOrders { get; set; }
    public DbSet<TransferOrderItem> TO_Items { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<PutawayBin> Putaway_Bins { get; set; }



}
