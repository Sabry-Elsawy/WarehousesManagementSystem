using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DAL.Entities._Identity;

namespace WMS.DAL.Data.Configurations._IdentityConfigs
{
    internal class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.Property(nameof(Address.Id))
                .ValueGeneratedOnAdd();

            builder.Property(A => A.Street)
                .HasColumnType("nvarchar")
                .HasMaxLength(50);

            builder.Property(A => A.City)
                .HasColumnType("nvarchar")
                .HasMaxLength(50);

            builder.Property(A => A.Country)
                .HasColumnType("nvarchar")
                .HasMaxLength(50);

            builder.ToTable("Addresses");
        }
    }
}
