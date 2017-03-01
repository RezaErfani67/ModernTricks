using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModernTricks.Features.Shop.Shop
{
    public class VM_Shop_RequestPayment
    {
        public string MerchantCode { get; set; }
        public int amount { get; set; }
        public string description { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string verifyUrl { get; set; }
    }
}