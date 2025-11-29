using WMS.DAL.Contract;
using WMS.DAL.Entities._Common;

namespace WMS.DAL.Repository
{
    public class GenericRepository<TEntity , TKey> : IGenericRepository<TEntity, TKey>
		where TEntity : BaseEntity<TKey>
		where TKey : IEquatable<TKey>
	{
		private readonly ApplicationDbContext _DbContext;
		private readonly DbSet<TEntity> _dbSet;
		public GenericRepository(ApplicationDbContext DbContext)
		{
			_DbContext = DbContext;
			_dbSet = _DbContext.Set<TEntity>();
		}

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(bool withTracking = false)
        => withTracking ?
        await _dbSet.ToListAsync() : await _dbSet.AsNoTracking().ToListAsync();

        public async Task<TEntity?> GetByIdAsync(TKey id)
            => await _dbSet.FindAsync(id);

        public async Task AddAsync(TEntity entity)
            => await _dbSet.AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
            => await _dbSet.AddRangeAsync(entities);

        public void Update(TEntity entity)
            => _dbSet.Update(entity);

        public void Delete(TEntity entity)
            => _dbSet.Remove(entity);
    }
}
