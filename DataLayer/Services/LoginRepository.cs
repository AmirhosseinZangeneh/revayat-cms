using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
   public class LoginRepository:ILoginRepository
   {
       private RevayatContext db;

       public LoginRepository(RevayatContext context)
       {
           db = context;
       }
       public bool IsExistUser(string username, string password)
       {
           var user = db.AdminLogins.SingleOrDefault(u => u.UserName == username);
           return user != null && PasswordHash.Verify(password, user.Password);
       }
    }
}
