
using WMS.DAL.Data.Configurations._Common;

namespace WMS.DAL;

internal class PurchaseOrderConfigurations : BaseAuditableEntityConfiguration<PurchaseOrder, int>
{
    public override void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        base.Configure(builder);

        builder.Property(po => po.PO_Number)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(po => po.OrderDate)
            .IsRequired();
        builder.Property(po => po.Status)
            .IsRequired();

        // one purchase order to many ASNs
        builder.HasMany(po => po.ASNs)
            .WithOne(asn => asn.PurchaseOrder)
            .HasForeignKey(asn => asn.PurchaseOrderId);

        builder.HasMany(po => po.POItems)
            .WithOne(poi => poi.PurchaseOrder)
            .HasForeignKey(poi => poi.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }

}
