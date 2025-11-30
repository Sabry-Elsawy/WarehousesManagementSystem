using WMS.DAL.Data.Configurations._Common;

namespace WMS.DAL;


internal class ReceiptItemConfigurations : BaseAuditableEntityConfiguration<ReceiptItem, int>
{
    public override void Configure(EntityTypeBuilder<ReceiptItem> builder)
    {
        base.Configure(builder);
        builder.ToTable("ReceiptItems");

        builder.Property(ri => ri.QtyExpected)
            .IsRequired();

        builder.Property(ri => ri.QtyReceived)
            .IsRequired();

        // Relationship with ASNItem (prevent cascade cycles)
        builder.HasOne(ri => ri.ASNItem)
            .WithMany()
            .HasForeignKey(ri => ri.ASNItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(ri => ri.Putaways)
            .WithOne(p => p.ReceiptItem)
            .HasForeignKey(p => p.ReceiptItemId)
            .OnDelete(DeleteBehavior.Restrict);

    }

}
