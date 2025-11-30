
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WMS.DAL;
using WMS.DAL.Contract;
using WMS.DAL.Entities._Identity;
using WMS.DAL.UnitOfWork;
using WMS.BLL.Interfaces;
using WMS.BLL.Services;
using WMS_DEPI_GRAD.Services;

namespace WMS_DEPI_GRAD;


public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextServices(configuration)
            .AddScoped<ILoggedInUserService, LoggedInUserService>()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IWarehouseService, WarehouseService>()
            .AddScoped<IZoneService, ZoneService>()
            .AddScoped<IAisleService, AisleService>()
            .AddScoped<IRackService, RackService>()
            .AddScoped<IBinService, BinService>()
            .AddScoped<IBinTypeService, BinTypeService>()
            .AddScoped<IPurchaseOrderService, PurchaseOrderService>()
            .AddScoped<IASNService, ASNService>()
            .AddScoped<IReceiptService, ReceiptService>();

        // Master Data Services
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<ICustomerService, CustomerService>();


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
