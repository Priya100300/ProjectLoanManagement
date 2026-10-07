using System.ComponentModel.DataAnnotations;

namespace ProjectLoanManagement.Models.Requests;

public class CreateCustomerRequest
{
    /// <summary>Optional: link to an existing user with the Customer role (portal login).</summary>
    public int? UserId { get; set; }

    [Required, StringLength(150)]
    public string FullName { get; set; }

    [Required, RegularExpression(@"^[6-9][0-9]{9}$", ErrorMessage = "MobileNo must be a 10-digit Indian mobile number.")]
    public string MobileNo { get; set; }

    [EmailAddress, StringLength(150)]
    public string Email { get; set; }

    [StringLength(300)]
    public string Address { get; set; }

    [StringLength(100)]
    public string City { get; set; }

    [StringLength(100)]
    public string State { get; set; }

    [RegularExpression(@"^[1-9][0-9]{5}$", ErrorMessage = "PinCode must be 6 digits.")]
    public string PinCode { get; set; }

    [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]$", ErrorMessage = "PAN format is ABCDE1234F (upper case).")]
    public string PAN { get; set; }

    /// <summary>Full 12-digit Aadhaar. Only the masked value and a keyed hash are stored.</summary>
    [RegularExpression(@"^[2-9][0-9]{11}$", ErrorMessage = "AadhaarNo must be 12 digits.")]
    public string AadhaarNo { get; set; }

    [Required]
    public DateTime? DateOfBirth { get; set; }
}

public class UpdateCustomerRequest
{
    [Required, StringLength(150)]
    public string FullName { get; set; }

    [Required, RegularExpression(@"^[6-9][0-9]{9}$", ErrorMessage = "MobileNo must be a 10-digit Indian mobile number.")]
    public string MobileNo { get; set; }

    [EmailAddress, StringLength(150)]
    public string Email { get; set; }

    [StringLength(300)]
    public string Address { get; set; }

    [StringLength(100)]
    public string City { get; set; }

    [StringLength(100)]
    public string State { get; set; }

    [RegularExpression(@"^[1-9][0-9]{5}$", ErrorMessage = "PinCode must be 6 digits.")]
    public string PinCode { get; set; }
}
