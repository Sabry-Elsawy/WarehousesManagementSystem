using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS_DEPI_GRAD.Data.Entities;

namespace WMS_DEPI_GRAD.Data.Configurations;

public class AisleConfiguration : IEntityTypeConfiguration<Aisle>
{
    public void Configure(EntityTypeBuilder<Aisle> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
               .HasMaxLength(100)
               .IsRequired();


        builder.HasOne(a => a.Zone)
            .WithMany(z => z.Aisles)
            .HasForeignKey(a => a.ZoneId);
    }
}