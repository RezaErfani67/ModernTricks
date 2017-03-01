using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ModernTricks.Models;

namespace ModernTricks.Common
{
    public class WareHouseChecker
    {
        public static int Count(Guid productID)
        {
            using (MainDBEntities db =new MainDBEntities())
            {
                var Enter =
                    db.WareHouse.Where(w => w.TypeID == 1 && w.ProductID == productID).Select(s => s.Count).ToList();

                var Exit =
                    db.WareHouse.Where(w => w.TypeID == 2 && w.ProductID == productID).Select(s => s.Count).ToList();

                return (Enter.Sum() - Exit.Sum());
            }
        }
    }
}