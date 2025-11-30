using WMS.DAL.Entities._Common;

namespace WMS.DAL.Contract
{
    public interface IGenericRepository<TEntity,TKey>
		where TEntity : BaseEntity<TKey>
		where TKey : IEquatable<TKey>
	{
		Task<IReadOnlyList<TEntity>> GetAllAsync(bool WithTracking = false);
        Task<(IReadOnlyList<TEntity> Items, int TotalCount)> GetPagedListAsync(
            int pageNumber, 
            int pageSize, 
            System.Linq.Expressions.Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "");
		Task<TEntity?> GetByIdAsync(TKey id);

		Task AddAsync(TEntity entity);
		Task AddRangeAsync(IEnumerable<TEntity> entity);

		void Update(TEntity entity);

		void Delete(TEntity entity);
	}
}
