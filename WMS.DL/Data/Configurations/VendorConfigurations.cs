using WMS.DAL.Data.Configurations._Common;

namespace WMS.DAL;

internal class VendorConfigurations : BaseAuditableEntityConfiguration<Vendor, int>
{
    public override void Configure(EntityTypeBuilder<Vendor> builder)
    {
        base.Configure(builder);

        builder.Property(v => v.Name)
    .IsRequired()
    .HasMaxLength(100);
        builder.Property(v => v.ContactEmail)
            .IsRequired()
            .HasMaxLength(100);
        // Relationships
        builder.HasMany(v => v.PurchaseOrders)
            .WithOne(po => po.Vendor)
            .HasForeignKey(po => po.VendorId);
    }

}

