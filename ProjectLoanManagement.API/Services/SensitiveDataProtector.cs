using System.Security.Cryptography;
using System.Text;

namespace ProjectLoanManagement.API.Services;

/// <summary>
/// Masks and hashes sensitive identifiers before they reach the repository layer.
/// Aadhaar is never stored in full: only "XXXX-XXXX-1234" plus an HMAC-SHA256 hash
/// (the hash lets the database detect duplicates without knowing the number).
/// </summary>
public class SensitiveDataProtector
{
    private readonly byte[] _aadhaarKey;

    public SensitiveDataProtector(IConfiguration configuration)
    {
        string key = configuration["Security:AadhaarHashKey"];
        if (string.IsNullOrWhiteSpace(key) || key.Length < 32)
        {
            throw new InvalidOperationException("Security:AadhaarHashKey must be configured with at least 32 characters.");
        }

        _aadhaarKey = Encoding.UTF8.GetBytes(key);
    }

    public byte[] HashAadhaar(string aadhaarNumber)
    {
        if (string.IsNullOrWhiteSpace(aadhaarNumber))
        {
            return null;
        }

        using (HMACSHA256 hmac = new HMACSHA256(_aadhaarKey))
        {
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(aadhaarNumber.Trim()));
        }
    }

    public static string MaskAadhaar(string aadhaarNumber)
    {
        if (string.IsNullOrWhiteSpace(aadhaarNumber) || aadhaarNumber.Length < 4)
        {
            return null;
        }

        return "XXXX-XXXX-" + aadhaarNumber.Trim().Substring(aadhaarNumber.Trim().Length - 4);
    }

    /// <summary>Keeps only the last <paramref name="visible"/> characters: 1234567890 -> XXXXXX7890.</summary>
    public static string MaskTail(string value, int visible = 4)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        string trimmed = value.Trim();
        if (trimmed.Length <= visible)
        {
            return new string('X', trimmed.Length);
        }

        return new string('X', trimmed.Length - visible) + trimmed.Substring(trimmed.Length - visible);
    }
}
