using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ModernTricks.Models.ViewModel
{
    public class FeatureItem
    {
        public Guid FeatureID { get; set; }

        [Display(Name = "ویژگی")]
        public string FeatureTitle { get; set; }
        [Display(Name = "مقدار")]
        public string Value { get; set; }

    }
}