namespace WMS.DAL;


public class ReceiptItemConfigurations : IEntityTypeConfiguration<ReceiptItem>
{
    public void Configure(EntityTypeBuilder<ReceiptItem> builder)
    {
        builder.ToTable("ReceiptItems");

        builder.HasKey(ri => ri.Id);

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
