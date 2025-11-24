using WMS.DAL.Data.Configurations._Common;

namespace WMS.DAL;


internal class ReceiptItemConfigurations : BaseAuditableEntityConfiguration<ReceiptItem, int>
{
    public override void Configure(EntityTypeBuilder<ReceiptItem> builder)
    {
        base.Configure(builder);
        builder.ToTable("ReceiptItems");

        builder.Property(ri => ri.ExpectedQty)
            .IsRequired();

        builder.Property(ri => ri.ReceivedQty)
            .IsRequired();

        builder.HasMany(ri => ri.Putaways)
            .WithOne(p => p.ReceiptItem)
            .HasForeignKey(p => p.ReceiptItemId)
            .OnDelete(DeleteBehavior.Cascade);

    }

}
