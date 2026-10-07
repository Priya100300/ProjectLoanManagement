namespace ProjectLoanManagement.Models;

/// <summary>
/// Role names (must match the CHECK constraint on Users.Role) and the role groups used in [Authorize].
/// </summary>
public static class Roles
{
    public const string Admin = "Admin";
    public const string LoanOfficer = "LoanOfficer";
    public const string Manager = "Manager";
    public const string Verifier = "Verifier";
    public const string Customer = "Customer";

    public const string Staff = "Admin,LoanOfficer,Manager,Verifier";
    public const string StaffAndCustomer = "Admin,LoanOfficer,Manager,Verifier,Customer";
    public const string LoanDesk = "Admin,LoanOfficer";
    public const string Approvers = "Admin,Manager";
    public const string KycTeam = "Admin,LoanOfficer,Verifier";
    public const string KycReviewers = "Admin,Verifier";
    public const string Collections = "Admin,LoanOfficer";
    public const string RoleNamePattern = "^(Admin|LoanOfficer|Manager|Verifier|Customer)$";
}
