
namespace WMS_DEPI_GRAD
{
    public class AdvancedShippingNoticeItemConfigurations : IEntityTypeConfiguration<AdvancedShippingNoticeItem>
    {
        public void Configure(EntityTypeBuilder<AdvancedShippingNoticeItem> builder)
        {
            builder.ToTable("ASN_Items");

            builder.HasKey(item => item.Id);

            builder.Property(item => item.Qty)
                   .IsRequired();

            builder.Property(item => item.SKU)
                   .IsRequired()
                   .HasMaxLength(50);

            
        }
    }
}
