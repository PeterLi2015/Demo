using XDropsWater.DataAccess;
using System.Data.Entity;
using System.Linq;

namespace XDropsWater.DataAccess.Interface
{
    /// <summary>
    /// IQueryableUnitOfWork
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Commit all changes made in a container.
        /// </summary>
        ///<remarks>
        /// If the entity have fixed properties and any optimistic concurrency problem exists,  
        /// then an exception is thrown
        ///</remarks>
        void Commit();

        /// <summary>
        /// Commit all changes made in  a container.
        /// </summary>
        ///<remarks>
        /// If the entity have fixed properties and any optimistic concurrency problem exists,
        /// then 'client changes' are refreshed - Client wins
        ///</remarks>
        void CommitAndRefreshChanges();

        /// <summary>
        /// Returns a IDbSet instance for access to entities of the given type in the context, 
        /// the ObjectStateManager, and the underlying store. 
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <returns>Entity Type</returns>
        IDbSet<TEntity> CreateSet<TEntity>() where TEntity : class;

        /// <summary>
        /// Attach this item into "ObjectStateManager"
        /// </summary>
        /// <typeparam name="TEntity">The type of entity</typeparam>
        /// <param name="item">The item </param>
        void Attach<TEntity>(TEntity item) where TEntity : class;

        /// <summary>
        /// Detch this item from "ObjectStateManager"
        /// </summary>
        /// <typeparam name="TEntity">The type of entity</typeparam>
        /// <param name="item">The item</param>
        void Detach<TEntity>(TEntity item) where TEntity : class;

        /// <summary>
        /// Set object as modified
        /// </summary>
        /// <typeparam name="TEntity">The type of entity</typeparam>
        /// <param name="item">The entity item to set as modifed</param>
        void SetModified<TEntity>(TEntity item) where TEntity : class;

        /// <summary>
        /// Apply current values in <paramref name="original"/>
        /// </summary>
        /// <typeparam name="TEntity">The type of entity</typeparam>
        /// <param name="original">The original entity</param>
        /// <param name="current">The current entity</param>
        void ApplyCurrentValues<TEntity>(TEntity original, TEntity current) where TEntity : class;

        /// <summary>
        ///  Set object as modified
        /// </summary>
        /// <typeparam name="TEntity">The type of entity</typeparam>
        /// <param name="oldItem">The old entity item to set as modifed</param>
        /// <param name="newItem">The new entity item to set as modifed</param>
        void SetModified<TEntity>(TEntity oldItem, TEntity newItem) where TEntity : class;

        /// <summary>
        /// Filter invalid records by sql statements.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        IQueryable<TEntity> FilterInvalidData<TEntity>(IQueryable<TEntity> query)
         where TEntity : class,IDeleteStatus;

        /// <summary>
        /// Filter invalid records by memory data.
        /// </summary>
        /// <param name="enetity"></param>
        /// <returns></returns>
        bool IsValidData(IDeleteStatus enetity);

    }
}
