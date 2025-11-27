
using Microsoft.EntityFrameworkCore;
using WMS.DAL;
using WMS.DAL.Contract;
using WMS_DEPI_GRAD.Services;

namespace WMS_DEPI_GRAD;


public static class DependencyInjection
{
    public static IServiceCollection AddDependences(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextServices(configuration)
            .AddScoped<ILoggedInUserService, LoggedInUserService>();
        

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
