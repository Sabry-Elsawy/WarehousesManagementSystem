using WMS.DAL.Data.Configurations._Common;

namespace WMS.DAL;


internal class TransferOrderConfigurations : BaseAuditableEntityConfiguration<TransferOrder, int>
{
    public override void Configure(EntityTypeBuilder<TransferOrder> builder)
    {
        base.Configure(builder);


        builder.Property(to => to.Status)
               .IsRequired();

        // many TransferOrderItems to one TransferOrder
        builder.HasMany(to => to.TransferOrderItems)
               .WithOne(toi => toi.TransferOrder)
               .HasForeignKey(toi => toi.TransferOrderId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
