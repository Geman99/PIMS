// PIMS.Repository/IUnitOfWork.cs
using PIMS.Repository.Repositories;

namespace PIMS.Repository
{
    public interface IUnitOfWork
    {
        /// <returns>The number of objects in an Added, Modified, or Deleted state</returns>
        int Commit();

        /// <returns>The number of objects in an Added, Modified, or Deleted state asynchronously</returns>
        Task<int> CommitAsync();

        /// <summary>
        /// Commit asynchronously with CancellationToken
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> CommitAsync(CancellationToken cancellationToken);

        /// <returns>Repository</returns>
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        void Dispose();

        int Count<TEntity>() where TEntity : class;
    }
}