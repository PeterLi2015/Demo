using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XDropsWater.Model;

namespace XDropsWater.Web
{
    public class Province : IProvince
    {
        public IEnumerable<AddressModel> GetAllProvinces()
        {
            List<AddressModel> result = new List<AddressModel>();

            #region 华北
            result.Add(new AddressModel()
            {
                Province = "北京市",
                ProvinceCode = "beijing"
            });

            result.Add(new AddressModel()
            {
                Province = "天津市",
                ProvinceCode = "tianjin"
            });

            result.Add(new AddressModel()
            {
                Province = "河北省",
                ProvinceCode = "hebei"
            });

            result.Add(new AddressModel()
            {
                Province = "山西省",
                ProvinceCode = "shanxi"
            });

            result.Add(new AddressModel()
            {
                Province = "内蒙古自治区",
                ProvinceCode = "neimenggu"
            });
            #endregion

            #region 东北
            result.Add(new AddressModel()
            {
                Province = "辽宁省",
                ProvinceCode = "liaoning"
            });


            result.Add(new AddressModel()
            {
                Province = "吉林省",
                ProvinceCode = "jilin"
            });

            result.Add(new AddressModel()
            {
                Province = "黑龙江省",
                ProvinceCode = "heilongjiang"
            });
            #endregion

            #region 华东
            result.Add(new AddressModel()
            {
                Province = "上海市",
                ProvinceCode = "shanghai"
            });
            result.Add(new AddressModel()
            {
                Province = "江苏省",
                ProvinceCode = "jiangsu"
            });
            result.Add(new AddressModel()
            {
                Province = "浙江省",
                ProvinceCode = "zhejiang"
            });
            result.Add(new AddressModel()
            {
                Province = "安徽省",
                ProvinceCode = "anhui"
            });
            result.Add(new AddressModel()
            {
                Province = "福建",
                ProvinceCode = "fujian"
            });
            result.Add(new AddressModel()
            {
                Province = "江西省",
                ProvinceCode = "jiangxi"
            });
            result.Add(new AddressModel()
            {
                Province = "山东省",
                ProvinceCode = "shandong"
            });
            #endregion

            #region 中南
            result.Add(new AddressModel()
            {
                Province = "河南省",
                ProvinceCode = "henan"
            });
            result.Add(new AddressModel()
            {
                Province = "湖北省",
                ProvinceCode = "hubei"
            });
            result.Add(new AddressModel()
            {
                Province = "广东省",
                ProvinceCode = "guangdong"
            });
            result.Add(new AddressModel()
            {
                Province = "广西壮族自治区",
                ProvinceCode = "guangxi"
            });
            result.Add(new AddressModel()
            {
                Province = "海南省",
                ProvinceCode = "hainan"
            });
            #endregion

            #region 西南
            result.Add(new AddressModel()
            {
                Province = "重庆市",
                ProvinceCode = "chongqing"
            });
            result.Add(new AddressModel()
            {
                Province = "四川省",
                ProvinceCode = "sichuan"
            });
            result.Add(new AddressModel()
            {
                Province = "贵州省",
                ProvinceCode = "guizhou"
            });
            result.Add(new AddressModel()
            {
                Province = "云南省",
                ProvinceCode = "yunnan"
            });
            result.Add(new AddressModel()
            {
                Province = "西藏自治区",
                ProvinceCode = "xizang"
            });
            #endregion

            #region 西北
            result.Add(new AddressModel()
            {
                Province = "陕西省",
                ProvinceCode = "shanxi"
            });
            result.Add(new AddressModel()
            {
                Province = "甘肃省",
                ProvinceCode = "gansu"
            });
            result.Add(new AddressModel()
            {
                Province = "青海省",
                ProvinceCode = "qinghai"
            });
            result.Add(new AddressModel()
            {
                Province = "宁夏回族自治区",
                ProvinceCode = "ningxia"
            });
            result.Add(new AddressModel()
            {
                Province = "新疆维吾尔自治区",
                ProvinceCode = "xinjiang"
            });
            #endregion

            #region 港澳台
            result.Add(new AddressModel()
            {
                Province = "香港特别行政区",
                ProvinceCode = "xianggang"
            });
            result.Add(new AddressModel()
            {
                Province = "澳门特别行政区",
                ProvinceCode = "aomen"
            });
            result.Add(new AddressModel()
            {
                Province = "台湾省",
                ProvinceCode = "taiwan"
            });
            #endregion

            return result.OrderBy(o => o.ProvinceCode);
        }
    }
}