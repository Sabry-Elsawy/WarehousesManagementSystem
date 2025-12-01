using WMS.DAL.Data.Configurations._Common;

namespace WMS.DAL;


internal class ReceiptConfigurations : BaseAuditableEntityConfiguration<Receipt, int>
{
    public override void Configure(EntityTypeBuilder<Receipt> builder)
    {
        base.Configure(builder);

        builder.ToTable("Receipts");

        builder.Property(r => r.ReceiptNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.ReceivedDate)
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
