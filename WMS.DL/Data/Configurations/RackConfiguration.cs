namespace WMS.DAL;


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