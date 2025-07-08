using System.Collections.Generic;
using System.Threading.Tasks;

namespace invoice.Data
{
     public interface IRepository<T> where T : class
     {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(string id);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(string id);
     }
}
