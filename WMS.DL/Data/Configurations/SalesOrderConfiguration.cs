using WMS.DAL.Data.Configurations._Common;

namespace WMS.DAL;


internal class SalesOrderConfiguration : BaseAuditableEntityConfiguration<SalesOrder, int>
{
    public override void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        base.Configure(builder);

        builder.Property(so => so.OrderDate)
       .IsRequired();

        builder.Property(so => so.CustomerRef)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(so => so.Status)
               .HasMaxLength(20)
               .IsRequired();
    }
}