using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ModernTricks.Models.ViewModel
{
    public class WareHouseItem
    {
        public Guid ProductID { get; set; }

        [Display(Name = "کالا")]
        public string ProductTitle { get; set; }

           [Display(Name = "تعداد موجودی")]
        public int Count { get; set; }

    }
}