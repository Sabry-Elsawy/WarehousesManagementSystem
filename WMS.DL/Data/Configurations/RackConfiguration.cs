using WMS.DAL.Data.Configurations._Common;

namespace WMS.DAL;


internal class RackConfiguration : BaseAuditableEntityConfiguration<Rack, int>
{
    public override void Configure(EntityTypeBuilder<Rack> builder)
    {
        base.Configure(builder);

        builder.Property(r => r.Name)
       .HasMaxLength(100)
       .IsRequired();

        builder.HasOne(r => r.Aisle)
            .WithMany(a => a.Racks)
            .HasForeignKey(r => r.AisleId);
    }

}