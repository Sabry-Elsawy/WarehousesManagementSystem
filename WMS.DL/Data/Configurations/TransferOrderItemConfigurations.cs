namespace WMS.DAL;

public class TransferOrderItemConfigurations : IEntityTypeConfiguration<TransferOrderItem>
{
    public void Configure(EntityTypeBuilder<TransferOrderItem> builder)
    {
        builder.ToTable("TransferOrderItems");
        builder.HasKey(toi => toi.Id);
        
        builder.Property(toi => toi.Qty)
            .IsRequired();
       
        // Relationships
        
    }
}


