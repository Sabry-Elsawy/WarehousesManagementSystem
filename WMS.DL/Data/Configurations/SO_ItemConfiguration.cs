
using WMS.DAL.Data.Configurations._Common;

namespace WMS.DAL;

internal class SO_ItemConfiguration : BaseAuditableEntityConfiguration<SO_Item, int>
{
    public override void Configure(EntityTypeBuilder<SO_Item> builder)
    {
        base.Configure(builder);

        builder.Property(si => si.SalesOrderId)
       .IsRequired();

        builder.Property(si => si.ProductId)
               .IsRequired();

        builder.Property(si => si.QtyOrdered)
               .IsRequired();

        builder.Property(si => si.QtyPicked)
               .IsRequired();

        builder.Property(si => si.UnitPrice)
               .HasColumnType("decimal(18,2)")
               .IsRequired();


        builder.HasOne(si => si.SalesOrder)
            .WithMany(so => so.SO_Items)
            .HasForeignKey(si => si.SalesOrderId);

        builder.HasOne(si => si.Product)
            .WithMany(p => p.SO_Items)
            .HasForeignKey(si => si.ProductId);
    }
}






