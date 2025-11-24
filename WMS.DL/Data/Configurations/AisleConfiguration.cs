
using WMS.DAL.Data.Configurations._Common;

namespace WMS.DAL;

internal class AisleConfiguration : BaseAuditableEntityConfiguration<Aisle, int>
{
    public override void Configure(EntityTypeBuilder<Aisle> builder)
    {
        base.Configure(builder);

        builder.Property(a => a.Name)
               .HasMaxLength(100)
               .IsRequired();


        builder.HasOne(a => a.Zone)
            .WithMany(z => z.Aisles)
            .HasForeignKey(a => a.ZoneId);
    }
}