using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ModernTricks.Models
{
     class ForumPostsMetadata
    {
     
        [Display(Name ="عنوان")]
        [Required(ErrorMessage ="لطفا عنوان را وارد نمایید.")]
        [MinLength(5,ErrorMessage ="حداقل باید 5 کاراکتر وارد نمایید")]
        public string Title { get; set; }


        [Display(Name = "توضیحات")]
        [MinLength(10, ErrorMessage = "حداقل باید 10 کاراکتر وارد نمایید")]
        [Required(ErrorMessage = "لطفا متن را وارد نمایید.")]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string Text { get; set; }



        [Required(ErrorMessage = "لطفا شماره گروه را وارد نمایید.")]
        public Guid ForumGroupID { get; set; }


        [Display(Name = "نام کاربری")]
        public string CreatedBy { get; set; }

        [Display(Name = "تاریخ ایجاد")]
        public string CreatedDate { get; set; }
    }
}