using System.Text.Json.Serialization;

namespace ProjectLoanManagement.Models;

public class User
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; }
    public int FailedLoginCount { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public DateTime CreatedDate { get; set; }

    /// <summary>Never serialised to the client.</summary>
    [JsonIgnore]
    public string PasswordHash { get; set; }
}
