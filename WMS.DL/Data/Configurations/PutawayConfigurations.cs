using WMS.DAL.Data.Configurations._Common;

namespace WMS.DAL;

internal class PutawayConfigurations : BaseAuditableEntityConfiguration<Putaway, int>
{
    public override void Configure(EntityTypeBuilder<Putaway> builder)
    {
        base.Configure(builder);

        builder.ToTable("Putaways");

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
