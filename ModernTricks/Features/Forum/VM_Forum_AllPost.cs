using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModernTricks.Features.Forum
{
    public class VM_Forum_AllPost
    {
        public Guid postID { get; set; }
        public string Title{ get; set; }
        public string CreatedBy_FLName{ get; set; }

        public string Pic{ get; set; }

        public DateTime? CreatedDate { get; set; }



    }
}