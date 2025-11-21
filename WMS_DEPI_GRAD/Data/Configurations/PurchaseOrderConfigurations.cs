

namespace WMS_DEPI_GRAD;

public class PurchaseOrderConfigurations : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.HasKey(po => po.Id);
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
