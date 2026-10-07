using System.ComponentModel.DataAnnotations;

namespace ProjectLoanManagement.Models;

public class LoginRequest
{
    [Required, StringLength(50)]
    public string UserName { get; set; }

    [Required, StringLength(128)]
    public string Password { get; set; }
}
