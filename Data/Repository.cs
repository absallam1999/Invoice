using invoice.Data;
using invoice.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext context;
    private readonly DbSet<T> dbSet;

    public Repository(ApplicationDbContext _context)
    {
        context = _context;
        dbSet = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAll(string userId ,params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = dbSet;

        if (!string.IsNullOrEmpty(userId) && typeof(T).GetProperty("UserId") != null)
            query = query.Where(e => EF.Property<string>(e, "UserId") == userId);


        if (includes != null)
        {
            foreach (var include in includes )
                query = query.Include(include);
        }
        return await query.ToListAsync();
    }
    public async Task<T> GetById(string id, string userId = null, Func<IQueryable<T>, IQueryable<T>> include = null)
    {
        IQueryable<T> query = dbSet;

      
        if (!string.IsNullOrEmpty(userId) && typeof(T).GetProperty("UserId") != null)
        {
            query = query.Where(e => EF.Property<string>(e, "UserId") == userId);
        }

     
        if (include != null)
        {
            query = include(query);
        }

        return await query.FirstOrDefaultAsync(e => EF.Property<string>(e, "Id") == id);
    }

    public async Task<T> GetByUserId( string UserId, Func<IQueryable<T>, IQueryable<T>> include = null)
    {
        IQueryable<T> query = dbSet;

      
        if (!string.IsNullOrEmpty(UserId) && typeof(T).GetProperty("UserId") != null)
        {
            query = query.Where(e => EF.Property<string>(e, "UserId") == UserId);
        }

        if (include != null)
        {
            query = include(query);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<OperationResult> Add(T entity)
    {
        try
        {
            await dbSet.AddAsync(entity);
            await context.SaveChangesAsync();

            return new OperationResult { Success = true };
        }
        catch (Exception ex)
        {
            var innerMessage = ex.InnerException?.Message ?? ex.Message;

            var fullError = $"{innerMessage} | StackTrace: {ex.StackTrace}";

            return new OperationResult
            {
                Success = false,
                Message = fullError,
            };
        }
    }

    public async Task<OperationResult<T>> Update(T entity)
    {
        try
        {
            dbSet.Update(entity);
            await context.SaveChangesAsync();

            return new OperationResult<T>
            {
                Success = true,
                Message = "Entity updated successfully.",
                Data = entity
            };
        }
        catch (DbUpdateConcurrencyException)
        {
            return new OperationResult<T>
            {
                Success = false,
                Message = "Update failed due to concurrency conflict.",
                Data = null
            };
        }
        catch (Exception ex)
        {
            return new OperationResult<T>
            {
                Success = false,
                Message = $"Update failed: {ex.Message}",
                Data = null
            };
        }
    }

    public async Task<OperationResult>  Delete(string id)
    {
        var entity = await dbSet.FindAsync(id);

        if (entity == null)
        {
            return new OperationResult
            {
                Success = false,
                Message = "Entity not found."
            };
        }

        try
        {
            if (entity is ISoftDeleteable softDeletable)
            {
                softDeletable.IsDeleted = true;
                dbSet.Update(entity);
            }
            else
            {
                dbSet.Remove(entity);
            }

            await context.SaveChangesAsync();

            return new OperationResult
            {
                Success = true,
                Message = "Entity deleted successfully."
            };
        }
        catch (Exception ex)
        {
            return new OperationResult
            {
                Success = false,
                Message = $"Delete failed: {ex.Message}"
            };
        }
    }

    public async Task<IEnumerable<T>> Query(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = dbSet;
        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }
        return await query.Where(predicate).ToListAsync();
    }
}
