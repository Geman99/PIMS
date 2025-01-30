using Microsoft.EntityFrameworkCore;
using PIMS.Repository.Repositories;

namespace PIMS.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private DbContext _context;

        private Dictionary<Type, object> repos;

        public UnitOfWork(DbContext context)
        {
            _context = context;
        }

        public IRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class
        {
            if (repos == null)
            {
                repos = new Dictionary<Type, object>();
            }

            var type = typeof(TEntity);
            if (!repos.ContainsKey(type))
            {
                repos[type] = new Repository<TEntity>(_context);
            }

            return (IRepository<TEntity>)repos[type];
        }

        public int Count<TEntity>()
            where TEntity : class
        {
            int count = 0;
            count = _context.ChangeTracker.Entries<TEntity>().Count();
            return count;
        }

        /// <returns>The number of objects in an Added, Modified, or Deleted state</returns>
        public int Commit()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync(CancellationToken.None);
        }

        public async Task<int> CommitAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(obj: this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }
    }
}