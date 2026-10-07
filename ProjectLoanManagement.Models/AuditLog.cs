namespace ProjectLoanManagement.Models;

public class AuditLog
{
    public long AuditId { get; set; }
    public int? UserId { get; set; }
    public string UserName { get; set; }
    public string Action { get; set; }
    public string Module { get; set; }
    public string ReferenceId { get; set; }
    public string Description { get; set; }
    public string IPAddress { get; set; }
    public DateTime CreatedDate { get; set; }
}
