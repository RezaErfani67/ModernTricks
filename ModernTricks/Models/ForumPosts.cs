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
    
    [MetadataType(typeof(ForumPostsMetadata))]
    public partial class ForumPosts
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ForumPosts()
        {
            this.ForumComments = new HashSet<ForumComments>();
        }
    
        public string Title { get; set; }
        public string Text { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public System.Guid ForumGroupID { get; set; }
        public System.Guid ID { get; set; }
        public System.Guid CreatedBy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForumComments> ForumComments { get; set; }
        public virtual ForumGroups ForumGroups { get; set; }
        public virtual USERS USERS { get; set; }
    }
}