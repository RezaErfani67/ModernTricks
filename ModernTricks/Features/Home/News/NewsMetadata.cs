using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ModernTricks.Models
{
     class NewsMetadata
    {

        [Key]
        public int ID { get; set; }

        [Display(Name ="عنوان")]

        public string Title { get; set; }

        [Display(Name ="مختصر اخبار")]
        public string ShortDescription { get; set; }

        [Display(Name ="توضیحات اخبار")]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string Text { get; set; }

        [Display(Name = "عکس خبر")]
        public string Image { get; set; }
        public System.DateTime CreateDate { get; set; }
        public short See { get; set; }
    }
}