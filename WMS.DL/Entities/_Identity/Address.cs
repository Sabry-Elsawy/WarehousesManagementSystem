using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DAL.Entities._Common;

namespace WMS.DAL.Entities._Identity
{
    public class Address : BaseEntity<int>
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string Country { get; set; }
        public string? Phone { get; set; }
        public string? Company { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
