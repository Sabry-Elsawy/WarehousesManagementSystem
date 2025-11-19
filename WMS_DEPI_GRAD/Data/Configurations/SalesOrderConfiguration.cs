using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesOrder = WMS_DEPI_GRAD.Data.Entities.SalesOrder;

namespace WMS_DEPI_GRAD.Data.Configurations;

public class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.HasKey(so => so.Id);

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