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
    
    [MetadataType(typeof(News_CommentsMetadata))]
    public partial class News_Comments
    {
        public int ID { get; set; }
        public int NewsID { get; set; }
        public Nullable<int> ParentID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<bool> IsApprove { get; set; }
    
        public virtual News News { get; set; }
    }
}
