using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DAL.Contract;
using WMS.DAL.Entities._Common;
using WMS.DAL.Repository;

namespace WMS.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly ConcurrentDictionary<string, object> _repositories;
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _repositories = new ConcurrentDictionary<string, object>();
        }
        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TEntity : BaseEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            return (IGenericRepository<TEntity, TKey>)_repositories.GetOrAdd(typeof(TEntity).Name, _ => new GenericRepository<TEntity, TKey>(_dbContext));
        }

        public async Task<int> CompleteAsync()
            => await _dbContext.SaveChangesAsync();

        public async ValueTask DisposeAsync()
            => await _dbContext.DisposeAsync();
    }
}
