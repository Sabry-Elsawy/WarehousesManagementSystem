
using WMS.DAL.Data.Configurations._Common;

namespace WMS.DAL;

internal class ProductConfiguration : BaseAuditableEntityConfiguration<Product, int>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Code)
       .HasMaxLength(50)
       .IsRequired();

        builder.Property(p => p.Name)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(p => p.Description)
               .HasMaxLength(500);

        builder.Property(p => p.Volume)
               .IsRequired();

        builder.Property(p => p.Weight)
               .IsRequired();

        builder.Property(p => p.UnitOfMeasure)
               .HasMaxLength(20)
               .IsRequired();
    }

}