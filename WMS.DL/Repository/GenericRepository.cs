using System.Linq.Expressions;
using WMS.DAL.Contract;
using WMS.DAL.Entities._Common;

namespace WMS.DAL.Repository
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
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

        public async Task<IEnumerable<TEntity>> GetAllWithIncludeAsync(bool withTracking = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            IQueryable<TEntity> query = withTracking ? _dbSet : _dbSet.AsNoTracking();

            if (include != null)
                query = include(query);

            return await query.ToListAsync();
        }

        public async Task<(IReadOnlyList<TEntity> Items, int TotalCount)> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            System.Linq.Expressions.Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            int totalCount = await query.CountAsync();

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<TEntity?> GetByIdAsync(TKey id)
            => await _dbSet.FindAsync(id);

        public async Task<TEntity?> GetByIdAsync(
            TKey id,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> include)
        {
            IQueryable<TEntity> query = _dbSet.AsNoTracking();

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(e => e.Id!.Equals(id));
        }


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
