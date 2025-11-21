
using WMS_DEPI_GRAD.Models;

namespace WMS_DEPI_GRAD;

public class ReceiptController : Controller
{
    public IActionResult Index()
    {
           IEnumerable<ReceiptModel> receipts = new List<ReceiptModel>
        {
            new ReceiptModel
            {
                Id = 1,
                POId = 1001,
                ASNId = 5001,
                VendorName = "Vendor A",
                ReceiptDate = DateTime.Now,
                Items = new List<ReceiptItemModel>
                {
                    new ReceiptItemModel {Id = 1, ProductName = "Mobile", Qty = 10, SKU = "SKU001", ReceiptId = 1},
                    new ReceiptItemModel {Id = 2, ProductName = "Laptop", Qty = 5, SKU = "SKU002", ReceiptId = 1}
                }
            },
            new ReceiptModel
            {
                Id = 2,
                POId = 1002,
                ASNId = null,
                VendorName = "Vendor B",
                ReceiptDate = DateTime.Now.AddDays(-1),
                Items = new List<ReceiptItemModel>
                {
                    new ReceiptItemModel {Id = 3, ProductName = "Tablet", Qty = 15, SKU = "SKU003", ReceiptId = 2},
                    new ReceiptItemModel {Id = 4, ProductName = "Monitor", Qty = 25, SKU = "SKU004", ReceiptId = 2}
                }
            },
            new ReceiptModel
            {
                Id = 3,
                POId = 1003,
                ASNId = 5003,
                VendorName = "Vendor C",
                ReceiptDate = DateTime.Now.AddDays(-2),
                Items = new List<ReceiptItemModel>
                {
                    new ReceiptItemModel {Id = 5, ProductName = "Keyboard", Qty = 50, SKU = "SKU005", ReceiptId = 3},
                    new ReceiptItemModel {Id = 6, ProductName = "Mouse", Qty = 75, SKU = "SKU006", ReceiptId = 3}
                }
            },
            new ReceiptModel
            {
                Id = 4,
                POId = 1004,
                ASNId = null,
                VendorName = "Vendor D",
                ReceiptDate = DateTime.Now.AddDays(-3),
                Items = new List<ReceiptItemModel>
                {
                    new ReceiptItemModel {Id = 7, ProductName = "Printer", Qty = 8, SKU = "SKU007", ReceiptId = 4},
                    new ReceiptItemModel {Id = 8, ProductName = "Scanner", Qty = 12, SKU = "SKU008", ReceiptId = 4}
                }
            },
            new ReceiptModel
            {
                Id = 5,
                POId = 1005,
                ASNId = 5005,
                VendorName = "Vendor E",
                ReceiptDate = DateTime.Now.AddDays(-4),
                Items = new List<ReceiptItemModel>
                {
                    new ReceiptItemModel {Id = 9, ProductName = "Webcam", Qty = 20, SKU = "SKU009", ReceiptId = 5},
                    new ReceiptItemModel {Id = 10, ProductName = "Headset", Qty = 30, SKU = "SKU010", ReceiptId = 5}
                }
            },
            new ReceiptModel
            {
                Id = 6,
                POId = 1006,
                ASNId = 5006,
                VendorName = "Vendor F",
                ReceiptDate = DateTime.Now.AddDays(-5),
                Items = new List<ReceiptItemModel>
                {
                    new ReceiptItemModel {Id = 11, ProductName = "Speaker", Qty = 18, SKU = "SKU011", ReceiptId = 6},
                    new ReceiptItemModel {Id = 12, ProductName = "Microphone", Qty = 22, SKU = "SKU012", ReceiptId = 6}
                }
            },
            new ReceiptModel
            {
                Id = 7,
                POId = 1007,
                ASNId = null,
                VendorName = "Vendor G",
                ReceiptDate = DateTime.Now.AddDays(-6),
                Items = new List<ReceiptItemModel>
                {
                    new ReceiptItemModel {Id = 13, ProductName = "External Hard Drive", Qty = 14, SKU = "SKU013", ReceiptId = 7},
                    new ReceiptItemModel {Id = 14, ProductName = "USB Flash Drive", Qty = 40, SKU = "SKU014", ReceiptId = 7}
                }
            },
            new ReceiptModel
            {
                Id = 8,
                POId = 1008,
                ASNId = 5008,
                VendorName = "Vendor H",
                ReceiptDate = DateTime.Now.AddDays(-7),
                Items = new List<ReceiptItemModel>
                {
                    new ReceiptItemModel {Id = 15, ProductName = "Router", Qty = 9, SKU = "SKU015", ReceiptId = 8},
                    new ReceiptItemModel {Id = 16, ProductName = "Switch", Qty = 11, SKU = "SKU016", ReceiptId = 8}
                }
            },
            new ReceiptModel
            {
                Id = 9,
                POId = 1009,
                ASNId = null,
                VendorName = "Vendor I",
                ReceiptDate = DateTime.Now.AddDays(-8),
                Items = new List<ReceiptItemModel>
                {
                    new ReceiptItemModel {Id = 17, ProductName = "Projector", Qty = 6, SKU = "SKU017", ReceiptId = 9},
                    new ReceiptItemModel {Id = 18, ProductName = "Whiteboard", Qty = 4, SKU = "SKU018", ReceiptId = 9}
                }
            },
            new ReceiptModel
            {
                Id = 10,
                POId = 1010,
                ASNId = 5010,
                VendorName = "Vendor J",
                ReceiptDate = DateTime.Now.AddDays(-9),
                Items = new List<ReceiptItemModel>
                {
                    new ReceiptItemModel {Id = 19, ProductName = "Desk Lamp", Qty = 27, SKU = "SKU019", ReceiptId = 10},
                    new ReceiptItemModel {Id = 20, ProductName = "Office Chair", Qty = 13, SKU = "SKU020", ReceiptId = 10}
                }
            }

        };
        
        return View(receipts);
    }


