using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.DAL.Entities._Common
{
    public interface IBaseAuditableEntity
    {
        public DateTime CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        
        // Closure audit fields
        public DateTime? ClosedOn { get; set; }
        public string? ClosedBy { get; set; }
        public bool IsAutoClosed { get; set; }
        public string? CloseReason { get; set; }
    }
    public class BaseAuditableEntity<TKey> : BaseEntity<TKey>, IBaseAuditableEntity
        where TKey : IEquatable<TKey>
    {
        public DateTime CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        
        // Closure audit fields
        public DateTime? ClosedOn { get; set; }
        public string? ClosedBy { get; set; }
        public bool IsAutoClosed { get; set; }
        public string? CloseReason { get; set; }
    }
}
