using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS_DEPI_GRAD.Data.Entities;

namespace WMS_DEPI_GRAD.Data.Configurations;

public class RackConfiguration : IEntityTypeConfiguration<Rack>
{
    public void Configure(EntityTypeBuilder<Rack> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
               .HasMaxLength(100)
               .IsRequired();

        builder.HasOne(r => r.Aisle)
            .WithMany(a => a.Racks)
            .HasForeignKey(r => r.AisleId);
    }
}