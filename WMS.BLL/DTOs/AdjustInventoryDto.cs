using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.BLL.DTOs;
public class AdjustInventoryDto
{
    public string SKU { get; set; } = string.Empty;
    public int Change { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
}