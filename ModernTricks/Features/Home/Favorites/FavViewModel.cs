
using ModernTricks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModernTricks.Features.Home
{
    public class FavViewModel
    {
        public IEnumerable<Videos> Videos { get; set; }
        public IEnumerable<News> News { get; set; }
    }
}