using WMS.DAL;

namespace WMS.BLL.Interfaces;

public interface IShippingService
{
    Task<IEnumerable<SalesOrder>> GetOrdersReadyForShippingAsync();
    Task<string> ShipOrderAsync(int salesOrderId);
}
