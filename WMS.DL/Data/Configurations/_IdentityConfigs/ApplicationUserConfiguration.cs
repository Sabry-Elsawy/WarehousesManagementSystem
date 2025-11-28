using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DAL.Data.Configurations._Common;
using WMS.DAL.Entities._Identity;

namespace WMS.DAL.Data.Configurations._IdentityConfigs
{
    internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.UserName)
               .IsRequired();

            builder.Property(c => c.Role)
                    .HasConversion(
                           (URole => URole.ToString()),
                           (URole => (UserRole)Enum.Parse(typeof(UserRole), URole))
                    );

            builder.HasOne(U => U.Address)
                .WithOne(A => A.User)
                .HasForeignKey<Address>(A => A.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
