
namespace WMS_DEPI_GRAD;

public class ReceiptConfigurations : IEntityTypeConfiguration<Receipt>
{
    public void Configure(EntityTypeBuilder<Receipt> builder)
    {
        builder.ToTable("Receipts");


        builder.HasKey(r => r.Id);

        builder.Property(r => r.RecievedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // many receipt items to one receipt
        builder.HasMany(r => r.ReceiptItems)
            .WithOne(ri => ri.Receipt)
            .HasForeignKey(ri => ri.ReceiptId)
            .OnDelete(DeleteBehavior.Restrict);

        // one ASN to one receipt
        builder.HasOne(builder => builder.AdvancedShippingNotice)
            .WithOne();






    }
}
