using Microsoft.EntityFrameworkCore;

namespace invoice.Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext context;
        private readonly DbSet<T> dbSet;

        public Repository(ApplicationDbContext _context)
        {
            context = _context;
            dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll() => await dbSet.ToListAsync();

        public async Task<T> GetById(string id) => await dbSet.FindAsync(id);

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
                dbSet.Remove(entity);
                await context.SaveChangesAsync();
            }
        }
    }
}