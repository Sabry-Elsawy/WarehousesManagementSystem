using WMS.DAL.Data.Configurations._Common;

namespace WMS.DAL;


internal class PurchaseOrderItemConfigurations : BaseAuditableEntityConfiguration<PurchaseOrderItem, int>
{
    public override void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        base.Configure(builder);


        builder.Property(poi => poi.PurchaseOrderId)
               .IsRequired();

        builder.Property(poi => poi.ProductId)
               .IsRequired();

        builder.Property(poi => poi.QtyOrdered)
               .IsRequired();

        builder.Property(poi => poi.UnitPrice)
               .HasColumnType("decimal(18,2)");

        builder.Property(poi => poi.SKU)
                 .IsRequired()
                 .HasMaxLength(100);
    }

}
