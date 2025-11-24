namespace WMS_DEPI_GRAD.Controllers;

public class SalesOrderController : Controller
{
    public IActionResult Index()
    {
        var salesOrders = new List<SalesOrderModel>
    {
        new SalesOrderModel
        {
            Id = 1001,
            CustomerName = "Tech Solutions Inc.",
            OrderDate = new DateTime(2024, 1, 15),
            Items = new List<SalesOrderItemModel>
            {
                new SalesOrderItemModel { SKU = "LAP-001", SalesOrderId = 1001, ProductName = "Business Laptop", Qty = 5 },
                new SalesOrderItemModel { SKU = "MON-001", SalesOrderId = 1001, ProductName = "24-inch Monitor", Qty = 10 }
            }
        },
        new SalesOrderModel
        {
            Id = 1002,
            CustomerName = "Office Supplies Co.",
            OrderDate = new DateTime(2024, 1, 16),
            Items = new List<SalesOrderItemModel>
            {
                new SalesOrderItemModel { SKU = "DESK-001", SalesOrderId = 1002, ProductName = "Executive Desk", Qty = 3 },
                new SalesOrderItemModel { SKU = "CHAIR-001", SalesOrderId = 1002, ProductName = "Ergonomic Chair", Qty = 6 }
            }
        },
        new SalesOrderModel
        {
            Id = 1003,
            CustomerName = "Retail Store Chain",
            OrderDate = new DateTime(2024, 1, 17),
            Items = new List<SalesOrderItemModel>
            {
                new SalesOrderItemModel { SKU = "PHONE-001", SalesOrderId = 1003, ProductName = "Smartphone", Qty = 25 },
                new SalesOrderItemModel { SKU = "TAB-001", SalesOrderId = 1003, ProductName = "Tablet", Qty = 15 }
            }
        },
        new SalesOrderModel
        {
            Id = 1004,
            CustomerName = "Hospital Equipment Ltd.",
            OrderDate = new DateTime(2024, 1, 18),
            Items = new List<SalesOrderItemModel>
            {
                new SalesOrderItemModel { SKU = "SCAN-001", SalesOrderId = 1004, ProductName = "Medical Scanner", Qty = 2 },
                new SalesOrderItemModel { SKU = "MON-002", SalesOrderId = 1004, ProductName = "Medical Monitor", Qty = 4 }
            }
        },
        new SalesOrderModel
        {
            Id = 1005,
            CustomerName = "Construction Pro",
            OrderDate = new DateTime(2024, 1, 19),
            Items = new List<SalesOrderItemModel>
            {
                new SalesOrderItemModel { SKU = "TOOL-001", SalesOrderId = 1005, ProductName = "Power Drill", Qty = 8 },
                new SalesOrderItemModel { SKU = "SAFE-001", SalesOrderId = 1005, ProductName = "Safety Helmet", Qty = 20 }
            }
        },
        new SalesOrderModel
        {
            Id = 1006,
            CustomerName = "Fashion Retail Inc.",
            OrderDate = new DateTime(2024, 1, 20),
            Items = new List<SalesOrderItemModel>
            {
                new SalesOrderItemModel { SKU = "SHIRT-001", SalesOrderId = 1006, ProductName = "Cotton Shirt", Qty = 50 },
                new SalesOrderItemModel { SKU = "PANTS-001", SalesOrderId = 1006, ProductName = "Denim Jeans", Qty = 40 }
            }
        },
        new SalesOrderModel
        {
            Id = 1007,
            CustomerName = "Auto Parts Distributor",
            OrderDate = new DateTime(2024, 1, 21),
            Items = new List<SalesOrderItemModel>
            {
                new SalesOrderItemModel { SKU = "OIL-001", SalesOrderId = 1007, ProductName = "Engine Oil", Qty = 100 },
                new SalesOrderItemModel { SKU = "FILTER-001", SalesOrderId = 1007, ProductName = "Air Filter", Qty = 75 }
            }
        },
        new SalesOrderModel
        {
            Id = 1008,
            CustomerName = "Home Appliances Corp.",
            OrderDate = new DateTime(2024, 1, 22),
            Items = new List<SalesOrderItemModel>
            {
                new SalesOrderItemModel { SKU = "FRIDGE-001", SalesOrderId = 1008, ProductName = "Refrigerator", Qty = 12 },
                new SalesOrderItemModel { SKU = "WASH-001", SalesOrderId = 1008, ProductName = "Washing Machine", Qty = 10 }
            }
        },
        new SalesOrderModel
        {
            Id = 1009,
            CustomerName = "Sports Equipment Ltd.",
            OrderDate = new DateTime(2024, 1, 23),
            Items = new List<SalesOrderItemModel>
            {
                new SalesOrderItemModel { SKU = "BIKE-001", SalesOrderId = 1009, ProductName = "Mountain Bike", Qty = 6 },
                new SalesOrderItemModel { SKU = "TREAD-001", SalesOrderId = 1009, ProductName = "Treadmill", Qty = 4 }
            }
        },
        new SalesOrderModel
        {
            Id = 1010,
            CustomerName = "Bookstore Chain",
            OrderDate = new DateTime(2024, 1, 24),
            Items = new List<SalesOrderItemModel>
            {
                new SalesOrderItemModel { SKU = "BOOK-001", SalesOrderId = 1010, ProductName = "Bestseller Novel", Qty = 200 },
                new SalesOrderItemModel { SKU = "BOOK-002", SalesOrderId = 1010, ProductName = "Educational Textbook", Qty = 150 }
            }
        }
    };
        // Assume this method fetches the data
        return View(salesOrders);
    }
    

