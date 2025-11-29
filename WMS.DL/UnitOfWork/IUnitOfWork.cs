using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DAL.Contract;
using WMS.DAL.Entities._Common;

namespace WMS.DAL.UnitOfWork
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TEntity : BaseEntity<TKey>
            where TKey : IEquatable<TKey>;

        IBinRepository GetBinRepository();

        Task<int> CompleteAsync();
    }
}
