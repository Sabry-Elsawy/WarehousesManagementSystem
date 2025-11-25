using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.DAL.Entities._Common;

namespace WMS.DAL.Contract
{
	public interface IBaseRepository<TEntity,TKey>
		where TEntity : BaseEntity<TKey>
		where TKey : IEquatable<TKey>
	{
		TEntity? GetById(int id);
		IEnumerable<TEntity> GetAll(bool WithTracking = false);

		void Add(TEntity entity);

		void Update(TEntity entity);

		void Delete(int id);
	}
}
