
using System;
using System.ComponentModel.DataAnnotations;
namespace XDropsWater.Model
{
    public class AddPersonalOrderModel : BaseModel<Guid>
    {
        [Required]
        [Display(Name = "订货数量")]
        public int Quantity { get; set; }
    }
}
