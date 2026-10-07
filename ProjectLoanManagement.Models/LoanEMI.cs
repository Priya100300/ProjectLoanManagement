namespace ProjectLoanManagement.Models;

public class LoanEMI
{
    public int EMIId { get; set; }
    public int LoanId { get; set; }
    public int EMINumber { get; set; }
    public DateTime DueDate { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal EMIAmount { get; set; }
    public decimal PrincipalAmount { get; set; }
    public decimal InterestAmount { get; set; }
    public decimal ClosingBalance { get; set; }
    public decimal PaidAmount { get; set; }
    public string EMIStatus { get; set; }
    public DateTime? PaidDate { get; set; }
}

public class EMISchedule
{
    public int LoanId { get; set; }
    public string LoanNumber { get; set; }
    public string LoanStatus { get; set; }
    public int TotalEMIs { get; set; }
    public decimal TotalPayable { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal Outstanding { get; set; }
    public DateTime? NextDueDate { get; set; }
    public List<LoanEMI> Installments { get; set; } = new List<LoanEMI>();
}
