using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ModernTricks.Models
{
     class ForumCommentsMetadata
    {
       
        [Display(Name = "متن نظر")]
        [Required(ErrorMessage = "لطفا متن را وارد نمایید.")]
        [AllowHtml]
        public string Text { get; set; }
     
     
    }
}