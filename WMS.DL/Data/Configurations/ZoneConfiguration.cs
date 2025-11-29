using WMS.DAL.Data.Configurations._Common;

namespace WMS.DAL;


internal class ZoneConfiguration : BaseAuditableEntityConfiguration<Zone, int>
{
    public override void Configure(EntityTypeBuilder<Zone> builder)
    {
        base.Configure(builder);

        builder.Property(z => z.Name)
       .HasMaxLength(100)
       .IsRequired();

        builder.HasOne(z => z.Warehouse)
            .WithMany(wh => wh.Zones)
            .HasForeignKey(z => z.WarehouseId);
    }
}