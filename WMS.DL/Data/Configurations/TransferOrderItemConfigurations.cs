using WMS.DAL.Data.Configurations._Common;

namespace WMS.DAL;

internal class TransferOrderItemConfigurations : BaseAuditableEntityConfiguration<TransferOrderItem, int>
{
    public override void Configure(EntityTypeBuilder<TransferOrderItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("TransferOrderItems");

        builder.Property(toi => toi.Qty)
            .IsRequired();
    }
}


