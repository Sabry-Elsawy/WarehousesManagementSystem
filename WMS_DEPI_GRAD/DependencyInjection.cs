
using Microsoft.EntityFrameworkCore;
using WMS.DAL;
using WMS.DAL.Contract;
using WMS_DEPI_GRAD.Services;

namespace WMS_DEPI_GRAD;


public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextServices(configuration)
            .AddScoped<ILoggedInUserService, LoggedInUserService>()
            .AddAuthConfigs();


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

    private static IServiceCollection AddAuthConfigs(this IServiceCollection services)
    {
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.ExpireTimeSpan = TimeSpan.FromDays(14); 
            options.SlidingExpiration = true;            
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        return services;
    }





}
