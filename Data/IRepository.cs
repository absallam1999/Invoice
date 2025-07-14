using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace invoice.Data
{
     public interface IRepository<T> where T : class
     {
        Task<IEnumerable<T>> GetAll(params Expression<Func<T, object>>[] includes);
        Task<T> GetById(string id, params Expression<Func<T, object>>[] includes);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(string id);
     }
}
