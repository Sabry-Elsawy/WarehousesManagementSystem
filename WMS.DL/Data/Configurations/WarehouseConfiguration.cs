namespace WMS.DAL;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(w => w.Code)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(w => w.Capacity)
               .IsRequired();

        builder.Property(w => w.City)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(w => w.Country)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(w => w.Street)
               .HasMaxLength(100);


        // many POs to one Warehouse
        builder.HasMany(w => w.PurchaseOrders)
            .WithOne(po => po.Warehouse)
            .HasForeignKey(po => po.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        // many SOs to one Warehouse
        builder.HasMany(w => w.SalesOrders)
            .WithOne(so => so.Warehouse)
            .HasForeignKey(so => so.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        // many Receipts to one Warehouse
        builder.HasMany(builder => builder.Receipts)
            .WithOne(r => r.Warehouse)
            .HasForeignKey(r => r.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        // many TOs where Warehouse is source
        builder.HasMany(w => w.SourceTransferOrders)
            .WithOne(to => to.SourceWarehouse)
            .HasForeignKey(to => to.SourceWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        // many TOs where Warehouse is destination
        builder.HasMany(w => w.DestinationTransferOrders)
            .WithOne(to => to.DestinationWarehouse)
            .HasForeignKey(to => to.DestinationWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}