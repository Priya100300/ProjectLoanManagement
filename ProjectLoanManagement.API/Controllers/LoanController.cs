using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProjectLoanManagement.API.Services;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;

namespace ProjectLoanManagement.API.Controllers;

/// <summary>
/// Loan lifecycle: Applied -> Under Review -> Approved -> Disbursed -> Active -> Closed (or Rejected).
/// The controller only receives the request and passes it on; every business rule is enforced
/// in the stored procedures, so it cannot be skipped by calling the API in a different order.
/// </summary>
[Route("api/loan")]
[Authorize]
public class LoanController : BaseApiController
{
    private readonly ILoanRepository _loanRepository;

    public LoanController(ILoanRepository loanRepository)
    {
        _loanRepository = loanRepository;
    }

    [HttpPost]
    [Authorize(Roles = Roles.LoanDesk)]
    public IActionResult CreateLoan([FromBody] CreateLoanRequest request)
    {
        return FromResult(_loanRepository.CreateLoan(request, CurrentUserId, ClientIp), StatusCodes.Status201Created);
    }

    /// <summary>Filter by status (Applied, Under Review, Approved, Rejected, Disbursed, Active, Closed) and/or customer.</summary>
    [HttpGet]
    [Authorize(Roles = Roles.Staff)]
    public IActionResult GetLoans([FromQuery] string status, [FromQuery] int? customerId,
                                  [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        return FromResult(_loanRepository.GetLoans(status, customerId, pageNumber, pageSize));
    }

    /// <summary>Loans waiting for an action (Applied, Under Review, Approved).</summary>
    [HttpGet("pending")]
    [Authorize(Roles = Roles.Staff)]
    public IActionResult GetPendingLoans()
    {
        return FromResult(_loanRepository.GetPendingLoans());
    }

    /// <summary>The logged-in customer's own loans.</summary>
    [HttpGet("my")]
    [Authorize(Roles = Roles.Customer)]
    public IActionResult GetMyLoans([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        return FromResult(_loanRepository.GetMyLoans(CurrentUserId, pageNumber, pageSize));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = Roles.StaffAndCustomer)]
    public IActionResult GetLoan(int id)
    {
        IActionResult denied = DenyIfNotOwnLoan(_loanRepository, id);
        return denied ?? FromResult(_loanRepository.GetLoanById(id));
    }

    [HttpGet("{id:int}/status")]
    [Authorize(Roles = Roles.StaffAndCustomer)]
    public IActionResult GetLoanStatus(int id)
    {
        IActionResult denied = DenyIfNotOwnLoan(_loanRepository, id);
        return denied ?? FromResult(_loanRepository.GetLoanStatus(id));
    }

    /// <summary>Edit an application (only while it is still Applied).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.LoanDesk)]
    public IActionResult UpdateLoan(int id, [FromBody] UpdateLoanRequest request)
    {
        return FromResult(_loanRepository.UpdateLoan(id, request, CurrentUserId, ClientIp));
    }

    /// <summary>Requires: Under Review, verified KYC, verified mandatory documents, Recommended assessment, approver != creator.</summary>
    [HttpPost("{id:int}/approve")]
    [Authorize(Roles = Roles.Approvers)]
    public IActionResult ApproveLoan(int id, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] ApproveLoanRequest request)
    {
        return FromResult(_loanRepository.ApproveLoan(id, request, CurrentUserId, ClientIp));
    }

    [HttpPost("{id:int}/reject")]
    [Authorize(Roles = Roles.Approvers)]
    public IActionResult RejectLoan(int id, [FromBody] RejectLoanRequest request)
    {
        return FromResult(_loanRepository.RejectLoan(id, request.Reason, CurrentUserId, ClientIp));
    }

    /// <summary>Disburses an Approved loan, generates the EMI schedule and makes it Active - in one transaction.</summary>
    [HttpPost("{id:int}/disburse")]
    [Authorize(Roles = Roles.Approvers)]
    public IActionResult DisburseLoan(int id, [FromBody] DisburseLoanRequest request)
    {
        string maskedAccount = SensitiveDataProtector.MaskTail(request.BeneficiaryAccountNo);
        return FromResult(_loanRepository.DisburseLoan(id, request, maskedAccount, CurrentUserId, ClientIp));
    }

    /// <summary>Closes an Active loan once every EMI is fully paid.</summary>
    [HttpPost("{id:int}/close")]
    [Authorize(Roles = Roles.Approvers)]
    public IActionResult CloseLoan(int id)
    {
        return FromResult(_loanRepository.CloseLoan(id, CurrentUserId, ClientIp));
    }
}
