namespace ProjectLoanManagement.Models;

/// <summary>Customer as returned by the API. PAN and Aadhaar are always masked.</summary>
public class Customer
{
    public int CustomerId { get; set; }
    public string CustomerCode { get; set; }
    public int? UserId { get; set; }
    public string FullName { get; set; }
    public string MobileNo { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PinCode { get; set; }
    public string PANMasked { get; set; }
    public string AadhaarNoMasked { get; set; }
    public DateTime DateOfBirth { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
