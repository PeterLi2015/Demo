using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using XDropsWater.DataAccess.Entity;

namespace XDropsWater.Dal.Entity
{
    public class MemberAddressEntity : BaseEncryptCreateUpdateLogEntity<Guid>
    {
        public Guid MemberID { get; set; }

        [MaxLength(50)]
        public string ProvinceName { get; set; }

        [MaxLength(100)]
        public string ProvinceCode { get; set; }

        [MaxLength(50)]
        public string CityName { get; set; }

        [MaxLength(100)]
        public string CityCode { get; set; }

        [MaxLength(50)]
        public string DistrictName { get; set; }

        [MaxLength(100)]
        public string DistrictCode { get; set; }

        [MaxLength(50)]
        public string StreetName { get; set; }

        [MaxLength(100)]
        public string StreetCode { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }
    }
}
