using System.ComponentModel.DataAnnotations;

namespace ProjectLoanManagement.Models.Requests;

public class CreateUserRequest
{
    [Required, RegularExpression(@"^[A-Za-z0-9._-]{3,50}$", ErrorMessage = "UserName must be 3-50 letters, digits, dot, dash or underscore.")]
    public string UserName { get; set; }

    [Required, StringLength(128, MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,128}$",
        ErrorMessage = "Password needs at least 8 characters with upper case, lower case, a digit and a symbol.")]
    public string Password { get; set; }

    [Required, StringLength(150)]
    public string FullName { get; set; }

    [EmailAddress, StringLength(150)]
    public string Email { get; set; }

    [Required, RegularExpression(Roles.RoleNamePattern, ErrorMessage = "Role must be Admin, LoanOfficer, Manager, Verifier or Customer.")]
    public string Role { get; set; }
}

public class ChangePasswordRequest
{
    [Required, StringLength(128)]
    public string CurrentPassword { get; set; }

    [Required, StringLength(128, MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,128}$",
        ErrorMessage = "Password needs at least 8 characters with upper case, lower case, a digit and a symbol.")]
    public string NewPassword { get; set; }
}

public class UserStatusRequest
{
    [Required]
    public bool? IsActive { get; set; }
}
