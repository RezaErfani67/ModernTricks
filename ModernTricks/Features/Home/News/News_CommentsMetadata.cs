using System;
using System.ComponentModel.DataAnnotations;

namespace ModernTricks.Models
{
     class News_CommentsMetadata
    {
        [Key]
        public int ID { get; set; }
        public int NewsID { get; set; }
        public Nullable<int> ParentID { get; set; }
        [Display(Name ="نام و نام خانوادگی")]
        [Required(ErrorMessage ="نام را وارد نمایید.")]
        public string Name { get; set; }
        [Display(Name = "آدرس ایمیل")]

        [Required(ErrorMessage = "ایمیل را وارد نمایید.")]
        public string Email { get; set; }
        [Display(Name = "نظرتان را بیان کنید")]
        public string Text { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<bool> IsApprove { get; set; }

    }
}