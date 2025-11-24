using WMS.DAL.Data.Configurations._Common;

namespace WMS.DAL;

internal class AdvancedShippingNoticeItemConfigurations : BaseAuditableEntityConfiguration<AdvancedShippingNoticeItem, int>
{
    public override void Configure(EntityTypeBuilder<AdvancedShippingNoticeItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("ASN_Items");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Qty)
               .IsRequired();

        builder.Property(item => item.SKU)
               .IsRequired()
               .HasMaxLength(50);
    }
}
