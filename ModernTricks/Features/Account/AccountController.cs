using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.Security;
using SendMail;
using ModernTricks.Models;
using ModernTricks.Models.ViewModel;
using ModernTricks.Common;
using System.IO;

namespace Main.Controllers
{

    public class AccountController : Controller
    {
        private MainDBEntities database = new MainDBEntities();



        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult Register(RegisterViewModel register,HttpPostedFileBase pic)
        {


            if (ModelState.IsValid)
            {
                if (true)//this.IsCaptchaValid("Captcha is not valid"))
                {
                    if (!database.USERS.Any(u => u.Username == register.Email.Trim().ToLower()))
                    {
                        USERS u = new USERS()
                        {
                            User_Id=Guid.NewGuid(),
                            ActiveCode = Guid.NewGuid().ToString().Replace("-", ""),
                            EMail = register.Email.Trim().ToLower(),
                            Inactive = false,
                            Password = FormsAuthentication.HashPasswordForStoringInConfigFile(register.Pass, "MD5"),
                            Username = register.Email.Trim().ToLower(),
                            Firstname = register.Firstname,
                            Lastname = register.Lastname,
                            //Standard User
                            ROLES = database.ROLES.Where(s => s.Role_Id == 2).ToList()
                        };
                        database.USERS.Add(u);

                        if (pic != null)
                        {
                            string ex = Path.GetExtension(pic.FileName).ToLower();
                            if (ex == ".jpg" || ex == ".png")
                            {
                                pic.InputStream.ResizeImage(60, 80,
                                Server.MapPath("/Content/Forum/Users/" + u.Username + ex),
                                Utilty.ImageComperssion.Normal);

                                u.Pic = u.Username + ex;
                            }
                        }

                        database.SaveChanges();

                        string Body = PartialToStringClass.RenderPartialView("SendEmail", "ActiveUser", u);
                        SendEmailGmail.Send(u.EMail, "ایمیل فعال سازی", Body);
                    }
                    else
                    {
                        ModelState.AddModelError("Username", "نام کاربری وارد شده تکراری است");
                    }
                }
                else
                {
                    //   ModelState.AddModelError("CaptchaInputText", "Captcha is not valid");
                }
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return PartialView();
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel Login, string returnUrl)
        {
            if (returnUrl== "undefined") { returnUrl = "/"; }
            string Pass = FormsAuthentication.HashPasswordForStoringInConfigFile(Login.Password, "MD5");
            var user = database.USERS.FirstOrDefault(u => u.Username == Login.Username && u.Password == Pass);
            if (user != null)
            {
                if (user.Inactive == true)
                {
                    FormsAuthentication.SetAuthCookie(user.User_Id.ToString(), Login.RememberMe);
                    return Redirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("Username", "حساب کاربری شما فعال نشده است");
                }
            }
            else
            {
                ModelState.AddModelError("Username", "کاربری یافت نشد");
            }
            return View();
        }
        public ActionResult ActiveUser(string id)
        {
            var user = database.USERS.FirstOrDefault(u => u.ActiveCode == id);
            if (user != null)
            {
                user.Inactive = true;
                user.ActiveCode = Guid.NewGuid().ToString();
                database.SaveChanges();
                ViewBag.IsOk = true;

            }
            return View(user);
        }

        public ActionResult ChangePassFromEmail(string id)
        {
            return View(new ChangePassFromEmail()
            {
                UserCode = id
            });
        }

        [HttpPost]
        public ActionResult ChangePassFromEmail(ChangePassFromEmail changePass)
        {
            var user = database.USERS.FirstOrDefault(u => u.ActiveCode == changePass.UserCode);

            if (user != null)
            {
                user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(changePass.Pass, "MD5");
                user.ActiveCode = Guid.NewGuid().ToString();

                database.SaveChanges();

                return Redirect("/Account/Login");
            }
            else
            {
                ModelState.AddModelError("Pass", "اطلاعات صحیح نمی باشد");

            }
            return View(changePass);
        }


        public ActionResult RecoveryPassword()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult RecoveryPassword(RecoveryPassViewModel recoveryPass)
        {
            var user = database.USERS.FirstOrDefault(u => u.EMail == recoveryPass.Email.Trim().ToLower());

            if (user != null)
            {

                string Body = PartialToStringClass.RenderPartialView("SendEmail", "SendPass", user);

                SendEmailGmail.Send(user.EMail, "بازیابی کلمه عبور", Body);

                ViewBag.SendEmail = true;
            }
            else
            {
                ModelState.AddModelError("Email", "کاربری با ایمیل وارد شده یافت نشد");
            }

            return View(recoveryPass);
        }
        public ActionResult StateLogin()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.FLName = database.USERS.Select(q => new { q.User_Id, q.FLName }).First(q => q.User_Id.ToString() == HttpContext.User.Identity.Name).FLName;
            }
            return PartialView("_StateLogin");
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return Redirect("/");
        }




        public ActionResult _ChangePass()
        {
            return PartialView();
        }


        [HttpPost]
        public ActionResult _ChangePass(ChangePass change)
        {
            var user = database.USERS.FirstOrDefault(u => u.Username == User.Identity.Name);

            string oldpass = FormsAuthentication.HashPasswordForStoringInConfigFile(change.OldPass, "MD5");
            if (user.Password == oldpass)
            {
                string pass = FormsAuthentication.HashPasswordForStoringInConfigFile(change.Pass, "MD5");
                user.Password = pass;
                database.SaveChanges();
                ViewBag.IsOk = true;
            }
            else
            {
                ModelState.AddModelError("OldPass", "کلمه عبور فعلی صحیح نمی باشد");
            }
            return View(change);
        }

    }
}