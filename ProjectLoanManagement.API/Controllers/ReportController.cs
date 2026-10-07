using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;

namespace ProjectLoanManagement.API.Controllers;

/// <summary>Management reports. Dates are UTC (yyyy-MM-dd); the default period is the last 30 days.</summary>
[Route("api/report")]
[Authorize(Roles = Roles.Approvers)]
public class ReportController : BaseApiController
{
    private readonly IReportRepository _reportRepository;
    private readonly IAuditRepository _auditRepository;

    public ReportController(IReportRepository reportRepository, IAuditRepository auditRepository)
    {
        _reportRepository = reportRepository;
        _auditRepository = auditRepository;
    }

    [HttpGet("loans")]
    public IActionResult LoanReport([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        return FromResult(_reportRepository.GetLoanReport(fromDate, toDate));
    }

    [HttpGet("kyc")]
    public IActionResult KYCReport([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        return FromResult(_reportRepository.GetKYCReport(fromDate, toDate));
    }

    [HttpGet("payments")]
    public IActionResult PaymentReport([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        return FromResult(_reportRepository.GetPaymentReport(fromDate, toDate));
    }

    /// <summary>Filter by module (AUTH, LOAN, KYC, VIDEO, PAYMENT...), reference id or user.</summary>
    [HttpGet("audit-logs")]
    [Authorize(Roles = Roles.Admin)]
    public IActionResult AuditLogs([FromQuery] string module, [FromQuery] string referenceId, [FromQuery] int? userId,
                                   [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate,
                                   [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
    {
        return FromResult(_auditRepository.GetAuditLogs(module, referenceId, userId, fromDate, toDate, pageNumber, pageSize));
    }
}
