using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DAL.Contract;
using WMS.DAL.Entities._Common;

namespace WMS.DAL.Repository
{
	public class BaseRepository<TEntity , TKey> : IBaseRepository<TEntity, TKey>
		where TEntity : BaseEntity<TKey>
		where TKey : IEquatable<TKey>
	{
		private readonly ApplicationDbContext _DbContext;
		private readonly DbSet<TEntity> _dbSet;
		public BaseRepository(ApplicationDbContext DbContext)
		{
			_DbContext = DbContext;
			_dbSet = _DbContext.Set<TEntity>();
		}

		public TEntity? GetById(int id)
		{
			return _dbSet.Find(id);

		}
		public IEnumerable<TEntity> GetAll(bool WithTracking = false)
		{
            if (!WithTracking)
            {
			return	_dbSet.AsNoTracking().ToList();
            }
			return _dbSet.ToList();
		}
		public void Add(TEntity entity)
		{
			_dbSet.Add(entity);
		}

		public void Update(TEntity entity)
		{
			 _dbSet.Update(entity);
		}
		public void Delete(int id)
		{
			var existingEntity = _dbSet.Find(id);
			if (existingEntity is { })
            {
				_dbSet.Remove(existingEntity);
            }
        }



	}
}
