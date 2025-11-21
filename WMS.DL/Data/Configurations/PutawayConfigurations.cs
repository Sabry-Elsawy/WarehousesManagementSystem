namespace WMS.DAL;

public class PutawayConfigurations : IEntityTypeConfiguration<Putaway>
{
    public void Configure(EntityTypeBuilder<Putaway> builder)
    {
        builder.ToTable("Putaways");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Qty)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired();

        // many PutawayBins to one Putaway
        builder.HasMany(p => p.PutawayBins)
            .WithOne(pb => pb.Putaway)
            .HasForeignKey(pb => pb.PutawayId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
