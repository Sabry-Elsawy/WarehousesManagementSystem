

namespace WMS_DEPI_GRAD.Data.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Status)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(i => i.Quantity)
               .IsRequired();

        builder.Property(i => i.BatchNumber)
               .HasMaxLength(50);
         
        builder.Property(i => i.ExpiryDate)
               .HasMaxLength(20);

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId);

        builder.HasOne(i => i.Bin)
            .WithMany(b => b.Inventories)
            .HasForeignKey(i => i.BinId);
    }
}