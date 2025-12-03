using WMS.DAL.Data.Configurations._Common;

namespace WMS.DAL;


internal class PickingConfiguration : BaseAuditableEntityConfiguration<Picking, int>
{
    public override void Configure(EntityTypeBuilder<Picking> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.QuantityToPick)
               .IsRequired();

        builder.Property(p => p.QuantityPicked)
               .IsRequired();

        builder.Property(p => p.Status)
               .IsRequired();

        //relationships
        builder.HasOne(p => p.SO_Item)
               .WithMany()
               .HasForeignKey(p => p.SO_ItemId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Product)
               .WithMany()
               .HasForeignKey(p => p.ProductId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Bin)
               .WithMany(b => b.Pickings)
               .HasForeignKey(p => p.BinId)
                .OnDelete(DeleteBehavior.Restrict);
    }

}