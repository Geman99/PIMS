using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace PIMS.Repository.Repositories
{
    public interface IRepository<T> where T : class
    {
        /// <returns>The Entity's state</returns>
        EntityState Add(T entity);
        /// <returns>The Entity's states</returns>
        IEnumerable<EntityState> AddRange(IEnumerable<T> entities);
        /// <returns>The Entity's state</returns>
        EntityState Update(T entity);
        /// <returns>Entity</returns>
        T Get<TKey>(TKey id);

        /// <returns>Task Entity</returns>
        Task<T> GetAsync<TKey>(TKey id);

        /// <returns>The requested Entity</returns>
        T Get(params object[] keyValues);

        /// <returns>Entity</returns>
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);

        /// <returns>Queryable</returns>
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate, string include);

        /// <returns>List of entities</returns>
        IQueryable<T> GetAll();
        IQueryable<T> GetAllAsNoTrancking();
        /// <returns>Queryable</returns>
        IQueryable<T> GetAll(int page, int pageCount);

        /// <returns>List of entities</returns>
        IQueryable<T> GetAll(string include);
        IQueryable<T> GetAll(params Expression<Func<T, object>>[] includes);
        IQueryable<T> IncludeMultiple(IEnumerable<string> includes);

        /// <returns>List of entities</returns>
        IQueryable<T> RawSql(string sql, params object[] parameters);

        /// <returns>List of entities</returns>
        IQueryable<T> GetAll(string include, string include2);

        /// <summary>
        /// Soft delete with using IsActive flag for entity
        /// </summary>
        /// <returns>The Entity's state</returns>
        EntityState SoftDelete(T entity);

        /// <summary>
        /// Deletes the specified entity
        /// </summary>
        /// <returns>The Entity's state</returns>
        EntityState HardDelete(T entity);

        /// <summary>
        /// Deletes all entities
        /// </summary>
        /// <returns>void</returns>
        void HardDeleteAll();
        bool Exists(Expression<Func<T, bool>> predicate);
        Task<T> GetAsync<TKey>(TKey id, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously adds a range of entities to the database context.
        /// </summary>
        /// <param name="entities">An enumerable collection of entities to be added.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddRangeAsync(IEnumerable<T> entities);
        Task AddAsync(T entity, CancellationToken cancellationToken);
    }
}