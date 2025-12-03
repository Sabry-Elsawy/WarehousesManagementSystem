using WMS.DAL.Data.Configurations._Common;

namespace WMS.DAL;


internal class SalesOrderConfiguration : BaseAuditableEntityConfiguration<SalesOrder, int>
{
    public override void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        base.Configure(builder);

        builder.Property(so => so.OrderDate)
       .IsRequired();

        builder.Property(so => so.SO_Number)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(so => so.Status)
               .IsRequired();

        builder.HasOne(so => so.Customer)
               .WithMany()
               .HasForeignKey(so => so.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(so => so.Warehouse)
               .WithMany()
               .HasForeignKey(so => so.WarehouseId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}