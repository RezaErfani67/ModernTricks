using System;
using System.ComponentModel.DataAnnotations;

namespace ModernTricks.Models
{
     class ForumGroupsMetadata
    {
        
        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "لطفا عنوان را وارد نمایید.")]
        public string Title { get; set; }


        [Display(Name = "گروه پدر")]
        public Nullable<Guid> ParentID { get; set; }

        [Display(Name = "نام کاربری مدیر گروه")]
        public Nullable<Guid> AdminUserId { get; set; }
    }
}