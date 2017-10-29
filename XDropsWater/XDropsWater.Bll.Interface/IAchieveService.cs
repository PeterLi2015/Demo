
using System;
using System.Collections.Generic;
using XDropsWater.Model;
namespace XDropsWater.Bll.Interface
{
    public interface IAchieveService : IService
    {
        List<AchieveModel> Get(string cardNo, int type);
        ErrorCodes Add(AchieveModel model);
        string Submit(int type);
        string Delete(Guid id);
        /// <summary>
        /// get achieve history
        /// </summary>
        /// <param name="batchNo"></param>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        List<AchieveModel> History(string cardNo, string dateFrom, string dateTo, int type, ref int total, int page = 1, int rows = 10);
        /// <summary>
        /// get the total of Achieve
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        string SumAchieve(string cardNo, int type);
        /// <summary>
        /// get the total of Achieve History
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="batchNo"></param>
        /// <returns></returns>
        string AchieveHistoryTotal(string cardNo, int batchNo, string dateFrom, string dateTo);
    }
}
