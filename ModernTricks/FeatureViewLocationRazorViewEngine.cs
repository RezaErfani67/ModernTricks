using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Main
{
    public class FeatureViewLocationRazorViewEngine : RazorViewEngine
    {
        public FeatureViewLocationRazorViewEngine()
        {
            var featureFolderViewLocationFormats = new[]
              {
        "~/Features/Home/{1}/{0}.cshtml",

        "~/Features/Shop/{1}/{0}.cshtml",

        "~/Features/{1}/{0}.cshtml",
      
        "~/Features/Shared/{0}.cshtml",
        
      };

            ViewLocationFormats = featureFolderViewLocationFormats;
            MasterLocationFormats = featureFolderViewLocationFormats;
            PartialViewLocationFormats = featureFolderViewLocationFormats;
        }
    }
}