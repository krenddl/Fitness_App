using System.Security.Cryptography;
using System.Text;

namespace Fitness_Api.UniversalMethods;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }

    public static bool Verify(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}
