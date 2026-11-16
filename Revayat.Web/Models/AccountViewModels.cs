using System.ComponentModel.DataAnnotations;

namespace Revayat.Web.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "نام کاربری را وارد کنید.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "نام کاربری باید دست‌کم ۳ کاراکتر باشد.")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "ایمیل را وارد کنید.")]
        [EmailAddress(ErrorMessage = "ایمیل معتبر نیست.")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Required(ErrorMessage = "رمز عبور را وارد کنید.")]
        [StringLength(100, MinimumLength = 12, ErrorMessage = "رمز عبور باید دست‌کم ۱۲ کاراکتر باشد.")]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "تکرار رمز عبور یکسان نیست.")]
        [DataType(DataType.Password)]
        [Display(Name = "تکرار رمز عبور")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "نام کاربری را وارد کنید.")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "رمز عبور را وارد کنید.")]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "رمز فعلی")]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 12)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز جدید")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "تکرار رمز جدید یکسان نیست.")]
        [DataType(DataType.Password)]
        [Display(Name = "تکرار رمز جدید")]
        public string ConfirmPassword { get; set; }
    }
}