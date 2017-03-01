using ModernTricks.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ModernTricks.Features.Automation
{
    public class VM_Task
    {
        [Key]
        public Guid ID { get; set; }
        public string Title { get; set; }

        public string Html { get; set; }
        public string Script { get; set; }
        public string ApproveHtml { get; set; }
        public string ApproveScript { get; set; }
        public string RejectHtml { get; set; }
        public string RejectScript { get; set; }
       
        public string Filter1 { get; set; }
        public string Filter2 { get; set; }
        public string Filter3 { get; set; }

        public string Description { get; set; }

        public Guid? genericId { get; set; }
        public  string GenericTitle { get; set; }

        #region BaniChav_ReturnedProduct
        public string BaniChav_ReturnedProducts_Title{ get; set; }
        public string BaniChav_ReturnedProducts_Description { get; set; }

        #endregion

    }
}