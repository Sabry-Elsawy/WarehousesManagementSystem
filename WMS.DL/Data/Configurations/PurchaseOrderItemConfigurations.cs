namespace WMS.DAL;


public class PurchaseOrderItemConfigurations : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.HasKey(poi => poi.Id);

        builder.Property(poi => poi.PurchaseOrderId)
               .IsRequired();

        builder.Property(poi => poi.ProductId)
               .IsRequired();

        builder.Property(poi => poi.Qty)
               .IsRequired();

        builder.Property(poi => poi.SKU)
                 .IsRequired()
                 .HasMaxLength(100);





    }
}