    public IActionResult GetSalesOrder(int id)
    {
        // In a real application, fetch the sales order from the database
          var salesOrders = new List<SalesOrderModel>
    {
        new SalesOrderModel
        {
            Id = 1001,
            CustomerName = "Tech Solutions Inc.",
            OrderDate = new DateTime(2024, 1, 15),
            Items = new List<SalesOrderItemModel>
            {
                new SalesOrderItemModel { SKU = "LAP-001", SalesOrderId = 1001, ProductName = "Business Laptop", Qty = 5 },
                new SalesOrderItemModel { SKU = "MON-001", SalesOrderId = 1001, ProductName = "24-inch Monitor", Qty = 10 }
            }
        },
        new SalesOrderModel
        {
            Id = 1002,
            CustomerName = "Office Supplies Co.",
            OrderDate = new DateTime(2024, 1, 16),
            Items = new List<SalesOrderItemModel>
            {
                new SalesOrderItemModel { SKU = "DESK-001", SalesOrderId = 1002, ProductName = "Executive Desk", Qty = 3 },
                new SalesOrderItemModel { SKU = "CHAIR-001", SalesOrderId = 1002, ProductName = "Ergonomic Chair", Qty = 6 }
            }
        },
        new SalesOrderModel
        {
            Id = 1003,
            CustomerName = "Retail Store Chain",
            OrderDate = new DateTime(2024, 1, 17),
            Items = new List<SalesOrderItemModel>
            {
                new SalesOrderItemModel { SKU = "PHONE-001", SalesOrderId = 1003, ProductName = "Smartphone", Qty = 25 },
                new SalesOrderItemModel { SKU = "TAB-001", SalesOrderId = 1003, ProductName = "Tablet", Qty = 15 }
            }
        },
        new SalesOrderModel
        {
            Id = 1004,
            CustomerName = "Hospital Equipment Ltd.",
            OrderDate = new DateTime(2024, 1, 18),
            Items = new List<SalesOrderItemModel>
            {
                new SalesOrderItemModel { SKU = "SCAN-001", SalesOrderId = 1004, ProductName = "Medical Scanner", Qty = 2 },
                new SalesOrderItemModel { SKU = "MON-002", SalesOrderId = 1004, ProductName = "Medical Monitor", Qty = 4 }
            }
        },
        new SalesOrderModel
        {
            Id = 1005,
            CustomerName = "Construction Pro",
            OrderDate = new DateTime(2024, 1, 19),
            Items = new List<SalesOrderItemModel>
            {
                new SalesOrderItemModel { SKU = "TOOL-001", SalesOrderId = 1005, ProductName = "Power Drill", Qty = 8 },
                new SalesOrderItemModel { SKU = "SAFE-001", SalesOrderId = 1005, ProductName = "Safety Helmet", Qty = 20 }
            }
        },
        new SalesOrderModel
        {
            Id = 1006,
            CustomerName = "Fashion Retail Inc.",
            OrderDate = new DateTime(2024, 1, 20),
            Items = new List<SalesOrderItemModel>
            {
                new SalesOrderItemModel { SKU = "SHIRT-001", SalesOrderId = 1006, ProductName = "Cotton Shirt", Qty = 50 },
                new SalesOrderItemModel { SKU = "PANTS-001", SalesOrderId = 1006, ProductName = "Denim Jeans", Qty = 40 }
            }
        },
        new SalesOrderModel
        {
            Id = 1007,
            CustomerName = "Auto Parts Distributor",
            OrderDate = new DateTime(2024, 1, 21),
            Items = new List<SalesOrderItemModel>
            {
                new SalesOrderItemModel { SKU = "OIL-001", SalesOrderId = 1007, ProductName = "Engine Oil", Qty = 100 },
                new SalesOrderItemModel { SKU = "FILTER-001", SalesOrderId = 1007, ProductName = "Air Filter", Qty = 75 }
            }
        },
        new SalesOrderModel
        {
            Id = 1008,
            CustomerName = "Home Appliances Corp.",
            OrderDate = new DateTime(2024, 1, 22),
            Items = new List<SalesOrderItemModel>
            {
                new SalesOrderItemModel { SKU = "FRIDGE-001", SalesOrderId = 1008, ProductName = "Refrigerator", Qty = 12 },
                new SalesOrderItemModel { SKU = "WASH-001", SalesOrderId = 1008, ProductName = "Washing Machine", Qty = 10 }
            }
        },
        new SalesOrderModel
        {
            Id = 1009,
            CustomerName = "Sports Equipment Ltd.",
            OrderDate = new DateTime(2024, 1, 23),
            Items = new List<SalesOrderItemModel>
            {
                new SalesOrderItemModel { SKU = "BIKE-001", SalesOrderId = 1009, ProductName = "Mountain Bike", Qty = 6 },
                new SalesOrderItemModel { SKU = "TREAD-001", SalesOrderId = 1009, ProductName = "Treadmill", Qty = 4 }
            }
        },
        new SalesOrderModel
        {
            Id = 1010,
            CustomerName = "Bookstore Chain",
            OrderDate = new DateTime(2024, 1, 24),
            Items = new List<SalesOrderItemModel>
            {
                new SalesOrderItemModel { SKU = "BOOK-001", SalesOrderId = 1010, ProductName = "Bestseller Novel", Qty = 200 },
                new SalesOrderItemModel { SKU = "BOOK-002", SalesOrderId = 1010, ProductName = "Educational Textbook", Qty = 150 }
            }
        }
    };

        foreach (var order in salesOrders)
        {
            if (order.Id == id)
            {
               return View(order);
                
            }
        }
        return NotFound();
    }

}
