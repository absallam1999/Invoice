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
       
        Task<OperationResult> Add(T entity);
        Task<OperationResult<T>> Update(T entity);
        Task <OperationResult> Delete(string idl);

   
    }
}
