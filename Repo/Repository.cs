using System.Linq.Expressions;
using invoice.Core.DTO;
using invoice.Repo;
using invoice.Repo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Repo
{
    public class Repository<T> : IRepository<T>
        where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // ---------- Retrieval ----------

        public async Task<IEnumerable<T>> GetAllAsync(
            string userId = null,
            params Expression<Func<T, object>>[] includes
        )
        {
            IQueryable<T> query = _dbSet;

            if (includes != null && includes.Any())
                query = includes.Aggregate(query, (current, include) => current.Include(include));

            if (typeof(T).GetProperty("IsDeleted") != null)
                query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);

            if (!string.IsNullOrEmpty(userId) && typeof(T).GetProperty("UserId") != null)
                query = query.Where(e => EF.Property<string>(e, "UserId") == userId);

            if (typeof(T).GetProperty("CreatedAt") != null)
                query = query.OrderByDescending(e => EF.Property<DateTime>(e, "CreatedAt"));

            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(
            string id,
            string userId = null,
            Func<IQueryable<T>, IQueryable<T>> include = null
        )
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
                query = include(query);

            if (!string.IsNullOrEmpty(userId) && typeof(T).GetProperty("UserId") != null)
                query = query.Where(e =>
                    EF.Property<string>(e, "UserId") == userId
                    && EF.Property<bool>(e, "IsDeleted") == false
                );

            return await query.FirstOrDefaultAsync(e => EF.Property<string>(e, "Id") == id);
        }

        public async Task<List<T>> GetByIdsAsync(
            List<string> ids,
            string userId = null,
            Func<IQueryable<T>, IQueryable<T>> include = null
        )
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
                query = include(query);

            if (!string.IsNullOrEmpty(userId) && typeof(T).GetProperty("UserId") != null)
                query = query.Where(e =>
                    EF.Property<string>(e, "UserId") == userId
                    && EF.Property<bool>(e, "IsDeleted") == false
                );
            if (typeof(T).GetProperty("CreatedAt") != null)
                query = query.OrderByDescending(e => EF.Property<DateTime>(e, "CreatedAt"));
            return await query.Where(e => ids.Contains(EF.Property<string>(e, "Id"))).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetByUserIdAsync(
            string userId,
            Func<IQueryable<T>, IQueryable<T>> include = null
        )
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
                query = include(query);

            if (!string.IsNullOrEmpty(userId) && typeof(T).GetProperty("UserId") != null)
                query = query.Where(e =>
                    EF.Property<string>(e, "UserId") == userId
                    && EF.Property<bool>(e, "IsDeleted") == false
                );
            if (typeof(T).GetProperty("CreatedAt") != null)
                query = query.OrderByDescending(e => EF.Property<DateTime>(e, "CreatedAt"));
            return await query.ToListAsync();
        }
        public async Task<T> GetSingleByUserIdAsync(
      string userId,
      Func<IQueryable<T>, IQueryable<T>> include = null
  )
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
                query = include(query);

            if (!string.IsNullOrEmpty(userId) && typeof(T).GetProperty("UserId") != null)
            {
                query = query.Where(e =>
                    EF.Property<string>(e, "UserId") == userId
                    && EF.Property<bool>(e, "IsDeleted") == false
                );
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> QueryAsync(
           Expression<Func<T, bool>> predicate,
           params Expression<Func<T, object>>[] includes
       )
        {
            IQueryable<T> query = _dbSet.Where(predicate);

            if (includes != null && includes.Any())
                query = includes.Aggregate(query, (current, include) => current.Include(include));

            if (typeof(T).GetProperty("CreatedAt") != null)
                query = query.OrderByDescending(e => EF.Property<DateTime>(e, "CreatedAt"));
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> QueryAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>> include = null
        )
        {
            IQueryable<T> query = _dbSet.Where(predicate);

            if (include != null)
                query = include(query);
            if (typeof(T).GetProperty("CreatedAt") != null)
                query = query.OrderByDescending(e => EF.Property<DateTime>(e, "CreatedAt"));
            return await query.ToListAsync();
        }

        public async Task<T> GetBySlugAsync(string slug, Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
                query = include(query);

                query = query.Where(e =>  EF.Property<bool>(e, "IsDeleted") == false);

            return await query.FirstOrDefaultAsync(e => EF.Property<string>(e, "Slug") == slug);
        }

        // ---------- Existence & Count ----------

        public async Task<bool> ExistsAsync(Expression < Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

 


        public async Task<int> CountAsync(string userId = null, Expression<Func<T, bool>> predicate = null)
        {
            IQueryable<T> query = _dbSet;

            if (!string.IsNullOrWhiteSpace(userId) && typeof(T).GetProperty("UserId") != null)
            {
                query = query.Where(e => EF.Property<string>(e, "UserId") == userId);
            }

            if (typeof(T).GetProperty("IsDeleted") != null)
            {
                query = query.Where(e => !EF.Property<bool>(e, "IsDeleted"));
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.CountAsync();
        }



        // ---------- Add / Update / Delete ----------

        public async Task<GeneralResponse<T>> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            return new GeneralResponse<T>
            {
                Success = true,
                Message = "Entity added successfully.",
                Data = entity,
            };
        }

        public async Task<GeneralResponse<IEnumerable<T>>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();

            return new GeneralResponse<IEnumerable<T>>
            {
                Success = true,
                Message = "Entities added successfully.",
                Data = entities,
            };
        }

        public async Task<GeneralResponse<T>> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();

            return new GeneralResponse<T>
            {
                Success = true,
                Message = "Entity updated successfully.",
                Data = entity,
            };
        }

        public async Task<GeneralResponse<IEnumerable<T>>> UpdateRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
            await _context.SaveChangesAsync();

            return new GeneralResponse<IEnumerable<T>>
            {
                Success = true,
                Message = "Entities updated successfully.",
                Data = entities,
            };
        }

        public async Task<GeneralResponse<T>> DeleteAsync(string id)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(e => EF.Property<string>(e, "Id") == id);
            if (entity == null)
            {
                return new GeneralResponse<T> { Success = false, Message = "Entity not found." };
            }

            var property = entity.GetType().GetProperty("IsDeleted");
            if (property != null)
            {
                property.SetValue(entity, true);
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }

            return new GeneralResponse<T>
            {
                Success = true,
                Message = "Entity deleted successfully.",
                Data = entity,
            };
        }

        public async Task<GeneralResponse<IEnumerable<T>>> DeleteRangeAsync(IEnumerable<string> ids)
        {
            var entities = await _dbSet
                .Where(e => ids.Contains(EF.Property<string>(e, "Id")))
                .ToListAsync();

            if (!entities.Any())
            {
                return new GeneralResponse<IEnumerable<T>>
                {
                    Success = false,
                    Message = "No entities found for deletion.",
                };
            }

            var property = entities.GetType().GetProperty("IsDeleted");
            if (property != null)
            {
                property.SetValue(entities, true);
                _dbSet.UpdateRange(entities);
                await _context.SaveChangesAsync();
            }
            else
            {
                _dbSet.RemoveRange(entities);
                await _context.SaveChangesAsync();
            }

            return new GeneralResponse<IEnumerable<T>>
            {
                Success = true,
                Message = "Entities deleted successfully.",
                Data = entities,
            };
        }

        // ---------- Utility ----------

        public IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }


        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            await transaction.CommitAsync();
        }

        public async Task RollbackTransactionAsync(IDbContextTransaction transaction)
        {
            await transaction.RollbackAsync();
        }

    }
}
