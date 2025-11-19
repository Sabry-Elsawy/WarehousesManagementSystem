using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS_DEPI_GRAD.Data.Entities;

namespace WMS_DEPI_GRAD.Data.Configurations;

public class PickingConfiguration : IEntityTypeConfiguration<Picking>
{
    public void Configure(EntityTypeBuilder<Picking> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Qty)
               .IsRequired();

        builder.Property(p => p.Status)
               .HasMaxLength(50)
               .IsRequired();

        //relationships
        builder.HasOne(p => p.SO_Item)
               .WithMany()
               .HasForeignKey(p => p.SO_ItemId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Product)
               .WithMany()
               .HasForeignKey(p => p.ProductId)
               .OnDelete(DeleteBehavior.Restrict); 

        builder.HasOne(p => p.Bin)
               .WithMany(b => b.Pickings)
               .HasForeignKey(p => p.BinId)
                .OnDelete(DeleteBehavior.Restrict);
    }
}