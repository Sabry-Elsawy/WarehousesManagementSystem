using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS_DEPI_GRAD.Data.Entities;

namespace WMS_DEPI_GRAD.Data.Configurations;

public class ZoneConfiguration : IEntityTypeConfiguration<Zone>
{
    public void Configure(EntityTypeBuilder<Zone> builder)
    {
        builder.HasKey(z => z.Id);

        builder.Property(z => z.Name)
               .HasMaxLength(100)
               .IsRequired();

        builder.HasOne(z => z.Warehouse)
            .WithMany(wh => wh.Zones)
            .HasForeignKey(z => z.WareHouseId)
            .IsRequired();
    }
}