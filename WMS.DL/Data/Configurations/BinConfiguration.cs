namespace WMS.DAL;

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

        // one bin to many putaway bins
        builder.HasMany(b => b.PutawayBins)
            .WithOne(pb => pb.Bin)
            .HasForeignKey(pb => pb.BinId);
    }
}