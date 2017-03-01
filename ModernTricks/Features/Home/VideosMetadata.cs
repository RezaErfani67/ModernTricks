using System.ComponentModel.DataAnnotations;

namespace ModernTricks.Models
{
     class VideosMetadata
    {
        [Key]
        public int ID { get; set; }



        [Required(ErrorMessage = "لطفا فایل ویدیو را وارد نمایید")]
        public string Name{ get; set; }

        [Required(ErrorMessage ="لطفا عنوان ویدیو را وارد نمایید")]
        public string Title { get; set; }
    }
}