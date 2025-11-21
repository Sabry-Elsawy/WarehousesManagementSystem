
namespace WMS_DEPI_GRAD;

public class AdvancedShippingNoticeConfigurations : IEntityTypeConfiguration<AdvancedShippingNotice>
{
    public void Configure(EntityTypeBuilder<AdvancedShippingNotice> builder)
    {
        builder.ToTable("ASNs");

        builder.HasKey(asn => asn.Id);

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
