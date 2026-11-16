using System;
using System.Globalization;
using System.Security.Cryptography;

namespace DataLayer
{
    public static class PasswordHash
    {
        private const int Iterations = 210000;
        private const int SaltSize = 32;
        private const int HashSize = 32;
        private const string Prefix = "pbkdf2-sha256";

        public static string Create(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("A password is required.", "password");

            var salt = new byte[SaltSize];
            using (var random = RandomNumberGenerator.Create())
                random.GetBytes(salt);

            byte[] hash;
            using (var derivation = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
                hash = derivation.GetBytes(HashSize);

            return Prefix + ":" + Iterations.ToString(CultureInfo.InvariantCulture) + ":" +
                   Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        public static bool IsHash(string value)
        {
            return value != null && value.StartsWith(Prefix + ":", StringComparison.Ordinal);
        }

        public static bool Verify(string password, string stored)
        {
            if (password == null || !IsHash(stored))
                return false;

            var parts = stored.Split(':');
            int iterations;
            if (parts.Length != 4 || !int.TryParse(parts[1], NumberStyles.None,
                    CultureInfo.InvariantCulture, out iterations) || iterations < 10000 || iterations > 1000000)
                return false;

            try
            {
                var salt = Convert.FromBase64String(parts[2]);
                var expected = Convert.FromBase64String(parts[3]);
                if (salt.Length != SaltSize || expected.Length != HashSize)
                    return false;

                byte[] actual;
                using (var derivation = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
                    actual = derivation.GetBytes(HashSize);

                var difference = 0;
                for (var i = 0; i < expected.Length; i++)
                    difference |= expected[i] ^ actual[i];
                return difference == 0;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
