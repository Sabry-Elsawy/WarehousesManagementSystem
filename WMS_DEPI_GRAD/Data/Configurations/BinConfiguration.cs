using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS_DEPI_GRAD.Data.Entities;

namespace WMS_DEPI_GRAD.Data.Configurations;

public class BinConfiguration : IEntityTypeConfiguration<Bin>
{
    public void Configure(EntityTypeBuilder<Bin> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Code)
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(b => b.Capacity)
               .IsRequired();

        builder.Property(b => b.BinType)
               .HasMaxLength(50)
               .IsRequired();

        builder.HasOne(b => b.Rack)
            .WithMany(r => r.Bins)
            .HasForeignKey(b => b.RackId);
    }
}