using DataLayer;
using Microsoft.AspNet.Identity;

namespace Revayat.Web
{
    public class IdentityPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return PasswordHash.Create(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            return PasswordHash.Verify(providedPassword, hashedPassword)
                ? PasswordVerificationResult.Success
                : PasswordVerificationResult.Failed;
        }
    }
}