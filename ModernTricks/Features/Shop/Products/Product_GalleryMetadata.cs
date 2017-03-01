using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModernTricks.Models
{
    class Product_GalleryMetadata
    {
        public Guid GalleryID { get; set; }
        public Guid ProductID { get; set; }
        public string ImageName { get; set; }
    }
}
