using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WMS.DAL.Contract;
using WMS.DAL.Entities._Common;
using WMS.DAL.Entities._Identity;

namespace WMS.DAL;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly ILoggedInUserService _loggedInUserService;

    public ApplicationDbContext(DbContextOptions options, ILoggedInUserService loggedInUserService) : base(options)
    {
        _loggedInUserService = loggedInUserService;
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

    public DbSet<Address> Addresses { get; set; }


    #region Audit Interceptor
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddAuditableProperties();

        return base.SaveChangesAsync(cancellationToken);
    }
    public override int SaveChanges()
    {
        AddAuditableProperties();
        return base.SaveChanges();
    }
    private void AddAuditableProperties()
    {
        var userId = _loggedInUserService.GetUserId() ?? "System";

        var entries = ChangeTracker.Entries<IBaseAuditableEntity>();
        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(x => x.CreatedOn).CurrentValue = DateTime.UtcNow;
                entityEntry.Property(x => x.CreatedBy).CurrentValue = userId;
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(x => x.LastModifiedOn).CurrentValue = DateTime.UtcNow;
                entityEntry.Property(x => x.LastModifiedBy).CurrentValue = userId;
            }
        }
    }
    #endregion
}
