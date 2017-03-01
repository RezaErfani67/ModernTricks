using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ModernTricks.Models
{
    class Product_CommentsMetadata
    {
        [Key]
        public Guid CommentID { get; set; }
        public Guid ProductID { get; set; }

        [Display(Name = "نام شما")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Name { get; set; }
        [Display(Name = "ایمیل شما")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نیست")]
        public string Email { get; set; }

         [Display(Name = "متن نظر")]
         [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<Guid> ParentID { get; set; }
    }
}
