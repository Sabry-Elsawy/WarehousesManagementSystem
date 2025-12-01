using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WMS_DEPI_GRAD.Controllers
{
    public class MasterDataController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // --- Warehouses ---
        public IActionResult WarehouseList()
        {
            // Dummy data for UI demo
            var warehouses = new List<dynamic>
            {
                new { Id = 1, Code = "WH-001", Name = "Main Distribution Center", City = "Cairo", Capacity = 50000 },
                new { Id = 2, Code = "WH-002", Name = "Alexandria Hub", City = "Alexandria", Capacity = 25000 },
                new { Id = 3, Code = "WH-003", Name = "Giza Overflow", City = "Giza", Capacity = 15000 }
            };
            return View(warehouses);
        }

        public IActionResult WarehouseUpsert(int? id)
        {
            return View();
        }

        // --- Zones ---
        public IActionResult ZoneList()
        {
             var zones = new List<dynamic>
            {
                new { Id = 1, Name = "Zone A - Cold Storage", Warehouse = "Main Distribution Center" },
                new { Id = 2, Name = "Zone B - Dry Goods", Warehouse = "Main Distribution Center" },
                new { Id = 3, Name = "Zone A - General", Warehouse = "Alexandria Hub" }
            };
            return View(zones);
        }

        public IActionResult ZoneUpsert(int? id)
        {
            return View();
        }

        // --- Aisles ---
        public IActionResult AisleList() => View(new List<dynamic> { 
            new { Id = 1, Code = "A-01", Zone = "Zone A - Cold Storage", Warehouse = "Main Distribution Center" },
            new { Id = 2, Code = "A-02", Zone = "Zone A - Cold Storage", Warehouse = "Main Distribution Center" }
        });
        public IActionResult AisleUpsert(int? id) => View();

        // --- Racks ---
        public IActionResult RackList() => View(new List<dynamic> {
            new { Id = 1, Code = "R-01-A", Aisle = "A-01", Zone = "Zone A", Warehouse = "Main DC" },
            new { Id = 2, Code = "R-01-B", Aisle = "A-01", Zone = "Zone A", Warehouse = "Main DC" }
        });
        public IActionResult RackUpsert(int? id) => View();

        // --- Bins ---
        public IActionResult BinList() => View(new List<dynamic> {
            new { Id = 1, Code = "B-01-A-1", Type = "Standard", Capacity = 100, Rack = "R-01-A" },
            new { Id = 2, Code = "B-01-A-2", Type = "Small Parts", Capacity = 50, Rack = "R-01-A" }
        });
        public IActionResult BinUpsert(int? id) => View();

        // --- Bin Types ---
        public IActionResult BinTypeList() => View(new List<dynamic> {
            new { Id = 1, Name = "Standard Pallet", Description = "Standard 1x1m pallet storage" },
            new { Id = 2, Name = "Cold Storage Unit", Description = "Temperature controlled bin" }
        });
        public IActionResult BinTypeUpsert(int? id) => View();

        // --- Products ---
        public IActionResult ProductList() => View(new List<dynamic> {
            new { Id = 1, Code = "PRD-001", Name = "Wireless Mouse", UOM = "Pcs", Weight = 0.2, Volume = 0.01 },
            new { Id = 2, Code = "PRD-002", Name = "Office Chair", UOM = "Pcs", Weight = 15.5, Volume = 0.5 }
        });
        public IActionResult ProductUpsert(int? id) => View();

        // --- Suppliers ---
        public IActionResult SupplierList() => View(new List<dynamic> {
            new { Id = 1, Name = "Tech Supplies Co.", Email = "contact@techsupplies.com", Address = "123 Tech Park" },
            new { Id = 2, Name = "Office Depot Inc.", Email = "sales@officedepot.com", Address = "456 Market St" }
        });
        public IActionResult SupplierUpsert(int? id) => View();

        // --- Customers ---
        public IActionResult CustomerList() => View(new List<dynamic> {
            new { Id = 1, Code = "CUST-001", Name = "Global Retailers", Contact = "John Doe" },
            new { Id = 2, Code = "CUST-002", Name = "Local Shop", Contact = "Jane Smith" }
        });
        public IActionResult CustomerUpsert(int? id) => View();
    }
}
