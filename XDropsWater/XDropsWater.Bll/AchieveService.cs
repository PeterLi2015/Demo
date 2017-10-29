using System;
using System.Collections.Generic;
using XDropsWater.Bll.Interface;
using XDropsWater.Dal.Entity;
using XDropsWater.Model;
using XDropsWater.DataAccess;
using XDropsWater.DataAccess.Interface;
using System.Linq;

namespace XDropsWater.Bll
{
    public class AchieveService : BaseService, IAchieveService
    {
        private IUnitOfWork uow = new SimpleWebUnitOfWork();

        public string SumAchieve(string cardNo, int type)
        {
            Repository<AchieveEntity> repo = new Repository<AchieveEntity>(uow);
            var q = repo.FindBy(p => p.CreateBy == this.CurrentUser.ID && p.IsSubmit == false && p.Type == type);
            if (!string.IsNullOrWhiteSpace(cardNo))
            {
                var q1 = q.ToList().Where(p => p.Member.Mobile.Contains(cardNo));
                return q1.Sum(p => p.Amount).ToString();
            }
            return q.Sum(p => p.Amount).ToString();
        }

        public string AchieveHistoryTotal(string cardNo, int batchNo, string dateFrom, string dateTo)
        {
            Repository<AchieveEntity> repo = new Repository<AchieveEntity>(uow);
            var q = repo.FindBy(p => p.CreateBy == this.CurrentUser.ID && p.IsSubmit == true);
            IEnumerable<AchieveEntity> list = null;
            if (!string.IsNullOrWhiteSpace(dateFrom))
            {
                var _d = new DateTime(1900, 1, 1);
                var _dateFrom = _d;
                DateTime.TryParse(dateFrom, out _dateFrom);
                if (_dateFrom > _d)
                {
                    q = q.Where(p => p.CreateOn >= _dateFrom);
                }
            }
            if (!string.IsNullOrWhiteSpace(dateTo))
            {
                var _d = new DateTime(1900, 1, 1);
                var _dateTo = _d;
                DateTime.TryParse(dateTo, out _dateTo);
                if (_dateTo > _d)
                {
                    _dateTo = _dateTo.AddDays(1);
                    q = q.Where(p => p.CreateOn <= _dateTo);
                }
            }
            if (!string.IsNullOrWhiteSpace(cardNo))
                list = q.ToList().Where(p => p.Member.Mobile == cardNo);
            if (batchNo != 0)
                list = q.Where(p => p.BatchNumber == batchNo);

            return list == null ? q.Sum(p => p.Amount).ToString() : list.Sum(p => p.Amount).ToString();

        }

        private Guid GetMemberID(string idCardNo)
        {
            Repository<MemberEntity> repo = new Repository<MemberEntity>(uow);
            var model = repo.FindBy(p => p.Mobile == idCardNo).FirstOrDefault();
            if (model == null)
                throw new Exception("身份证号码不存在");
            else
                return model.ID;
        }

        public ErrorCodes Add(AchieveModel model)
        {
            Repository<AchieveEntity> repo = new Repository<AchieveEntity>(uow);
            AchieveEntity achieve = new AchieveEntity();
            achieve.Amount = model.Amount;
            achieve.CreateBy = this.CurrentUser.ID;
            achieve.CreateOn = DateTime.Now;
            achieve.IsSubmit = false;
            achieve.MemberID = GetMemberID(model.MemberIdentityCardNo);
            achieve.BatchNumber = GetBatchNumber(this.CurrentUser.ID, model.Type);
            achieve.Type = model.Type;
            achieve.Score = model.Score;
            repo.Add(achieve);
            uow.Commit();
            return ErrorCodes.Successed;
        }
        public string Delete(Guid id)
        {
            Repository<AchieveEntity> repo = new Repository<AchieveEntity>(uow);
            var entity = repo.FindBy(p => p.ID == id).FirstOrDefault();
            if (entity != null)
            {
                repo.Remove(entity);
                uow.Commit();
            }
            else
            {
                return "您要删除的记录不存在";
            }
            return ExecuteResult.Success.ToString();
        }

        /// <summary>
        /// 业绩提交
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string Submit(int type)
        {
            Repository<AchieveEntity> repo = new Repository<AchieveEntity>(uow);
            foreach (var achieve in repo.FindBy(o => o.CreateBy == this.CurrentUser.ID && o.Type == type))
            {
                achieve.IsSubmit = true;
                repo.Update(achieve);
            }
            uow.Commit();
            return ExecuteResult.Success.ToString();
        }

        private int GetBatchNumber(Guid createBy, int type)
        {
            Repository<AchieveEntity> repo = new Repository<AchieveEntity>(uow);
            List<AchieveEntity> list = repo.FindBy(p => p.CreateBy == createBy && p.IsSubmit == true && p.Type == type).ToList();
            if (list == null || !list.Any())
            {
                return 1;
            }
            else
            {
                return list.Max(p => p.BatchNumber.Value) + 1;
            }
        }


        public List<AchieveModel> History(string cardNo, string dateFrom, string dateTo, int type, ref int total, int page = 1, int rows = 10)
        {
            throw new NotImplementedException();
        }

        public List<AchieveModel> Get(string cardNo, int type)
        {
            throw new NotImplementedException();
        }
    }

}
