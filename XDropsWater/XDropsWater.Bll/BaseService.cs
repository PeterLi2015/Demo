using XDropsWater.Bll.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDropsWater.Model;
using AutoMapper;
using XDropsWater.Dal.Entity;
using XDropsWater.DataAccess.Interface;
using XDropsWater.DataAccess;

namespace XDropsWater.Bll
{
    public abstract class BaseService : IService
    {
        private IUnitOfWork uow = new SimpleWebUnitOfWork();

        private object OrderNoLock = new object();

        public UserSummary CurrentUser
        {
            get; set;
        }

        /// <summary>
        /// 计算起始行数，结束行数，总行数
        /// </summary>
        /// <param name="model">实体对象</param>
        /// <param name="page">页码</param>
        /// <param name="size">每页显示行数</param>
        /// <param name="totalCount">总行数</param>
        public void CalculateRowNo(BaseSummary model, IEnumerable<IBaseModel> list, int page, int size, int totalCount)
        {
            model.TotalCount = totalCount;
            model.TotalPages = (int)Math.Ceiling((decimal)totalCount / size);
            model.RowFrom = (page - 1) * size + 1;
            model.RowTo = page * size < totalCount ? page * size : totalCount;
            var rowNo = model.RowFrom;
            list.Each(o =>
            {
                o.No = rowNo;
                rowNo++;
            });
        }

        /// <summary>
        /// get current order number
        /// </summary>
        /// <returns></returns>
        public string GetOrderNo()
        {
            lock (OrderNoLock)
            {
                var db = new Repository<SystemConfigEntity>(uow);
                var orderNoConfig = db.FindBy(o => o.Name.Equals(XDropsWater.Dal.Entity.GlobalConstants.OrderNo)).Single();
                var currentOrderNo = orderNoConfig.ConfigValue;

                string sOrderNo = string.Empty;
                //same day
                if (currentOrderNo.Substring(0, 8).Equals(DateTime.Now.ToString("yyyyMMdd")))
                {
                    var iNo = int.Parse(currentOrderNo.Substring(8, 4)) + 1;

                    var length = iNo.ToString().Length;
                    switch (length)
                    {
                        case 1:
                            {
                                sOrderNo = "000" + iNo.ToString();
                            }
                            break;
                        case 2:
                            {
                                sOrderNo = "00" + iNo.ToString();
                            }
                            break;
                        case 3:
                            {
                                sOrderNo = "0" + iNo.ToString();
                            }
                            break;
                        case 4:
                            {
                                sOrderNo = iNo.ToString();
                            }
                            break;
                        default:
                            break;
                    }
                }
                else // different day
                {
                    sOrderNo = DateTime.Now.ToString("yyyyMMdd") + XDropsWater.Dal.Entity.GlobalConstants.OrderNoDefault;
                }

                // update order no to system config
                orderNoConfig.ConfigValue = sOrderNo;
                db.Update(orderNoConfig);
                uow.Commit();

                return currentOrderNo;
            }
        }
    }
}
