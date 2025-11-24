
using WMS.DAL.Data.Configurations._Common;

namespace WMS.DAL;

internal class AdvancedShippingNoticeConfigurations : BaseAuditableEntityConfiguration<AdvancedShippingNotice, int>
{
    public override void Configure(EntityTypeBuilder<AdvancedShippingNotice> builder)
    {
        base.Configure(builder);
        builder.ToTable("ASNs");

        builder.Property(asn => asn.ExpectedArrivalDate)
            .IsRequired();

        builder.Property(asn => asn.SKU)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(asn => asn.Status)
            .IsRequired();

        // many ASN Items to one ASN
        builder.HasMany(asn => asn.ASNItems)
            .WithOne(item => item.AdvancedShippingNotice)
            .HasForeignKey(item => item.AdvancedShippingNoticeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
