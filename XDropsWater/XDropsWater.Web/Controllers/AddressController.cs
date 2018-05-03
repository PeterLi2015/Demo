using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XDropsWater.Bll;
using XDropsWater.Bll.Interface;
using XDropsWater.Model;
using XDropsWater.Model.Register;

namespace XDropsWater.Web.Controllers
{
    public class AddressController : Controller
    {
        public IMemberAddressService service;

        public AddressController(IMemberAddressService service)
        {
            this.service = service;
        }

        [HttpPost]
        public bool CheckAddressUpdate()
        {
            bool result = false;
            result = service.CheckAddressUpdate();
            return result;
        }
        [HttpPost]
        public ActionResult UpdateAddress(MemberAddress addressModel)
        {
            service.UpdateAddress(addressModel);
            return new EmptyResult();
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
