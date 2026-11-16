using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Revayat.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
    }

    public class IdentityContext : IdentityDbContext<ApplicationUser>
    {
        static IdentityContext()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<IdentityContext>());
        }

        public IdentityContext() : base("RevayatIdentityContext")
        {
        }

        public static IdentityContext Create()
        {
            return new IdentityContext();
        }
    }
}