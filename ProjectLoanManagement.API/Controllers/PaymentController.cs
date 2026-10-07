using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;

namespace ProjectLoanManagement.API.Controllers;

[Route("api/payment")]
[Authorize]
public class PaymentController : BaseApiController
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILoanRepository _loanRepository;

    public PaymentController(IPaymentRepository paymentRepository, ILoanRepository loanRepository)
    {
        _paymentRepository = paymentRepository;
        _loanRepository = loanRepository;
    }

    /// <summary>Records an EMI payment. PaymentReference must be unique (protects against double posting).</summary>
    [HttpPost]
    [Authorize(Roles = Roles.Collections)]
    public IActionResult RecordPayment([FromBody] RecordPaymentRequest request)
    {
        return FromResult(_paymentRepository.RecordPayment(request, CurrentUserId, ClientIp), StatusCodes.Status201Created);
    }

    [HttpGet("loan/{loanId:int}")]
    [Authorize(Roles = Roles.StaffAndCustomer)]
    public IActionResult GetPaymentHistory(int loanId)
    {
        IActionResult denied = DenyIfNotOwnLoan(_loanRepository, loanId);
        return denied ?? FromResult(_paymentRepository.GetPaymentHistory(loanId));
    }
}
