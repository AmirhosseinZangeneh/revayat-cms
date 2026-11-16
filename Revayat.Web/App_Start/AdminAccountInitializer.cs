using System;
using System.Linq;
using DataLayer;

namespace Revayat.Web
{
    public static class AdminAccountInitializer
    {
        public static void Initialize()
        {
            using (var db = new RevayatContext())
            {
                var users = db.AdminLogins.ToList();
                foreach (var user in users.Where(u => !PasswordHash.IsHash(u.Password)))
                    user.Password = PasswordHash.Create(user.Password);

                if (users.Count == 0)
                {
                    var userName = Environment.GetEnvironmentVariable("REVAYAT_ADMIN_USER");
                    var email = Environment.GetEnvironmentVariable("REVAYAT_ADMIN_EMAIL");
                    var password = Environment.GetEnvironmentVariable("REVAYAT_ADMIN_PASSWORD");
                    if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(email) &&
                        !string.IsNullOrWhiteSpace(password))
                    {
                        db.AdminLogins.Add(new AdminLogin
                        {
                            UserName = userName,
                            Email = email,
                            Password = PasswordHash.Create(password)
                        });
                    }
                }

                db.SaveChanges();
            }
        }
    }
}
