using WMS.DAL.Data.Configurations._Common;

namespace WMS.DAL;

internal class AdvancedShippingNoticeItemConfigurations : BaseAuditableEntityConfiguration<AdvancedShippingNoticeItem, int>
{
    public override void Configure(EntityTypeBuilder<AdvancedShippingNoticeItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("ASN_Items");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.QtyShipped)
               .IsRequired();

        builder.Property(item => item.SKU)
               .IsRequired()
               .HasMaxLength(50);

        // Optional relationship with PurchaseOrderItem for partial shipments
        builder.HasOne<PurchaseOrderItem>()
               .WithMany()
               .HasForeignKey(item => item.LinkedPOItemId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(false);
    }
}
