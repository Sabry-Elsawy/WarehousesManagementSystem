using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS_DEPI_GRAD.Data.Entities;

namespace WMS_DEPI_GRAD.Data.Configurations;

public class SO_ItemConfiguration : IEntityTypeConfiguration<SO_Item>
{
    public void Configure(EntityTypeBuilder<SO_Item> builder)
    {
        builder.HasKey(si => si.Id);

        builder.Property(si => si.SalesOrderId)
               .IsRequired();

        builder.Property(si => si.ProductId)
               .IsRequired();

        builder.Property(si => si.QtyOrdered)
               .IsRequired();

        builder.Property(si => si.QtyPicked)
               .IsRequired();

        builder.Property(si => si.Status)
               .HasMaxLength(20)
               .IsRequired();


        builder.HasOne(si => si.SalesOrder)
            .WithMany(so => so.SO_Items)
            .HasForeignKey(si => si.SalesOrderId);

        builder.HasOne(si => si.Product)
            .WithMany(p => p.SO_Items)
            .HasForeignKey(si => si.ProductId);

    }
}






