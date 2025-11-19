using Microsoft.AspNetCore.Mvc;
using WMS_DEPI_GRAD.Models;

namespace WMS_DEPI_GRAD.Controllers;
public class OutboundController : Controller
{
    private static List<SalesOrder> Orders = new()
        {
            new SalesOrder { Id = 1, CustomerName = "John Smith", OrderDate = DateTime.Parse("2024-01-15"), Items = new List<SalesOrderItem>() },
            new SalesOrder { Id = 2, CustomerName = "Sarah Johnson", OrderDate = DateTime.Parse("2024-01-16"), Items = new List<SalesOrderItem>() },
        };

    public IActionResult Index()
    {
        return View(Orders);
    }

    [HttpPost]
    public IActionResult CreateOrder(string CustomerName, DateTime OrderDate)
    {
        var newOrder = new SalesOrder
        {
            Id = Orders.Count + 1,
            CustomerName = CustomerName,
            OrderDate = OrderDate,
            Items = new List<SalesOrderItem>()
        };

        Orders.Add(newOrder);
        TempData["Success"] = $"Order #{newOrder.Id} created successfully.";
        return RedirectToAction(nameof(Index));
    }
}
