using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace invoice.Data
{
     public interface IRepository<T> where T : class
     {
        Task<IEnumerable<T>> GetAll(string userId=null ,params Expression<Func<T, object>>[] includes);
        Task<T> GetById( string id, string userId = null, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> Query(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(string idl);

        //Task<IEnumerable<T>> Filter(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);

    }
}
