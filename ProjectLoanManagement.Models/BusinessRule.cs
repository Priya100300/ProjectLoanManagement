namespace ProjectLoanManagement.Models;

public class BusinessRule
{
    public string RuleKey { get; set; }
    public decimal RuleValue { get; set; }
    public string Description { get; set; }
    public DateTime UpdatedDate { get; set; }
    public int? UpdatedBy { get; set; }
}
