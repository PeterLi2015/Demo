using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDropsWater.DataAccess;
using System.Data.Entity;
using XDropsWater.Dal.DataAccess.Entity;
using XDropsWater.DataAccess.Configuration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Data.Entity.Infrastructure.MappingViews;
using System.IO;
using InteractivePreGeneratedViews;

namespace XDropsWater.Dal.Entity
{
    /// <summary>
    /// 用于创建实体类与数据库表之间的关联，搭建EntityFramework
    /// </summary>
    public class SimpleWebUnitOfWork : UnitOfWorkOnEF
    {
        private static DbMappingViewCacheFactory viewCacheFactory;

        private static DbMappingViewCacheFactory ViewCacheFactory
        {
            get
            {
                if (viewCacheFactory == null)
                {
                    string path = DbCacheHelper.EdmxCacheFile;
                    Directory.CreateDirectory(Path.GetDirectoryName(path)); //ensure that directory exists
                    viewCacheFactory = new FileViewCacheFactory(path);
                }
                return viewCacheFactory;
            }
        }

        public SimpleWebUnitOfWork()
            : base("SimpleWebDbCon")
        {
            InteractiveViews.SetViewCacheFactory(this, ViewCacheFactory);
        }

        //public IDbSet<PermissionEntity> Permissions { get; set; }

        //public IDbSet<RoleEntity> Roles { get; set; }

        public IDbSet<UserEntity> Users { get; set; }
        //public IDbSet<ChildOrderEntity> ChildOrders { get; set; }
        public IDbSet<MemberRoleConfigEntity> MemberRoleConfig { get; set; }
        public IDbSet<MemberRoleEntity> MemberRoles { get; set; }
        public IDbSet<OrderEntity> Orders { get; set; }
        public IDbSet<UserRoleEntity> UserRoles { get; set; }

        public IDbSet<SystemConfigEntity> SystemConfig { get; set; }

        public IDbSet<UserLogEntity> Userlogs { get; set; }

        //public IDbSet<DepartmentEntity> Departments { get; set; }

        public IDbSet<MemberEntity> Members { get; set; }

        //public IDbSet<MemberHistoryEntity> MembersHistory { get; set; }

        //public IDbSet<AchieveEntity> Achieves { get; set; }

        public IDbSet<LogEntity> Logs { get; set; }

        public IDbSet<RoleUpgradeEntity> RoleUpgrade { get; set; }

        public IDbSet<ParentChildEntity> ParentChild { get; set; }

        public IDbSet<DirectorEntity> Director { get; set; }

        public IDbSet<IdentityCodeEntity> IdentityCode { get; set; }

        public IDbSet<ProductEntity> Products { get; set; }

        public IDbSet<StockEntity> Stock { get; set; }

        public IDbSet<OrderDetailsEntity> OrderDetails { get; set; }

        public IDbSet<ProductMemberRoleEntity> ProductMemberRole { get; set; }

        public IDbSet<ShoppingCartEntity> ShoppingCart { get; set; }

        /// <summary>
        /// 会员产品表
        /// </summary>
        public IDbSet<MemberProductEntity> MemberProduct { get; set; }

        /// <summary>
        /// 会员产品识别码
        /// </summary>
        public IDbSet<MemberProductCodeEntity> MemberProductCode { get; set; }

        /// <summary>
        /// 快递
        /// </summary>
        public IDbSet<ExpressEntity> Express { get; set; }

        public IDbSet<AddressEntity> Address { get; set; }

        public IDbSet<MemberAddressEntity> MemberAddress { get; set; }

        //public IDbSet<LevelEntity> Levels { get; set; }

        public DatabaseGeneratedOption EntityIDDatabaseGeneratedOption { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //#region relationship between Users and Roles
            //modelBuilder.Entity<RoleEntity>()
            //    .HasMany(e => e.Users)
            //    .WithMany(e => e.Roles)
            //    .Map(m =>
            //        {
            //            m.ToTable("RoleUserRel");
            //            m.MapLeftKey("RoleID");
            //            m.MapRightKey("UserID");
            //        });
            //#endregion

            //#region relationship between Roles and Permissions
            //modelBuilder.Entity<RoleEntity>()
            //    .HasMany(e => e.Permissions)
            //    .WithMany(e => e.Roles)
            //    .Map(m =>
            //    {
            //        m.ToTable("RolePermissionRel");
            //        m.MapLeftKey("RoleID");
            //        m.MapRightKey("PermissionID");
            //    });
            //#endregion

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<SimpleWebUnitOfWork, SimpleWebMigrationsConfiguration>());


            #region ConfigMap
            var entityTypes = GetAllRegEntityTypes();
            foreach (var entityType in entityTypes)
            {
                var idType = entityType.GetProperty("ID").PropertyType;
                Type closedGenericListType = typeof(ExtendEntityTypeConfiguration<,>).MakeGenericType(entityType, idType);
                var iConfig = Activator.CreateInstance(closedGenericListType) as IEntityTypeConfiguration;
                iConfig.EntityIDDatabaseGeneratedOption = this.EntityIDDatabaseGeneratedOption;

                var entity = Activator.CreateInstance(entityType);
                var mapConfigMethod = entity.GetType().GetMethod("ConfigMap", BindingFlags.NonPublic | BindingFlags.Instance);
                mapConfigMethod.Invoke(entity, new object[] { iConfig });

                //reg config mapping
                modelBuilder.Configurations.Add(iConfig as dynamic);
            }
            #endregion

            modelBuilder.Entity<OrderEntity>()
                .HasRequired(o => o.Member)
                .WithMany()
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<MemberEntity>().HasRequired(p => p.IdentityCodes).WithOptional();

            //modelBuilder.Entity<MemberEntity>()
            //    .HasRequired(x => x.IdentityCodes)
            //    .WithOptional()
            //    .HasForeignKey(x => x.IdentityCodeID);

            modelBuilder.Entity<OrderEntity>().Property(order => order.Quantity).HasPrecision(18, 6);

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            foreach (var dee in this.ChangeTracker.Entries())
            {
                if (dee.State != System.Data.Entity.EntityState.Added) continue;
                Guid guid = (dee.Entity as BaseEntity<Guid>).ID;
                if (guid.ToString().Equals("00000000-0000-0000-0000-000000000000"))
                    (dee.Entity as BaseEntity<Guid>).ID = Guid.NewGuid();
            }
            return base.SaveChanges();
        }

        public override void Commit()
        {
            foreach (var dee in this.ChangeTracker.Entries())
            {
                if (dee.State != System.Data.Entity.EntityState.Added) continue;
                Guid guid = (dee.Entity as BaseEntity<Guid>).ID;
                if (guid.ToString().Equals("00000000-0000-0000-0000-000000000000"))
                    (dee.Entity as BaseEntity<Guid>).ID = Guid.NewGuid();
            }
            base.Commit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Type[] GetAllRegEntityTypes()
        {
            var typeList = new List<Type>();
            var ps = this.GetType().GetProperties();
            foreach (var p in ps)
            {
                if (p.PropertyType.Name != typeof(IDbSet<>).Name)
                {
                    continue;
                }
                var dbEntityTypes = p.PropertyType.GetGenericArguments();
                if (dbEntityTypes == null || dbEntityTypes.Length == 0) continue;
                if (!typeList.Contains(dbEntityTypes[0]))
                {
                    typeList.Add(dbEntityTypes[0]);
                }
            }
            return typeList.ToArray();
        }
    }
}
