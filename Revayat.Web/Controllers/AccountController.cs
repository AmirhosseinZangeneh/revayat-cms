using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Revayat.Web.Models;

namespace Revayat.Web.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Profile");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Profile");
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                UserName = model.UserName.Trim(),
                Email = model.Email.Trim()
            };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return View(model);
            }

            result = await UserManager.AddToRoleAsync(user.Id, "User");
            if (!result.Succeeded)
            {
                await UserManager.DeleteAsync(user);
                AddErrors(result);
                return View(model);
            }

            await SignInAsync(user, false);
            return RedirectToAction("Profile");
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction(User.IsInRole("Admin") ? "Index" : "Profile",
                    User.IsInRole("Admin") ? "Dashboard" : "Account",
                    User.IsInRole("Admin") ? new { area = "Admin" } : new { area = "" });
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Profile");
            if (!ModelState.IsValid) return View(model);

            var user = await UserManager.FindByNameAsync(model.UserName.Trim());
            if (user != null && !await UserManager.IsLockedOutAsync(user.Id))
            {
                if (await UserManager.CheckPasswordAsync(user, model.Password))
                {
                    await UserManager.ResetAccessFailedCountAsync(user.Id);
                    await SignInAsync(user, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        if (returnUrl.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase) &&
                            !await UserManager.IsInRoleAsync(user.Id, "Admin"))
                            return RedirectToAction("AccessDenied");
                        return Redirect(returnUrl);
                    }
                    if (await UserManager.IsInRoleAsync(user.Id, "Admin"))
                        return Redirect("/Admin");
                    return RedirectToAction("Profile");
                }
                await UserManager.AccessFailedAsync(user.Id);
            }
            ModelState.AddModelError("", "نام کاربری یا رمز عبور نادرست است، یا حساب موقتاً قفل شده است.");
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [Authorize]
        [ActionName("Profile")]
        public async Task<ActionResult> MyAccount()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null) return HttpNotFound();
            return View("Profile", user);
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await UserManager.ChangePasswordAsync(
                User.Identity.GetUserId(), model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return View(model);
            }

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            await SignInAsync(user, false);
            TempData["Message"] = "رمز عبور تغییر کرد.";
            return RedirectToAction("Profile");
        }

        [Authorize]
        public ActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult SignOut()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return Redirect("/");
        }

        private async Task SignInAsync(ApplicationUser user, bool persistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            var identity = await UserManager.CreateIdentityAsync(
                user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = persistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);
        }
    }
}