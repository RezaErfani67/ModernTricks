using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModernTricks.Features.Forum
{
    public class VM_Forum_ShowComments
    {
        public Guid ID { get; set; }
        public Guid ForumGroupID { get; set; }
        public Guid ForumPostID { get; set; }
        public string Username { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ParentID { get; set; }
        public string Text { get; set; }
    


    }
}