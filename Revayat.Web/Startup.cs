using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Revayat.Web.Models;

[assembly: OwinStartup(typeof(Revayat.Web.Startup))]

namespace Revayat.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext(IdentityContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                CookieName = "revayat.identity",
                CookieHttpOnly = true,
#if DEBUG
                CookieSecure = CookieSecureOption.SameAsRequest,
#else
                CookieSecure = CookieSecureOption.Always,
#endif
                ExpireTimeSpan = TimeSpan.FromMinutes(60),
                SlidingExpiration = true,
                Provider = new CookieAuthenticationProvider
                {
                    OnApplyRedirect = context =>
                    {
                        if (string.Equals(context.Request.Headers.Get("X-Requested-With"), "XMLHttpRequest", StringComparison.OrdinalIgnoreCase))
                            context.Response.StatusCode = 401;
                        else if (context.Request.User != null && context.Request.User.Identity.IsAuthenticated)
                            context.Response.Redirect("/Account/AccessDenied");
                        else
                            context.Response.Redirect(context.RedirectUri);
                    },
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => manager.CreateIdentityAsync(
                            user, DefaultAuthenticationTypes.ApplicationCookie))
                }
            });
            IdentityAccountInitializer.Initialize();
        }
    }
}