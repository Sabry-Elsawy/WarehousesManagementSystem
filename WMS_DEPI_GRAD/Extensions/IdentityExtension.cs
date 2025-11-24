using Microsoft.AspNetCore.Identity;
using WMS.DAL.Entities._Identity;
using WMS.DAL;

namespace WMS_DEPI_GRAD.Extensions
{
    public static class IdentityExtension
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.Configure<IdentityOptions>(identityOptions =>
            {
                identityOptions.User.RequireUniqueEmail = true;
                identityOptions.SignIn.RequireConfirmedEmail = true;
                identityOptions.Lockout.MaxFailedAccessAttempts = 4;
                identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                identityOptions.Lockout.AllowedForNewUsers = true;
            });
            return services;
        }
    }
}
