using System.ComponentModel.DataAnnotations;

namespace ModernTricks.Models
{
    internal class GalleryMetadata
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Alt { get; set; }
    }
}