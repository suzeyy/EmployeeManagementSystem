using System.Text;
using System.Security.Cryptography;

namespace EmployeeManagementSystem.Helpers
{
    public class PasswordHasher
    {
        public static string Hash(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hashBytes = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
