// PIMS.Repository/Repository.cs
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace PIMS.Repository.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _context;
        private readonly DbSet<T> dbSet;

        public Repository(DbContext context)
        {
            _context = context;
            dbSet = context.Set<T>();
        }

        public virtual EntityState Add(T entity)
        {
            return dbSet.Add(entity).State;
        }
        public virtual IEnumerable<EntityState> AddRange(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities), "Entities collection cannot be null.");
            }

            var addedEntityStates = new List<EntityState>();

            foreach (var entity in entities)
            {
                if (entity == null)
                {
                    throw new ArgumentException("One or more entities in the collection are null.", nameof(entities));
                }

                var entry = dbSet.Add(entity);
                addedEntityStates.Add(entry.State);
            }

            return addedEntityStates;
        }
        public T Get<TKey>(TKey id)
        {
            return dbSet.Find(id);
        }

        public async Task<T> GetAsync<TKey>(TKey id)
        {
            return await dbSet.FindAsync(id);
        }

        public T Get(params object[] keyValues)
        {
            return dbSet.Find(keyValues);
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return dbSet.Where(predicate);
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate, string include)
        {
            return FindBy(predicate).Include(include);
        }

        public IQueryable<T> GetAll()
        {
            return dbSet;
        }

        public IQueryable<T> GetAllAsNoTrancking()
        {
            return dbSet.AsNoTracking();
        }
        public IQueryable<T> GetAll(params Expression<Func<T, object>>[] includes)
        {
            var query = GetAll();
            if (includes != null)
            {
                query = includes.Aggregate(query,
                          (current, include) => current.Include(include));
            }
            return query;
        }

        public IQueryable<T> GetAll(int page, int pageCount)
        {
            var pageSize = (page - 1) * pageCount;

            return dbSet.Skip(pageSize).Take(pageCount);
        }

        public IQueryable<T> GetAll(string include)
        {
            return dbSet.Include(include);
        }

        public IQueryable<T> IncludeMultiple(IEnumerable<string> includes)
        {
            IQueryable<T> query = dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }

        public IQueryable<T> RawSql(string query, params object[] parameters)
        {
            return dbSet.FromSqlRaw(query, parameters);
        }

        public IQueryable<T> GetAll(string include, string include2)
        {
            return dbSet.Include(include).Include(include2);
        }

        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            return dbSet.Any(predicate);
        }

        public virtual EntityState SoftDelete(T entity)
        {
            entity.GetType().GetProperty("IsActive")?.SetValue(entity, false);
            return dbSet.Update(entity).State;
        }

        public virtual EntityState HardDelete(T entity)
        {
            return dbSet.Remove(entity).State;
        }
        public void HardDeleteAll()
        {
            dbSet.RemoveRange(dbSet);
        }
        public virtual EntityState Update(T entity)
        {
            return dbSet.Update(entity).State;
        }
        public async Task<T> GetAsync<TKey>(TKey id, CancellationToken cancellationToken)
        {
            return await dbSet.FindAsync(id, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await dbSet.AddRangeAsync(entities);
        }
        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await dbSet.AddAsync(entity, cancellationToken);
        }
    }
}