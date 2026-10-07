namespace ProjectLoanManagement.Models;

public class LoanPayment
{
    public int PaymentId { get; set; }
    public int LoanId { get; set; }
    public int? EMIId { get; set; }
    public int? EMINumber { get; set; }
    public string EMIStatus { get; set; }
    public string PaymentReference { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMode { get; set; }
    public string ExternalTransactionId { get; set; }
    public string Remarks { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}
