using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InsertShowImage;
using ModernTricks.Models;
using ModernTricks.Models.ViewModel;

namespace ModernTricks.Controllers
{
    public class ProductsController : Controller
    {

        public ProductsController()
        {
            //Session.Timeout = 40;
        }

        private MainDBEntities db = new MainDBEntities();

        // GET: Admin/Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Product_Groups);
            return View(products.ToList());
        }

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.GroupID = new SelectList(db.Product_Groups, "GroupID", "GroupTitle");
            ViewBag.Features = new SelectList(db.Features, "FeatureID", "FeatureTitle");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Products products,HttpPostedFileBase Image,HttpPostedFileBase[] Gallery,string Tags)
        {
            if (ModelState.IsValid)
            {
               
                products.Productimage = "no-photo.jpg";
                db.Products.Add(products);
                db.SaveChanges();

                if (Image != null)
                {
                    Image.SaveAs(Server.MapPath("/Content/Product/Images/large/" + products.ProductID + "_" + Image.FileName));

                    ImageResizer img=new ImageResizer();
                    img.Resize(Server.MapPath("/Content/Product/Images/large/" + products.ProductID+"_" + Image.FileName),
                        Server.MapPath("/Content/Product/Images/thumb/" + products.ProductID + "_" + Image.FileName));

                    products.Productimage = products.ProductID + "_" + Image.FileName;
                    db.Entry(products).State=EntityState.Modified;


                }


                if ( Gallery!=null&&Gallery.Any() )
                {
                    foreach (HttpPostedFileBase gal in Gallery)
                    {
                        string imageName = Guid.NewGuid().ToString() + Path.GetExtension(gal.FileName);

                        gal.SaveAs(Server.MapPath("/Content/Product/Images/large/" + imageName));

                        ImageResizer img = new ImageResizer();
                        img.Resize(Server.MapPath("/Content/Product/Images/large/" + imageName),
                            Server.MapPath("/Content/Product/Images/thumb/" + imageName));


                        db.Product_Gallery.Add(new Product_Gallery()
                        {
                            ImageName = imageName,
                            ProductID = products.ProductID,
                           
                        });
                    }
                }

                if (!string.IsNullOrEmpty(Tags))
                {
                    string[] Tag = Tags.Split('-');

                    foreach (string s in Tag)
                    {
                        db.Product_Tags.Add(new Product_Tags()
                        {
                            ProductID = products.ProductID,
                            TagTitle = s.Trim().ToLower(),
                           
                        });
                    }
                }

                if (Session["Features"] != null)
                {
                    List<FeatureItem> list = Session["Features"] as List<FeatureItem>;

                    foreach (var feature in list)
                    {
                        db.Product_Features.Add(new Product_Features()
                        {
                            FeatureID = feature.FeatureID,
                            Value = feature.Value,
                            ProductID = products.ProductID,
                           
                        });

                    }

                    Session["Features"] = null;
                }


                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.GroupID = new SelectList(db.Product_Groups, "GroupID", "GroupTitle", products.GroupID);
            return View(products);
        }

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }

            string tags = "";

            tags = string.Join("-", products.Product_Tags.Select(s => s.TagTitle));
            ViewBag.Tags = tags;

            if (products.Product_Features.Any())
            {
                Session["Features"] = products.Product_Features.Select(s => new FeatureItem()
                {
                    FeatureID = s.FeatureID,
                    FeatureTitle = s.Features.FeatureTitle,
                    Value = s.Value
                }).ToList();
            }
            ViewBag.Features = new SelectList(db.Features, "FeatureID", "FeatureTitle");
            ViewBag.GroupID = new SelectList(db.Product_Groups, "GroupID", "GroupTitle", products.GroupID);
            return View(products);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Products products, HttpPostedFileBase Image, HttpPostedFileBase[] Gallery, string Tags)
        {
            if (ModelState.IsValid)
            {

                if (Image != null)
                {
                    if (products.Productimage != "no-photo.jpg")
                    {
                        System.IO.File.Delete(Server.MapPath("/Content/Product/Images/large/" + products.Productimage));
                        System.IO.File.Delete(Server.MapPath("/Content/Product/Images/thumb/" + products.Productimage));
                    }


                    Image.SaveAs(Server.MapPath("/Content/Product/Images/large/" + products.ProductID + "_" + Image.FileName));

                    ImageResizer img = new ImageResizer();
                    img.Resize(Server.MapPath("/Content/Product/Images/large/" + products.ProductID + "_" + Image.FileName),
                        Server.MapPath("/Content/Product/Images/thumb/" + products.ProductID + "_" + Image.FileName));

                    products.Productimage = products.ProductID + "_" + Image.FileName;
             


                }

                db.Entry(products).State = EntityState.Modified;


               
                    foreach (HttpPostedFileBase gal in Gallery)
                    {
                    if (gal != null)
                    {
                        string imageName = Guid.NewGuid().ToString() + Path.GetExtension(gal.FileName);

                        gal.SaveAs(Server.MapPath("/Content/Product/Images/large/" + imageName));

                        ImageResizer img = new ImageResizer();
                        img.Resize(Server.MapPath("/Content/Product/Images/large/" + imageName),
                            Server.MapPath("/Content/Product/Images/thumb/" + imageName));


                        db.Product_Gallery.Add(new Product_Gallery()
                        {
                            GalleryID = Guid.NewGuid(),
                            ImageName = imageName,
                            ProductID = products.ProductID,

                        });
                    }
                }

                db.Product_Tags.Where(t => t.ProductID == products.ProductID).ToList().ForEach(t=>db.Product_Tags.Remove(t));

                if (!string.IsNullOrEmpty(Tags))
                {
                    string[] Tag = Tags.Split('-');

                    foreach (string s in Tag)
                    {
                        db.Product_Tags.Add(new Product_Tags()
                        {

                            ProductID = products.ProductID,
                            TagTitle = s.Trim().ToLower(),

                        });
                    }
                }
                db.Product_Features.Where(t => t.ProductID == products.ProductID).ToList().ForEach(t => db.Product_Features.Remove(t));

                if (Session["Features"] != null)
                {
                    List<FeatureItem> list = Session["Features"] as List<FeatureItem>;

                    foreach (var feature in list)
                    {
                        db.Product_Features.Add(new Product_Features()
                        {
                            FeatureID = feature.FeatureID,
                            Value = feature.Value,
                            ProductID = products.ProductID,

                        });

                    }

                    Session["Features"] = null;
                }

             
                db.SaveChanges();


                return RedirectToAction("Index");
            }
            ViewBag.GroupID = new SelectList(db.Product_Groups, "GroupID", "GroupTitle", products.GroupID);
            return View(products);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Products products = db.Products.Find(id);

            db.Product_Tags.Where(t => t.ProductID == products.ProductID).ToList().ForEach(t => db.Product_Tags.Remove(t));

            db.Product_Features.Where(t => t.ProductID == products.ProductID).ToList().ForEach(t => db.Product_Features.Remove(t));

            foreach (var gallery in db.Product_Gallery.Where(g=>g.ProductID==products.ProductID))
            {
                db.Product_Gallery.Remove(gallery);

                System.IO.File.Delete(Server.MapPath("/Content/Product/Images/large/" + gallery.ImageName));
                System.IO.File.Delete(Server.MapPath("/Content/Product/Images/thumb/" + gallery.ImageName));
            }

            db.Products.Remove(products);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        public bool DeleteImage(int id)
        {
            var gallery = db.Product_Gallery.Find(id);

            System.IO.File.Delete(Server.MapPath("/Content/Product/Images/large/" + gallery.ImageName));
            System.IO.File.Delete(Server.MapPath("/Content/Product/Images/thumb/" + gallery.ImageName));

            db.Product_Gallery.Remove(gallery);
            db.SaveChanges();
            return true;
        }


        public ActionResult AddFeature(Guid FeatureID,string FeatureTitle, string Value)
        {
            List<FeatureItem> list = new List<FeatureItem>();
            if (!string.IsNullOrEmpty(FeatureTitle) && !string.IsNullOrEmpty(Value))
            {
             

                if (Session["Features"] != null)
                {
                    list = Session["Features"] as List<FeatureItem>;
                }

                list.Add(new FeatureItem()
                {
                    FeatureID = FeatureID,
                    FeatureTitle = FeatureTitle,
                    Value = Value
                });

                Session["Features"] = list;
            }
            return PartialView("listFeature", list);

        }

        public ActionResult listFeature()
        {
            List<FeatureItem> list = new List<FeatureItem>();

            if (Session["Features"] != null)
            {
                list = Session["Features"] as List<FeatureItem>;
            }

            return PartialView(list);
        }


        public ActionResult DeleteFeature(Guid FeatureID, string Value)
        {
            List<FeatureItem> list = new List<FeatureItem>();
     


                if (Session["Features"] != null)
                {
                    list = Session["Features"] as List<FeatureItem>;

                    var feature = list.FirstOrDefault(f => f.FeatureID == FeatureID && f.Value == Value);
                    if (feature != null)
                    {
                        list.Remove(feature);
                    }
                }

            

                Session["Features"] = list;
            
            return PartialView("listFeature", list);

        }
        
    }
}
