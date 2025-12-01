using WMS.DAL.Entities._Common;

namespace WMS.DAL.Contract
{
    public interface IGenericRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        Task<IEnumerable<TEntity>> GetAllAsync(bool withTracking,Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);
        Task<IReadOnlyList<TEntity>> GetAllAsync(bool WithTracking = false);

        Task<TEntity?> GetByIdAsync(TKey id, Func<IQueryable<TEntity>, IQueryable<TEntity>> include);
        Task<TEntity?> GetByIdAsync(TKey id);
         
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);
    }
}
