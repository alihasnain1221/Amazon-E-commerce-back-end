using System.Security.Cryptography;

namespace E_commerce_FYP_backend.Utils
{
    public class Password
    {

        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 10000;

        public static string EncryptPassword(string password)
        {
            byte[] salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] key = pbkdf2.GetBytes(KeySize);

            var bytes = new byte[SaltSize + KeySize];
            Array.Copy(salt, 0, bytes, 0, SaltSize);
            Array.Copy(key, 0, bytes, SaltSize, KeySize);

            return Convert.ToBase64String(bytes);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            byte[] bytes = Convert.FromBase64String(hashedPassword);
            byte[] salt = new byte[SaltSize];
            Array.Copy(bytes, 0, salt, 0, SaltSize);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] key = pbkdf2.GetBytes(KeySize);

            for (int i = 0; i < KeySize; i++)
            {
                if (key[i] != bytes[i + SaltSize])
                {
                    return false;
                }
            }

            return true;
        }

    }
}
