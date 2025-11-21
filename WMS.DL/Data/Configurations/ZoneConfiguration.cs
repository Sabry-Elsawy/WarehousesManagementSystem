namespace WMS.DAL;


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
            .HasForeignKey(z => z.WarehouseId)
            .IsRequired();
    }
}