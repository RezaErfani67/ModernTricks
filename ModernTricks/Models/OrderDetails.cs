//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ModernTricks.Models
{
    using System.ComponentModel.DataAnnotations;
    using System;
    using System.Collections.Generic;
    
    [MetadataType(typeof(OrderDetailsMetadata))]
    public partial class OrderDetails
    {
        public System.Guid DetailID { get; set; }
        public int ProductCount { get; set; }
        public int ProductPrice { get; set; }
        public System.Guid ProductID { get; set; }
        public System.Guid OrderID { get; set; }
    
        public virtual Products Products { get; set; }
        public virtual Orders Orders { get; set; }
    }
}
