namespace XDropsWater.DataAccess.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Linq;

    /// <summary>
    /// Define IRepository Interface
    /// </summary>
    /// <typeparam name="TEntity">Generic Entity Type</typeparam>
    /// <code>public interface IRepository</code>
    public interface IRepository<TEntity>
    {
        /// <summary>
        /// Get the unit of work in this repository
        /// </summary>
        /// <value>IUnitofWork</value>
        IUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// Add item into repository
        /// </summary>
        /// <param name="item">Item to add to repository</param>
        void Add(TEntity item);

        /// <summary>
        /// Delete item 
        /// </summary>
        /// <param name="item">Item to delete</param>
        void Remove(TEntity item);

        /// <summary>
        /// Update Entity Status in ObjectStateManager
        /// </summary>
        /// <param name="item">Entity</param>
        void Update(TEntity item);


        /// <summary>
        /// Get all elements of type {T} in repository
        /// </summary>
        /// <returns>List of selected elements</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Find specific entity by key values
        /// </summary>
        /// <param name="keyValues">entity key value</param>
        /// <returns>Entity object</returns>
        TEntity Find(params object[] keyValues);


        /// <summary>
        /// Find TEntity By Linq Expression
        /// </summary>
        /// <param name="predicate">Linq Expression</param>
        /// <returns>Enumerable Result List</returns>
        IQueryable<TEntity> FindBy(Func<TEntity, bool> predicate);

        /// <summary>
        /// Get Single Entity by LINQ
        /// </summary>
        /// <param name="predicate">LINQ</param>
        /// <returns>Type of Entity</returns>
        TEntity Single(Func<TEntity, bool> predicate);


        /// <summary>
        /// Get all elements of type {T} in repository
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageCount">Number of elements in each page</param>
        /// <param name="total">总的行数</param>
        /// <returns>List of selected elements</returns>
        IQueryable<TEntity> GetPaged(int pageIndex, int pageCount, out int total);


        /// <summary>
        /// Get all elements of type {T} in repository
        /// </summary>
        /// <typeparam name="TSortField"></typeparam>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageCount">Number of elements in each page</param>
        /// <param name="orderByExpression">Order by expression for this query</param>
        /// <param name="ascending">Specify if order is ascending</param>
        /// <param name="total">返回总的行数</param>
        /// <returns>List of selected elements</returns>
        IQueryable<TEntity> GetPaged<TSortField>(int pageIndex, int pageCount, Func<TEntity, TSortField> orderByExpression, bool ascending, out int total);

        /// <summary>
        /// Get all elements of type {T} in repository
        /// </summary>
        /// <typeparam name="TSortField"></typeparam>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageCount">Number of elements in each page</param>
        /// <param name="orderByExpression">Order by expression for this query</param>
        /// <param name="ascending">Specify if order is ascending</param>
        /// <param name="filterExpression">删选数据的表达式</param>
        /// <param name="total">返回总的行数</param>
        /// <returns>List of selected elements</returns>
        IQueryable<TEntity> GetPaged<TSortField>(int pageIndex, int pageCount, Func<TEntity, bool> filterExpression, Func<TEntity, TSortField> orderByExpression, bool ascending, out int total);

        /// <summary>
        /// Get all elements of type {T} in repository
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageCount">Number of elements in each page</param>
        /// <param name="filterExpression">删选数据的表达式</param>
        /// <param name="total">返回总的行数</param>
        /// <returns>List of selected elements</returns>
        IQueryable<TEntity> GetPaged(int pageIndex, int pageCount, Func<TEntity, bool> filterExpression, out int total);


        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="oldItem"></param>
        /// <param name="newItem"></param>
        void Update(TEntity oldItem, TEntity newItem);

        IQueryable<T> Find<T, TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderBy) where T : class;
        IQueryable<T> FindAll<T>(Expression<Func<T, bool>> predicate) where T : class;
    }
}
