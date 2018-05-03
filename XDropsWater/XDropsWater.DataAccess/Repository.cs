using XDropsWater.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using XDropsWater.DataAccess.Interface;
using System.Linq.Expressions;

namespace XDropsWater.DataAccess
{
    /// <summary>
    /// Repository
    /// </summary>
    /// <typeparam name="TEntity">Type of Entity</typeparam>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private IUnitOfWork unitOfWork;

        /// <summary>
        /// Constrcutor
        /// </summary>
        /// <param name="uow"></param>
        public Repository(IUnitOfWork uow)
        {
            if (uow == null)
                throw new ArgumentNullException("uow", "unitOfWork is null");

            this.unitOfWork = uow;


        }

        /// <summary>
        /// Unit of Work
        /// </summary>
        protected IUnitOfWork UnitOfWork
        {
            get { return this.unitOfWork; }
        }

        #region IRepository Members

        /// <summary>
        /// Get All Entity Enum
        /// </summary>
        /// <returns>Enumertor of Entity</returns>
        public virtual IQueryable<TEntity> GetAll()
        {
            return unitOfWork.CreateSet<TEntity>().AsNoTracking();
        }

        /// <summary>
        /// Find specific entity by key values
        /// </summary>
        /// <param name="keyValues">entity key value</param>
        /// <returns>Entity object</returns>
        public virtual TEntity Find(params object[] keyValues)
        {
            return unitOfWork.CreateSet<TEntity>().Find(keyValues);
        }

        /// <summary>
        /// Find Object by LINQ expression
        /// </summary>
        /// <param name="predicate">LINQ expression</param>
        /// <returns>Entity Object</returns>
        public virtual IQueryable<TEntity> FindBy(Func<TEntity, bool> predicate)
        {
            return unitOfWork.CreateSet<TEntity>().Where(predicate).AsQueryable().AsNoTracking();
        }

        /// <summary>
        /// Get Single Entity by LINQ
        /// </summary>
        /// <param name="predicate">LINQ</param>
        /// <returns>Type of Entity</returns>
        public virtual TEntity Single(Func<TEntity, bool> predicate)
        {
            return unitOfWork.CreateSet<TEntity>().AsNoTracking().Single(predicate);
        }

        /// <summary>
        /// Add Entity to ObjectStateManager
        /// </summary>
        /// <param name="item">Entity</param>
        public virtual void Add(TEntity item)
        {
            if (item != (TEntity)null)
            {
                unitOfWork.CreateSet<TEntity>().Add(item);
            }
        }

        /// <summary>
        /// Remove Entity from ObjectStateManager
        /// </summary>
        /// <param name="item">Entity</param>
        public virtual void Remove(TEntity item)
        {
            if (item != (TEntity)null)
            {
                unitOfWork.CreateSet<TEntity>().Remove(item);
            }
        }

        /// <summary>
        /// Update Entity Status in ObjectStateManager
        /// </summary>
        /// <param name="item">Entity</param>
        public virtual void Update(TEntity item)
        {
            unitOfWork.SetModified<TEntity>(item);
        }

        /// <summary>
        /// Update Entity Status in ObjectStateManager
        /// </summary>
        /// <param name="oldItem">Entity Before Change</param>
        /// <param name="newItem">Entity After Change</param>
        public virtual void Update(TEntity oldItem, TEntity newItem)
        {
            if (oldItem != null && newItem != null)
            {
                unitOfWork.SetModified(oldItem, newItem);
            }
        }

        /// <summary>
        /// Set Entity State to Unchanged
        /// </summary>
        /// <param name="item">Entity Object</param>
        public virtual void Attach(TEntity item)
        {
            if (item != (TEntity)null)
                unitOfWork.Attach<TEntity>(item);
        }

        /// <summary>
        /// Set Entity State to Detached
        /// </summary>
        /// <param name="item">Entity Object</param>
        public virtual void Detach(TEntity item)
        {
            if (item != (TEntity)null)
                unitOfWork.Detach<TEntity>(item);
        }

