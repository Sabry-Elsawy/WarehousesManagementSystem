using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WMS_DEPI_GRAD.Data.Entities;
using SalesOrder = WMS_DEPI_GRAD.Data.Entities.SalesOrder;

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


}
