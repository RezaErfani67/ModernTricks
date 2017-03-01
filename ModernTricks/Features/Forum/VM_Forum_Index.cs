using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModernTricks.Features.Forum
{
    public class VM_Forum_Index
    {
        public Guid groupID { get; set; }

        public Guid? groupParentID { get; set; }
        public string groupTitle { get; set; }
        public string groupManager { get; set; }
    }
}