        #endregion IRepository Members

        /// <summary>
        /// Unit of Work
        /// </summary>
        IUnitOfWork IRepository<TEntity>.UnitOfWork
        {
            get { return unitOfWork; }
        }

        /// <summary>
        /// Get all elements of type {T} in repository
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageCount">Number of elements in each page</param>
        /// <param name="orderByExpression">Order by expression for this query</param>
        /// <param name="ascending">Specify if order is ascending</param>
        /// <param name="total">返回总的行数</param>
        /// <returns>List of selected elements</returns>
        public IQueryable<TEntity> GetPaged<TSortField>(int pageIndex, int pageCount, Func<TEntity, TSortField> orderByExpression, bool ascending, out int total)
        {
            var tempData = GetAll();
            total = tempData.Count();
            if (ascending)
            {
                tempData = tempData.OrderBy(orderByExpression).
                    Skip<TEntity>(pageIndex * pageCount).
                    Take<TEntity>(pageCount).AsQueryable();
            }
            else
            {
                tempData = tempData.OrderByDescending(orderByExpression).
                    Skip<TEntity>(pageIndex * pageCount).
                    Take<TEntity>(pageCount).AsQueryable();
            }

            return tempData.AsQueryable();
        }

        /// <summary>
        /// Filter invalid records by memory data.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsValidData(IDeleteStatus entity)
        {
            return ((IUnitOfWork)unitOfWork).IsValidData(entity);
        }

        /// <summary>
        /// Filter invalid records by sql statements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<T> FilterInvalidData<T>(IQueryable<T> query)
            where T : class,IDeleteStatus
        {
            return ((IUnitOfWork)unitOfWork).FilterInvalidData<T>(query);
        }

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
        public IQueryable<TEntity> GetPaged<TSortField>(int pageIndex, int pageCount, Func<TEntity, bool> filterExpression, Func<TEntity, TSortField> orderByExpression, bool ascending, out int total)
        {
            var tempData = FindBy(filterExpression);
            total = tempData.Count();
            if (ascending)
            {
                tempData = tempData.OrderBy(orderByExpression).
                    Skip<TEntity>(pageIndex * pageCount).
                    Take<TEntity>(pageCount).AsQueryable();
            }
            else
            {
                tempData = tempData.OrderByDescending(orderByExpression).
                    Skip<TEntity>(pageIndex * pageCount).
                    Take<TEntity>(pageCount).AsQueryable();
            }

            return tempData.AsQueryable();
        }


        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IQueryable<TEntity> GetPaged(int pageIndex, int pageCount, out int total)
        {
            var tempData = GetAll();
            total = tempData.Count();
            tempData = tempData.Skip<TEntity>(pageIndex * pageCount).
                    Take<TEntity>(pageCount).AsQueryable();
            return tempData.AsQueryable();
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <param name="filterExpression"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IQueryable<TEntity> GetPaged(int pageIndex, int pageCount, Func<TEntity, bool> filterExpression, out int total)
        {
            var tempData = FindBy(filterExpression);
            total = tempData.Count();
            tempData = tempData.Skip<TEntity>(pageIndex * pageCount).
                    Take<TEntity>(pageCount).AsQueryable();
            return tempData.AsQueryable();
        }

        public virtual IQueryable<T> Find<T, TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderBy) where T : class
        {
            if (predicate != null)
            {
                if (orderBy != null)
                {
                    return FindAll<T>(predicate).OrderBy(orderBy).AsQueryable<T>(); ;

                }
                else
                {
                    throw new ArgumentNullException("OrderBy value must be passed to Find<T,TKey>.");
                }
            }
            else
            {
                throw new ArgumentNullException("Predicate value must be passed to Find<T,TKey>.");
            }
        }

        public virtual IQueryable<T> FindAll<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            if (predicate != null)
            {
                return unitOfWork.CreateSet<T>().Where(predicate);
            }
            else
            {
                throw new ArgumentNullException("Predicate value must be passed to FindAll<T>.");
            }
        }
    }


}
