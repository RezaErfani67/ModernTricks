using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Main.Models.ViewModel
{
    public class RegisterViewModel
    {
        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "لطفا نام کاربری را وارد کنید")]
        public string UserName { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [DataType(DataType.Password)]
        public string Pass { get; set; }

        [Display(Name = "تکرار کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Compare("Pass", ErrorMessage = "کلمه های عبور مغایرت دارند")]
        [DataType(DataType.Password)]
        [MaxLength(80, ErrorMessage = "{0} نمیتواند بیشتر از {1} باشد")]
        public string RePass { get; set; }

        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نمی باشد")]
        public string Email { get; set; }

    }
    public class LoginViewModel
    {
        [Display(Name ="نام کاربری")]
        public string Username { get; set; }
        [Display(Name ="رمز عبور")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name ="مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }
    public class ChangePassViewModel
    {
        [Display(Name = "کلمه عبور فعلی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [DataType(DataType.Password)]
        public string OldPass { get; set; }

        [Display(Name = " کلمه عبور جدید")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [DataType(DataType.Password)]
        public string Pass { get; set; }

        [Display(Name = "تکرار کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [DataType(DataType.Password)]
        [Compare("Pass", ErrorMessage = "کلمه های عبور مغایرت دارند")]
        public string RePass { get; set; }
    }


    public class ChangePassFromEmail
    {
        [Display(Name = " کلمه عبور جدید")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [DataType(DataType.Password)]
        public string Pass { get; set; }

        [Display(Name = "تکرار کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [DataType(DataType.Password)]
        [Compare("Pass", ErrorMessage = "کلمه های عبور مغایرت دارند")]
        public string RePass { get; set; }


        public string UserCode { get; set; }

    }

    public class RecoveryPassViewModel
    {
        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نمی باشد")]

        public string Email { get; set; }
    }
    public class ChangePass
    {

        [Display(Name = " کلمه عبور فعلی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [DataType(DataType.Password)]
        public string OldPass { get; set; }


        [Display(Name = " کلمه عبور جدید")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [DataType(DataType.Password)]
        public string Pass { get; set; }

        [Display(Name = "تکرار کلمه عبور جدید")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Compare("Pass", ErrorMessage = "کلمه های عبور مغایرت دارند")]
        [DataType(DataType.Password)]
        [MaxLength(80, ErrorMessage = "{0} نمیتواند بیشتر از {1} باشد")]
        public string RePass { get; set; }
    }
}