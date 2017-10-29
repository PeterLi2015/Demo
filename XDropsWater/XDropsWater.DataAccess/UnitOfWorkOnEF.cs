using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using XDropsWater.DataAccess;
using System.Data;
using XDropsWater.DataAccess.Interface;
using System.Runtime.Remoting.Messaging;

namespace XDropsWater.DataAccess
{
    /// <summary>
    /// Unit of Work
    /// </summary>
    public abstract class UnitOfWorkOnEF : DbContext, IUnitOfWork
    {
        /// <summary>
        /// Set the Resolve Concurrency Mode 
        /// </summary>
        public ResolveConcurrencyMode ResolveConcurrencyMode { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public UnitOfWorkOnEF()
            : base()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="nameOrConnectionString">数据库连接名称或数据库连接字符串</param>
        public UnitOfWorkOnEF(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="nameOrConnectionString">数据库连接名称或数据库连接字符串</param>
        /// <param name="model"></param>
        public UnitOfWorkOnEF(string nameOrConnectionString, System.Data.Entity.Infrastructure.DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="existingConnection"></param>
        /// <param name="contextOwnsConnection"></param>
        public UnitOfWorkOnEF(System.Data.Common.DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="existingConnection"></param>
        /// <param name="model"></param>
        /// <param name="contextOwnsConnection"></param>
        public UnitOfWorkOnEF(System.Data.Common.DbConnection existingConnection, System.Data.Entity.Infrastructure.DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
        }

        /// <summary>
        /// Create Set for Type of Entity
        /// </summary>
        /// <typeparam name="TEntity">Type of Entity</typeparam>
        /// <returns>DB Set of Entity</returns>
        public IDbSet<TEntity> CreateSet<TEntity>()
           where TEntity : class
        {
            return base.Set<TEntity>();
        }

        /// <summary>
        /// Set Entity State to Unchanged
        /// </summary>
        /// <typeparam name="TEntity">Type of Entity</typeparam>
        /// <param name="item">Entity Object</param>
        public void Attach<TEntity>(TEntity item)
            where TEntity : class
        {
            //attach and set as unchanged
            base.Entry<TEntity>(item).State = EntityState.Unchanged;
        }

        /// <summary>
        /// Set Entity State to Modified
        /// </summary>
        /// <typeparam name="TEntity">Type of Entity</typeparam>
        /// <param name="item">Entity Object</param>
        public void SetModified<TEntity>(TEntity item)
            where TEntity : class
        {
            //this operation also attach item in object state manager
            base.Entry<TEntity>(item).State = EntityState.Modified;
        }

        /// <summary>
        /// Set Original Value to Current
        /// </summary>
        /// <typeparam name="TEntity">Type of Entity</typeparam>
        /// <param name="original">Oraginal Entity</param>
        /// <param name="current">Current Entity</param>
        public void ApplyCurrentValues<TEntity>(TEntity original, TEntity current)
            where TEntity : class
        {
            //if not is attached, attach original and set current values
            base.Entry<TEntity>(original).CurrentValues.SetValues(current);
        }

        /// <summary>
        /// Set Entity State to Detached
        /// </summary>
        /// <typeparam name="TEntity">Type of Entity</typeparam>
        /// <param name="item"></param>
        public void Detach<TEntity>(TEntity item) where TEntity : class
        {
            base.Entry<TEntity>(item).State = EntityState.Detached;
        }

        /// <summary>
        /// Modify Entity Value and Set Entity to Unchanged
        /// </summary>
        /// <typeparam name="TEntity">Type of Entity</typeparam>
        /// <param name="oldItem">Original Entity</param>
        /// <param name="newItem">Current Entity</param>
        public void SetModified<TEntity>(TEntity oldItem, TEntity newItem) where TEntity : class
        {
            Attach(oldItem);
            base.Entry<TEntity>(oldItem).CurrentValues.SetValues(newItem);
        }

        /// <summary>
        /// Commit Changes
        /// </summary>
        public virtual void Commit()
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    base.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    // Update the values of the entity that failed to save from the store 
                    if (ResolveConcurrencyMode == ResolveConcurrencyMode.StoreWins)
                    {
                        //ex.Entries.Single().Reload(); 
                        ex.Entries.ToList()
                                  .ForEach(entry =>
                                  {
                                      entry.Reload();
                                  });
                    }
                    // Update original values from the database 
                    else if (ResolveConcurrencyMode == ResolveConcurrencyMode.ClientWins)
                    {
                        ex.Entries.ToList()
                                  .ForEach(entry =>
                                  {
                                      entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                                  });
                    }
                    else if (ResolveConcurrencyMode == ResolveConcurrencyMode.UserResolveConcurrency)
                    {
                        ex.Entries.ToList()
                                  .ForEach(entry =>
                                  {
                                      // Get the current entity values and the values in the database 
                                      //var entry = ex.Entries.Single();
                                      var currentValues = entry.CurrentValues;
                                      var databaseValues = entry.GetDatabaseValues();

                                      // Choose an initial set of resolved values. In this case we 
                                      // make the default be the values currently in the database. 
                                      var resolvedValues = databaseValues.Clone();

                                      // Have the user choose what the resolved values should be 
                                      var userResolveConcurrency = entry as IConcurrencyResolveHandler;
                                      if (userResolveConcurrency != null)
                                      {
                                          userResolveConcurrency.HandleUserResolveConcurrency(currentValues, databaseValues, resolvedValues);
                                      }
                                      // Update the original values with the database values and 
                                      // the current values with whatever the user choose. 
                                      entry.OriginalValues.SetValues(databaseValues);
                                      entry.CurrentValues.SetValues(resolvedValues);
                                  });
                    }
                }
            } while (saveFailed);
        }

        /// <summary>
        /// Commit and Refresh Changes
        /// </summary>
        public void CommitAndRefreshChanges()
        {
            bool saveFailed = false;

            do
            {
                try
                {
                    Commit();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    ex.Entries.ToList()
                              .ForEach(entry =>
                              {
                                  entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                              });
                }
            } while (saveFailed);
        }

        /// <summary>
        /// Filter invalid records by memory data.
        /// </summary>
        /// <param name="enetity"></param>
        /// <returns></returns>
        public bool IsValidData(IDeleteStatus enetity)
        {
            return enetity.EntityStatus != DeleteStatus.Deleted;
        }


        /// <summary>
        /// Filter invalid records by sql statements.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<TEntity> FilterInvalidData<TEntity>(IQueryable<TEntity> query)
            where TEntity : class,IDeleteStatus
        {
            return query.Where<TEntity>(sql => sql.EntityStatus != DeleteStatus.Deleted);
        }
    }

}