    [HttpGet]
    public IActionResult GetReceipt(int id)
    {
        IEnumerable<ReceiptModel> receipts = new List<ReceiptModel>
        {
            new ReceiptModel
            {
                Id = 1,
                POId = 1001,
                ASNId = 5001,
                VendorName = "Vendor A",
                ReceiptDate = DateTime.Now,
                Items = new List<ReceiptItemModel>
                {
                    new ReceiptItemModel {Id = 1, ProductName = "Mobile", Qty = 10, SKU = "SKU001", ReceiptId = 1},
                    new ReceiptItemModel {Id = 2, ProductName = "Laptop", Qty = 5, SKU = "SKU002", ReceiptId = 1}
                }
            },
            new ReceiptModel
            {
                Id = 2,
                POId = 1002,
                ASNId = null,
                VendorName = "Vendor B",
                ReceiptDate = DateTime.Now.AddDays(-1),
                Items = new List<ReceiptItemModel>
                {
                    new ReceiptItemModel {Id = 3, ProductName = "Tablet", Qty = 15, SKU = "SKU003", ReceiptId = 2},
                    new ReceiptItemModel {Id = 4, ProductName = "Monitor", Qty = 25, SKU = "SKU004", ReceiptId = 2}
                }
            },
            new ReceiptModel
            {
                Id = 3,
                POId = 1003,
                ASNId = 5003,
                VendorName = "Vendor C",
                ReceiptDate = DateTime.Now.AddDays(-2),
                Items = new List<ReceiptItemModel>
                {
                    new ReceiptItemModel {Id = 5, ProductName = "Keyboard", Qty = 50, SKU = "SKU005", ReceiptId = 3},
                    new ReceiptItemModel {Id = 6, ProductName = "Mouse", Qty = 75, SKU = "SKU006", ReceiptId = 3}
                }
            },
            new ReceiptModel
            {
                Id = 4,
                POId = 1004,
                ASNId = null,
                VendorName = "Vendor D",
                ReceiptDate = DateTime.Now.AddDays(-3),
                Items = new List<ReceiptItemModel>
                {
                    new ReceiptItemModel {Id = 7, ProductName = "Printer", Qty = 8, SKU = "SKU007", ReceiptId = 4},
                    new ReceiptItemModel {Id = 8, ProductName = "Scanner", Qty = 12, SKU = "SKU008", ReceiptId = 4}
                }
            },
            new ReceiptModel
            {
                Id = 5,
                POId = 1005,
                ASNId = 5005,
                VendorName = "Vendor E",
                ReceiptDate = DateTime.Now.AddDays(-4),
                Items = new List<ReceiptItemModel>
                {
                    new ReceiptItemModel {Id = 9, ProductName = "Webcam", Qty = 20, SKU = "SKU009", ReceiptId = 5},
                    new ReceiptItemModel {Id = 10, ProductName = "Headset", Qty = 30, SKU = "SKU010", ReceiptId = 5}
                }
            },
            new ReceiptModel
            {
                Id = 6,
                POId = 1006,
                ASNId = 5006,
                VendorName = "Vendor F",
                ReceiptDate = DateTime.Now.AddDays(-5),
                Items = new List<ReceiptItemModel>
                {
                    new ReceiptItemModel {Id = 11, ProductName = "Speaker", Qty = 18, SKU = "SKU011", ReceiptId = 6},
                    new ReceiptItemModel {Id = 12, ProductName = "Microphone", Qty = 22, SKU = "SKU012", ReceiptId = 6}
                }
            },
            new ReceiptModel
            {
                Id = 7,
                POId = 1007,
                ASNId = null,
                VendorName = "Vendor G",
                ReceiptDate = DateTime.Now.AddDays(-6),
                Items = new List<ReceiptItemModel>
                {
                    new ReceiptItemModel {Id = 13, ProductName = "External Hard Drive", Qty = 14, SKU = "SKU013", ReceiptId = 7},
                    new ReceiptItemModel {Id = 14, ProductName = "USB Flash Drive", Qty = 40, SKU = "SKU014", ReceiptId = 7}
                }
            },
            new ReceiptModel
            {
                Id = 8,
                POId = 1008,
                ASNId = 5008,
                VendorName = "Vendor H",
                ReceiptDate = DateTime.Now.AddDays(-7),
                Items = new List<ReceiptItemModel>
                {
                    new ReceiptItemModel {Id = 15, ProductName = "Router", Qty = 9, SKU = "SKU015", ReceiptId = 8},
                    new ReceiptItemModel {Id = 16, ProductName = "Switch", Qty = 11, SKU = "SKU016", ReceiptId = 8}
                }
            },
            new ReceiptModel
            {
                Id = 9,
                POId = 1009,
                ASNId = null,
                VendorName = "Vendor I",
                ReceiptDate = DateTime.Now.AddDays(-8),
                Items = new List<ReceiptItemModel>
                {
                    new ReceiptItemModel {Id = 17, ProductName = "Projector", Qty = 6, SKU = "SKU017", ReceiptId = 9},
                    new ReceiptItemModel {Id = 18, ProductName = "Whiteboard", Qty = 4, SKU = "SKU018", ReceiptId = 9}
                }
            },
            new ReceiptModel
            {
                Id = 10,
                POId = 1010,
                ASNId = 5010,
                VendorName = "Vendor J",
                ReceiptDate = DateTime.Now.AddDays(-9),
                Items = new List<ReceiptItemModel>
                {
                    new ReceiptItemModel {Id = 19, ProductName = "Desk Lamp", Qty = 27, SKU = "SKU019", ReceiptId = 10},
                    new ReceiptItemModel {Id = 20, ProductName = "Office Chair", Qty = 13, SKU = "SKU020"}
                }
            }

        };
        foreach (var receipt in receipts)
        {
            if (receipt.Id == id)
            {
                return View(receipt);
            }
        }
        return NotFound();
    }

    [HttpPost]

    public IActionResult ProcessItem(ReceiptItemModel item)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Process the item (e.g., update inventory, etc.)
        // ...

        return RedirectToAction("GetReceipt", new { id = item.ReceiptId });
    }
}
