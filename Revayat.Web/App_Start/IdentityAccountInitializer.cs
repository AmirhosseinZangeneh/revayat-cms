using System;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Revayat.Web.Models;

namespace Revayat.Web
{
    public static class IdentityAccountInitializer
    {
        public static void Initialize()
        {
            using (var db = new IdentityContext())
            using (var roles = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db)))
            using (var users = ApplicationUserManager.CreateForContext(db))
            {
                db.Database.Initialize(false);
                EnsureRole(roles, "User");
                EnsureRole(roles, "Admin");

                var adminRole = roles.FindByName("Admin");
                if (db.Users.Any(u => u.Roles.Any(r => r.RoleId == adminRole.Id)))
                    return;

                var name = Environment.GetEnvironmentVariable("REVAYAT_ADMIN_USER");
                var email = Environment.GetEnvironmentVariable("REVAYAT_ADMIN_EMAIL");
                var password = Environment.GetEnvironmentVariable("REVAYAT_ADMIN_PASSWORD");
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(password))
                    return;

                if (users.FindByName(name) != null || users.FindByEmail(email) != null)
                    throw new InvalidOperationException("The bootstrap admin name or email already belongs to a user.");

                var result = users.Create(new ApplicationUser { UserName = name, Email = email }, password);
                if (!result.Succeeded)
                    throw new InvalidOperationException("Admin creation failed: " + string.Join("; ", result.Errors));

                var admin = users.FindByName(name);
                result = users.AddToRole(admin.Id, "Admin");
                if (!result.Succeeded)
                {
                    users.Delete(admin);
                    throw new InvalidOperationException("Admin role assignment failed: " + string.Join("; ", result.Errors));
                }
            }
        }

        private static void EnsureRole(RoleManager<IdentityRole> roles, string role)
        {
            if (roles.RoleExists(role)) return;
            var result = roles.Create(new IdentityRole(role));
            if (!result.Succeeded)
                throw new InvalidOperationException("Role creation failed: " + string.Join("; ", result.Errors));
        }
    }
}