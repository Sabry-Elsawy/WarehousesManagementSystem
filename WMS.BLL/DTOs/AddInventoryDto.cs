using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.BLL.DTOs;
public class AddInventoryDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string Location { get; set; }
    public string Status { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
}

