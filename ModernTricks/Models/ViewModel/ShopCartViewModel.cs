using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModernTricks.Models
{
    public class ShopCartItem
    {
        public Guid ProductID { get; set; }

        public string Title { get; set; }

        public int Count { get; set; }
    }

    public class ShowOrderItemViewModel
    {
        public Guid ProductID { get; set; }

        public string Title { get; set; }

        public int Count { get; set; }

        public int Price { get; set; }

        public int Sum { get; set; }
    }
}