using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModernTricks.Common
{
    public static class CreateUrl
    {
        public static string MyUrl(this string value)
        {
            return value.Replace(".", "").Replace("/", "_");
        }
    }
}