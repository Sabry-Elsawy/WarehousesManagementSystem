using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using WMS_DEPI_GRAD.Data;

namespace WMS_DEPI_GRAD;


public static class DependencyInjection
{
    public static IServiceCollection AddDependences(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextServices(configuration);
        

        return services;
    }

    private static IServiceCollection AddDbContextServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("connection string 'DefaultConnection' is not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    



}
