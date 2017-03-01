using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModernTricks.Models
{
    class Product_FeaturesMetadata
    {
        public int PF_ID { get; set; }
        public Guid ProductID { get; set; }
        public Guid FeatureID { get; set; }
        public string Value { get; set; }
    }
}
