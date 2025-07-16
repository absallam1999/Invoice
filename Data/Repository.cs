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
        if (typeof(ISoftDeleteable).IsAssignableFrom(typeof(T)))
            query = query.Where(e => !EF.Property<bool>(e, "IsDeleted"));

        if (!string.IsNullOrEmpty(userId) && typeof(T).GetProperty("UserId") != null)
            query = query.Where(e => EF.Property<string>(e, "UserId") == userId);

        if (includes != null)
        {
            foreach (var include in includes )
                query = query.Include(include);
        }
        return await query.ToListAsync();
    }

    public async Task<T> GetById(string id, string userId, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = dbSet;
        if (typeof(ISoftDeleteable).IsAssignableFrom(typeof(T)))
            query = query.Where(e => !EF.Property<bool>(e, "IsDeleted"));


        if (!string.IsNullOrEmpty(userId) && typeof(T).GetProperty("UserId") != null)
            query = query.Where(e => EF.Property<string>(e, "UserId") == userId);


        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }
        return await query.FirstOrDefaultAsync(e => EF.Property<string>(e, "Id") == id);
    }

    public async Task Add(T entity)
    {
        await dbSet.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task Update(T entity)
    {
        dbSet.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task Delete(string id)
    {
        var entity = await dbSet.FindAsync(id);

        if (entity != null)
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
