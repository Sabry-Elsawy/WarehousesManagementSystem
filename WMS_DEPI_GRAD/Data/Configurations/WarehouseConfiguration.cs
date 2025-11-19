using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS_DEPI_GRAD.Data.Entities;

namespace WMS_DEPI_GRAD.Data.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(w => w.Code)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(w => w.Capacity)
               .IsRequired();

        builder.Property(w => w.City)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(w => w.Country)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(w => w.Street)
               .HasMaxLength(100);
    }
}