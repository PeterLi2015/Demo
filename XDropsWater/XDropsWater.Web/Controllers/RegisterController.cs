using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Unity.Attributes;
using XDropsWater.Bll.Interface;
using XDropsWater.Model;

namespace XDropsWater.Web.Controllers
{
    public class RegisterController : BaseController
    {
        private readonly IRegisterService service;
        private readonly IMemberService memberService;

        [Dependency]
        public IProvince Province { get; set; }

        public RegisterController(IRegisterService service, IMemberService _memberService)
        {
            this.service = service;
            this.memberService = _memberService;
        }

        /// <summary>
        /// 根据电话号码获取代理对象
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetMemberName(string mobile)
        {
            var member = memberService.GetMemberByMobile(mobile);
            return Json(member);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RegisterAccount()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel register)
        {
            service.Register(register);
            return new EmptyResult();
        }

        public ActionResult RegisterUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterUser(Member user)
        {
            service.RegisterUser(user);
            return new EmptyResult();
        }

        public ActionResult RegisterProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterProduct(IEnumerable<ProductRegister> productList)
        {
            service.RegisterProduct(productList);
            return new EmptyResult();
        }



        [HttpPost]
        public ActionResult UpdateAddress()
        {

            //AddressModel province = new AddressModel()
            //{
            //    Province = "海南",
            //    ProvinceCode = "hainan"
            //};

            HttpClient client = new HttpClient();

            //client.DefaultRequestHeaders.Referrer = new Uri("http://www.amap.com");
            //var url = string.Format("http://restapi.amap.com/v3/config/district?keywords={0}&subdistrict=10&key=afb2f4bc069c348df3c9b8bfbb329f8e", province.Province);
            //province.JsonValue = client.GetStringAsync(url).Result;
            //service.UpdateAddress(province);


            var provinceList = Province.GetAllProvinces();
            string url = string.Empty;
            foreach (var province in provinceList)
            {
                client.DefaultRequestHeaders.Referrer = new Uri("http://www.amap.com");
                url = string.Format("http://restapi.amap.com/v3/config/district?keywords={0}&subdistrict=3&key=afb2f4bc069c348df3c9b8bfbb329f8e", province.Province);
                province.JsonValue = client.GetStringAsync(url).Result;
                service.UpdateAddress(province);
            }




            //var result = service.GetAddress(address.Province);
            //var myaddress = JsonConvert.DeserializeObject<Address>(result.JsonValue.Replace("[]", "\"\""));
            ////int count = GetStreetDistrictCount(myaddress.districts);
            //var list = GetStreetDistricts(myaddress.districts);
            return new EmptyResult();
        }
        List<Districts> result = new List<Districts>();
        private IEnumerable<Districts> GetStreetDistricts(IEnumerable<Districts> districts, List<Districts> result)
        {

            districts.ToList().ForEach(o =>
            {
                if (o.districts != null && o.districts.Any())
                {
                    result.AddRange(GetStreetDistricts(o.districts, result));
                }
                else
                {
                    result.Add(o);
                }
            });
            return result;
        }

        private int GetStreetDistrictCount(IEnumerable<Districts> districts)
        {
            int count = 0;
            districts.ToList().ForEach(o =>
            {
                if (o.districts != null && o.districts.Any())
                {
                    count += GetStreetDistrictCount(o.districts);
                }
                else
                {
                    count++;
                }
            });
            return count;
        }

        [HttpPost]
        public ActionResult GetProvinces()
        {
            Districts district = null;
            Model.Province province = null;
            List<Model.Province> result = new List<Model.Province>();
            foreach (var address in AddressCache.Addresses)
            {
                if (address.districts.Any(o => o.level == "province"))
                {
                    district = address.districts.First();
                    province = new Model.Province();
                    province.ProvinceCode = district.adcode + "-" + district.level + "-" + district.name;
                    province.ProvinceName = district.name;
                    result.Add(province);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetCities(string province)
        {
            Districts myProvince = null;
            foreach (var address in AddressCache.Addresses)
            {
                if (address.districts.Any(o => o.name == province))
                {
                    myProvince = address.districts.First(o => o.name == province);
                    if (myProvince != null)
                    {
                        break;
                    }
                }

            }
            City myCity = null;
            List<City> result = new List<City>();
            foreach (var city in myProvince.districts ?? Enumerable.Empty<Districts>())
            {
                myCity = new City();
                myCity.CityCode = city.adcode + "-" + city.level + "-" + city.name;
                myCity.CityName = city.name;
                result.Add(myCity);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetDistricts(string province, string city)
        {
            Districts myProvince = null;
            foreach (var address in AddressCache.Addresses)
            {
                if (address.districts.Any(o => o.name == province))
                {
                    myProvince = address.districts.First(o => o.name == province);
                    if (myProvince != null)
                    {
                        break;
                    }
                }

            }
            Districts myCity = null;
            if (myProvince.districts.Any(o => o.name == city))
            {
                myCity = myProvince.districts.First(o => o.name == city);
            }
            List<District> result = new List<District>();
            if (myCity != null)
            {
                District myDistrict = null;
                foreach (var district in myCity.districts ?? Enumerable.Empty<Districts>())
                {
                    myDistrict = new District();
                    myDistrict.DistrictCode = district.adcode + "-" + district.level + "-" + district.name;
                    myDistrict.DistrictName = district.name;
                    result.Add(myDistrict);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetStreets(string province, string city, string district)
        {
            Districts myProvince = null;
            foreach (var address in AddressCache.Addresses)
            {
                if (address.districts.Any(o => o.name == province))
                {
                    myProvince = address.districts.First(o => o.name == province);
                    if (myProvince != null)
                    {
                        break;
                    }
                }

            }

            List<Street> result = new List<Street>();

            Districts myCity = myProvince.districts.First(o => o.name == city);

            if (myCity != null)
            {
                Districts myDistrict = myCity.districts.First(o => o.name == district);

                if (myDistrict != null)
                {
                    Street myStreet = null;

                    foreach (var tmpDistrict in myDistrict.districts ?? Enumerable.Empty<Districts>())
                    {
                        myStreet = new Street();
                        myStreet.StreetCode = tmpDistrict.adcode + "-" + tmpDistrict.level + "-" + tmpDistrict.name;
                        myStreet.StreetName = tmpDistrict.name;
                        result.Add(myStreet);
                    }
                }
                
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
