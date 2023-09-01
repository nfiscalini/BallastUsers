using System.Text;
using XSystem.Security.Cryptography;

namespace Api.Users.Services
{
    public class PasswordService
    {
        public static string GetHashedPassword(string password)
        {
            byte[] hash = Encoding.ASCII.GetBytes(password);
            var sha1 = new SHA1CryptoServiceProvider();
            byte[] sha1hash = sha1.ComputeHash(hash);
            string hashedPassword = Convert.ToBase64String(sha1hash);

            return hashedPassword;
        }
    }
}
