
using WMS.DAL.Data.Configurations._Common;

namespace WMS.DAL;

internal class InventoryConfiguration : BaseAuditableEntityConfiguration<Inventory, int>
{
    public override void Configure(EntityTypeBuilder<Inventory> builder)
    {
        base.Configure(builder);

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