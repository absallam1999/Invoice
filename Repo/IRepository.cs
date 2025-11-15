using invoice.Core.DTO;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace invoice.Repo
{
    public interface IRepository<T> where T : class
    {
        // ---------- Retrieval ----------
        Task<IEnumerable<T>> GetAllAsync(string userId = null, params Expression<Func<T, object>>[] includes);
        Task<T> GetByIdAsync(string id, string userId = null, Func<IQueryable<T>, IQueryable<T>> include = null);
        Task<IEnumerable<T>> GetByUserIdAsync(string userId, Func<IQueryable<T>, IQueryable<T>> include = null);
        Task<T> GetSingleByUserIdAsync(string userId, Func<IQueryable<T>, IQueryable<T>> include = null);
        Task<List<T>> GetByIdsAsync(List<string> ids, string userId = null, Func<IQueryable<T>, IQueryable<T>> include = null);
        Task<T> GetBySlugAsync(string slug, Func<IQueryable<T>, IQueryable<T>> include = null);
        Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> include = null);

        // ---------- Existence & Count ----------
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(string userId = null,Expression < Func<T, bool>> predicate = null);

        // ---------- Add / Update / Delete ----------
        Task<GeneralResponse<T>> AddAsync(T entity);
        Task<GeneralResponse<IEnumerable<T>>> AddRangeAsync(IEnumerable<T> entities);

        Task<GeneralResponse<T>> UpdateAsync(T entity);
        Task<GeneralResponse<IEnumerable<T>>> UpdateRangeAsync(IEnumerable<T> entities);

        Task<GeneralResponse<T>> DeleteAsync(string id);
        Task<GeneralResponse<IEnumerable<T>>> DeleteRangeAsync(IEnumerable<string> ids);

        // ---------- Utility ----------
        IQueryable<T> GetQueryable();
        Task<int> SaveChangesAsync();

        // Transaction
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync(IDbContextTransaction transaction);
        Task RollbackTransactionAsync(IDbContextTransaction transaction);
    }
}
