using System.ComponentModel.DataAnnotations;

namespace ProjectLoanManagement.Models.Requests;

public class RecordPaymentRequest
{
    [Required, Range(1, int.MaxValue)]
    public int? LoanId { get; set; }

    /// <summary>Optional. When empty the payment is applied to the oldest unpaid EMI.</summary>
    [Range(1, int.MaxValue)]
    public int? EMIId { get; set; }

    [Required, StringLength(50, MinimumLength = 3)]
    public string PaymentReference { get; set; }

    [Required, Range(typeof(decimal), "0.01", "1000000000")]
    public decimal? Amount { get; set; }

    [Required, RegularExpression("^(Cash|Bank Transfer|UPI|Cheque|Other)$",
        ErrorMessage = "PaymentMode must be Cash, Bank Transfer, UPI, Cheque or Other.")]
    public string PaymentMode { get; set; }

    [StringLength(100)]
    public string ExternalTransactionId { get; set; }

    [StringLength(500)]
    public string Remarks { get; set; }
}

public class UpdateBusinessRuleRequest
{
    [Required, Range(typeof(decimal), "0", "1000000000")]
    public decimal? RuleValue { get; set; }
}
