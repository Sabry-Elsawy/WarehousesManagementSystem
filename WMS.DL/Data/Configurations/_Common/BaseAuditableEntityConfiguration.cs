using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DAL.Entities._Common;

namespace WMS.DAL.Data.Configurations._Common
{
    internal class BaseAuditableEntityConfiguration<TEntity, TKey> : BaseEntityConfiguration<TEntity, TKey>
        where TEntity : BaseAuditableEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        public override void Configure(EntityTypeBuilder<TEntity> builder)
        {
            base.Configure(builder);


        }
    }
}
