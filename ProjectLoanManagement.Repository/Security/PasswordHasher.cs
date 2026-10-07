using System.Security.Cryptography;

namespace ProjectLoanManagement.Repository.Security;

/// <summary>
/// PBKDF2-SHA256 password hashing (built into .NET, no extra package).
/// Stored format: PBKDF2-SHA256$iterations$saltBase64$hashBase64
/// </summary>
public static class PasswordHasher
{
    private const string Prefix = "PBKDF2-SHA256";
    private const int Iterations = 100000;
    private const int SaltSize = 16;
    private const int KeySize = 32;

    /// <summary>Checked when the user does not exist, so response time does not reveal valid usernames.</summary>
    public static readonly string DummyHash = Hash("dummy-password-for-timing");

    public static string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
        return string.Join("$", Prefix, Iterations, Convert.ToBase64String(salt), Convert.ToBase64String(key));
    }

    public static bool Verify(string password, string storedHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
        {
            return false;
        }

        string[] parts = storedHash.Split('$');
        if (parts.Length != 4 || parts[0] != Prefix || !int.TryParse(parts[1], out int iterations))
        {
            return false;
        }

        try
        {
            byte[] salt = Convert.FromBase64String(parts[2]);
            byte[] expected = Convert.FromBase64String(parts[3]);
            byte[] actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
