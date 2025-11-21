namespace WMS.DAL;


public class TransferOrderConfigurations : IEntityTypeConfiguration<TransferOrder>
{
    public void Configure(EntityTypeBuilder<TransferOrder> builder)
    {
        builder.HasKey(to => to.Id);

        builder.Property(to => to.Status)
               .IsRequired();

        // many TransferOrderItems to one TransferOrder
        builder.HasMany(to => to.TransferOrderItems)
               .WithOne(toi => toi.TransferOrder)
               .HasForeignKey(toi => toi.TransferOrderId)
               .OnDelete(DeleteBehavior.Cascade);


    }
}
