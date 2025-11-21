namespace WMS_DEPI_GRAD
{
    public class VendorConfigurations : IEntityTypeConfiguration<Vendor>
    {
        public void Configure(EntityTypeBuilder<Vendor> builder)
        {
            builder.HasKey(v => v.Id);
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
}
