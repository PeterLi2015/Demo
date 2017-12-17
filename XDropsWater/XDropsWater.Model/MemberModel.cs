using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace XDropsWater.Model
{
    public class MemberModel : BaseModel<Guid>
    {
        public string IdentityCardNo { get; set; }

        [Display(Name = "姓名")]
        public string MemberName { get; set; }

        [Display(Name = "身份证号码")]
        public string IdentityNo { get; set; }


        public Guid ParentMemberID { get; set; }

        public string BankCardNo { get; set; }

        public string BankName { get; set; }

        public string ParentIdentityCardNo { get; set; }

        /// <summary>
        /// 推荐人身份证号码
        /// </summary>
        public string RecommendIdentityCardNo { get; set; }

        public bool IsNew { get; set; }

        public string IdentityCardPath { get; set; }

        public bool AllowUpdate { get; set; }

        public string ParentName { get; set; }

        /// <summary>
        /// 推荐人姓名
        /// </summary>
        public string RecommendName { get; set; }

        /// <summary>
        /// 身份信息是否确认过
        /// </summary>
        public string HasChecked { get; set; }

        /// <summary>
        /// 谁确认的
        /// </summary>
        public string CheckBy { get; set; }

        /// <summary>
        /// 什么时间确认的
        /// </summary>
        public DateTime CheckOn { get; set; }

        /// <summary>
        /// 代理级别ID
        /// </summary>
        public int CardLevelID { get; set; }

        /// <summary>
        /// 代理级别ID
        /// </summary>
        public int AgencyLevelID { get; set; }

        [Display(Name = "手机号码")]
        public string Mobile { get; set; }

        [Display(Name = "地址")]
        public string Address { get; set; }

        public string StoreName { get; set; }

        public string Account { get; set; }

        public string StoreManagerName { get; set; }

        /// <summary>
        /// 部门积分
        /// </summary>
        public int DepartScore { get; set; }

        /// <summary>
        /// 部门名称(A,B,C)
        /// </summary>
        public string DepartName { get; set; }

        /// <summary>
        /// 部门区代数量
        /// </summary>
        public int DepartDistrctAgencyQuantity { get; set; }

        public int MemberRoleID { get; set; }

        public string ParentMemberMobile { get; set; }

        [Display(Name = "当前级别")]
        public string MemberRoleName { get; set; }

        [Display(Name = "升级订货量")]
        public int RiseQuantity { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        [Display(Name = "库存")]
        public decimal TotalQuantity { get; set; }

        public int UserRoleId { get; set; }

        [Display(Name = "当前角色库存")]
        public decimal CurrentRoleQuantity { get; set; }

        [Display(Name = "有效省代")]
        public int ProvinceAvailable { get; set; }

        public string ProvinceAvailableDesc { get; set; }

        [Display(Name = "有效总代")]
        public int GeneralAvailable { get; set; }

        public string GeneralAvailableDesc { get; set; }
    }

    public class AddMemberModel : BaseModel<Guid>
    {
        [Required(ErrorMessage = "手机号码不能为空")]
        [Display(Name = "手机号码")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "姓名不能为空")]
        [Display(Name = "姓名")]
        public string MemberName { get; set; }

        [Display(Name = "身份证号码")]
        public string IdentityNo { get; set; }

        [Required]
        [Display(Name = "级别编号")]
        public int RoleId { get; set; }

        [Display(Name = "级别")]
        public IEnumerable<MemberRoleModel> RoleList { get; set; }

        [Display(Name = "上级手机号码")]
        public string ParentMobile { get; set; }

        [Display(Name = "上级姓名")]
        public string ParentMemberName { get; set; }

        [Display(Name = "地址")]
        public string Address { get; set; }

        [Display(Name = "当前角色库存")]
        public int CurrentRoleQuantity { get; set; }

        [Display(Name = "库存")]
        public int TotalQuantity { get; set; }

        [Display(Name = "有效省代")]
        public int ProvinceAvailable { get; set; }

        [Display(Name = "有效总代")]
        public int GeneralAvailable { get; set; }

        [Display(Name = "是否有效省代")]
        public IEnumerable<ValidRoleModel> ProvinceAvailableList { get; set; }

        [Display(Name = "是否有效总代")]
        public IEnumerable<ValidRoleModel> GeneralAvailableList { get; set; }
    }

    public class AddChildMemberModel : BaseModel<Guid>
    {
        [Required]
        [Display(Name = "手机号码")]
        public string Mobile { get; set; }

        [Required]
        [Display(Name = "姓名")]
        public string MemberName { get; set; }

        [Display(Name = "身份证号码")]
        public string IdentityNo { get; set; }

        [Required]
        [Display(Name = "地址")]
        public string Address { get; set; }
    }

    public class MemberRoleModel
    {
        [Required]
        [Display(Name = "编号")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "级别名称")]
        public string Name { get; set; }
    }

    public class ValidRoleModel
    {
        [Required]
        [Display(Name = "编号")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "是否有效")]
        public string Name { get; set; }
    }

    public class SubMemberModel : BaseModel<Guid>
    {

        public string Mobile { get; set; }

        public string MemberName { get; set; }

        public Guid ParentMemberID { get; set; }

        public int RoleID { get; set; }

        public string RoleName
        {
            get
            {
                return ModelEnumHelper<enmMemberRole>.GetDisplayValue((enmMemberRole)RoleID);
            }
        }


        public string Address { get; set; }

        public int LevelID { get; set; }

        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BackgroundColor { get; set; }
    }

    public class Member503020
    {
        public Guid ID { get; set; }

        public string Mobile { get; set; }

        public string MemberName { get; set; }

        public int RoleId { get; set; }

        public string RoleName
        {
            get
            {
                return ModelEnumHelper<enmMemberRole>.GetDisplayValue((enmMemberRole)RoleId);
            }
        }

        public decimal Count50 { get; set; }

        public decimal Count30 { get; set; }

        public decimal Count20 { get; set; }

        public decimal Total { get; set; }
    }

    public class DetailsModel
    {
        public string Mobile { get; set; }

        public string MemberName { get; set; }

        public int RoleId { get; set; }

        public string RoleName
        {
            get
            {
                return ModelEnumHelper<enmMemberRole>.GetDisplayValue((enmMemberRole)RoleId);
            }
        }

        public decimal Quantity { get; set; }

        public DateTime CreateOn { get; set; }
    }

    public class DirectorBonusModel
    {
        public Guid ID { get; set; }

        public string Mobile { get; set; }

        public string MemberName { get; set; }

        public int RoleId { get; set; }

        public string RoleName
        {
            get
            {
                string result = string.Empty;
                if (DirectorCount > 0)
                {
                    if (DirectorCount >= 2)
                    {
                        if (DirectorCount == 2)
                        {
                            result += "双";
                        }
                        else
                        {
                            result += DirectorCount;
                        }
                    }
                }
                return result + ModelEnumHelper<enmMemberRole>.GetDisplayValue((enmMemberRole)RoleId);
            }
        }

        public string SelfBonusStr { get; set; }

        public decimal SelfBonus { get; set; }

        public string CompanyBonusStr { get; set; }

        public decimal CompanyBonus { get; set; }

        public decimal Total { get; set; }

        public int DirectorCount { get; set; }

        public int BonusCount { get; set; }
    }

    public enum enmDetailsType
    {
        type20 = 20,
        type30 = 30,
        type50 = 50,
        typeSelf = 51,
        typeCompany = 52
    }


}
