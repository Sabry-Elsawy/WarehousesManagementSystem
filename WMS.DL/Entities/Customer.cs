using WMS.DAL.Entities._Common;

namespace WMS.DAL;

public class Customer : BaseAuditableEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
