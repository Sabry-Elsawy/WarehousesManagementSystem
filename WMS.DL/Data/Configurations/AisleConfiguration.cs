
namespace WMS.DAL;

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