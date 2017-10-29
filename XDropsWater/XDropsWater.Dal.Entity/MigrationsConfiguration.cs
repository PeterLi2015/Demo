using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using XDropsWater.DataAccess;
using XDropsWater.CrossCutting;

namespace XDropsWater.Dal.Entity
{
    public class SimpleWebMigrationsConfiguration : DbMigrationsConfiguration<SimpleWebUnitOfWork>
    {
        public SimpleWebMigrationsConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(SimpleWebUnitOfWork context)
        {
            base.Seed(context);

            //#region create unique index for Members
            //context.Database.ExecuteSqlCommand("IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Members]') AND name = N'IX_IdentityCardNo') BEGIN CREATE UNIQUE INDEX IX_IdentityCardNo ON Members(IdentityCardNo) END");
            //#endregion

            //Guid departmentID = Guid.NewGuid();
            //if (context.Departments.FirstOrDefault(p => p.Name == "上海总部") == null)
            //{
            //    context.Departments.Add(new DepartmentEntity() { ID = departmentID, Name = "上海总部", Description = "上海总部" });
            //    context.SaveChanges();
            //}
            //if (context.Permissions.FirstOrDefault(p => p.Name == "系统管理") == null)
            //{
            //    context.Permissions.Add(new PermissionEntity() { ShortName = "system", Name = "系统管理", Description = "系统管理" });
            //    context.Permissions.Add(new PermissionEntity() { ShortName = "wd", Name = "网点操作", Description = "网点操作" });
            //    context.Permissions.Add(new PermissionEntity() { ShortName = "zl", Name = "资料操作", Description = "资料操作" });
            //    context.Permissions.Add(new PermissionEntity() { ShortName = "qd", Name = "清单操作", Description = "清单操作" });
            //    context.Permissions.Add(new PermissionEntity() { ShortName = "dc", Name = "电查操作", Description = "电查操作" });
            //    context.SaveChanges();
            //}
            //if (context.Users.FirstOrDefault(p => p.Name == "admin") == null)
            //{
            //    DepartmentEntity de = context.Departments.FirstOrDefault(p => p.ID == departmentID);
            //    context.Users.Add(new UserEntity()
            //    {
            //        Name = "admin",
            //        Account = "admin",
            //        Password = "admin",
            //        Sex = 1,
            //        Telphone = "12345678901",
            //        MobTel = "12345678901",
            //        //Department = de,
            //        CreateOn = DateTime.Now,
            //        UpdateOn = DateTime.Now
            //    });
            //    context.SaveChanges();
            //}
            //context.SystemConfig.Add(new SystemConfigEntity() { Name = "GroupID", ConfigValue = "0", Description = "新增票据和划款信息时，需要读取的分组编号。" });

            // setup order number to system configuration
            var orderNoExisting = context.SystemConfig.Any(o => o.Name.Equals(GlobalConstants.OrderNo, StringComparison.OrdinalIgnoreCase));
            if (!orderNoExisting)
            {
                var systemConfigEntity = new SystemConfigEntity();
                systemConfigEntity.ConfigValue = DateTime.Now.ToString("yyyyMMdd") + GlobalConstants.OrderNoDefault;
                systemConfigEntity.Description = GlobalConstants.OrderNoDescription;
                systemConfigEntity.EntityStatus = (int)SystemConfigStatus.Available;
                systemConfigEntity.ID = Guid.NewGuid();
                systemConfigEntity.Name = GlobalConstants.OrderNo;
                context.SystemConfig.Add(systemConfigEntity);
            }
            context.SaveChanges();
        }
    }
}
