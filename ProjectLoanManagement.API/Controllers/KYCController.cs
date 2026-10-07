using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProjectLoanManagement.API.Requests;
using ProjectLoanManagement.API.Services;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;

namespace ProjectLoanManagement.API.Controllers;

/// <summary>
/// KYC workflow: create case -> upload documents -> verify documents -> tick checklist ->
/// (video KYC if Video/Full) -> Verified / Rejected / Rework.
/// </summary>
[Route("api/kyc")]
[Authorize(Roles = Roles.Staff)]
public class KYCController : BaseApiController
{
    private const string StorageCategory = "kyc-documents";

    private readonly IKYCRepository _kycRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly IFileStorageService _storage;
    private readonly FileValidator _fileValidator;

    public KYCController(IKYCRepository kycRepository, IAuditRepository auditRepository, IFileStorageService storage, FileValidator fileValidator)
    {
        _kycRepository = kycRepository;
        _auditRepository = auditRepository;
        _storage = storage;
        _fileValidator = fileValidator;
    }

    /// <summary>Starts KYC for a loan and moves the loan to Under Review.</summary>
    [HttpPost]
    [Authorize(Roles = Roles.KycTeam)]
    public IActionResult CreateKYC([FromBody] CreateKYCRequest request)
    {
        return FromResult(_kycRepository.CreateKYC(request, CurrentUserId, ClientIp), StatusCodes.Status201Created);
    }

    /// <summary>Full case: header, documents, checklist and video sessions.</summary>
    [HttpGet("{id:int}")]
    public IActionResult GetKYC(int id)
    {
        return FromResult(_kycRepository.GetKYC(id));
    }

    [HttpGet("loan/{loanId:int}")]
    public IActionResult GetKYCByLoan(int loanId)
    {
        return FromResult(_kycRepository.GetKYCByLoan(loanId));
    }

    [HttpPost("{id:int}/documents")]
    [Authorize(Roles = Roles.KycTeam)]
    [Consumes("multipart/form-data")]
    public IActionResult UploadDocument(int id, [FromForm] UploadKYCDocumentRequest request)
    {
        ResultSet validation = _fileValidator.ValidateDocument(request.File);
        if (!validation.Status)
        {
            return FromResult(validation);
        }

        StoredFile stored;
        using (Stream content = request.File.OpenReadStream())
        {
            stored = _storage.Save(content, request.File.FileName, FileValidator.ContentTypeFor(request.File.FileName), StorageCategory);
        }

        string maskedNumber = SensitiveDataProtector.MaskTail(request.DocumentNumber);
        ResultSet result = _kycRepository.AddKYCDocument(id, request.DocumentTypeId.Value, maskedNumber, stored, CurrentUserId, ClientIp);
        if (!result.Status)
        {
            _storage.Delete(stored.StorageKey);
        }

        return FromResult(result, StatusCodes.Status201Created);
    }

    [HttpGet("documents/{kycDocumentId:int}/download")]
    [Produces("application/octet-stream", "application/json")]
    public IActionResult DownloadDocument(int kycDocumentId)
    {
        ResultSet result = _kycRepository.GetKYCDocumentById(kycDocumentId);
        if (!result.Status)
        {
            return FromResult(result);
        }

        KYCDocument document = (KYCDocument)result.Data;
        Stream stream;
        try
        {
            stream = _storage.OpenRead(document.DocumentFilePath);
        }
        catch (FileNotFoundException)
        {
            return FromResult(ResultSet.Failure("The stored file could not be found.", "KYC_404"));
        }

        _auditRepository.WriteLog(CurrentUserId, "KYC_DOCUMENT_DOWNLOAD", "KYC", kycDocumentId.ToString(), null, ClientIp);
        return File(stream, document.ContentType, document.FileName);
    }

    [HttpPost("documents/{kycDocumentId:int}/verify")]
    [Authorize(Roles = Roles.KycReviewers)]
    public IActionResult VerifyDocument(int kycDocumentId, [FromBody] VerifyDocumentRequest request)
    {
        return FromResult(_kycRepository.VerifyKYCDocument(kycDocumentId, request, CurrentUserId, ClientIp));
    }

    [HttpGet("{id:int}/checklist")]
    public IActionResult GetChecklist(int id)
    {
        return FromResult(_kycRepository.GetChecklist(id));
    }

    [HttpPut("{id:int}/checklist/{checklistId:int}")]
    [Authorize(Roles = Roles.KycReviewers)]
    public IActionResult UpdateChecklist(int id, int checklistId, [FromBody] UpdateChecklistRequest request)
    {
        return FromResult(_kycRepository.UpdateChecklist(id, checklistId, request, CurrentUserId, ClientIp));
    }

    /// <summary>Needs every mandatory document verified, the checklist complete and (Video/Full) a verified video.</summary>
    [HttpPost("{id:int}/verify")]
    [Authorize(Roles = Roles.KycReviewers)]
    public IActionResult Verify(int id, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] KYCDecisionRequest request)
    {
        return FromResult(_kycRepository.UpdateKYCStatus(id, "Verified", request?.Reason, CurrentUserId, ClientIp));
    }

    [HttpPost("{id:int}/reject")]
    [Authorize(Roles = Roles.KycReviewers)]
    public IActionResult Reject(int id, [FromBody] KYCDecisionRequest request)
    {
        return FromResult(_kycRepository.UpdateKYCStatus(id, "Rejected", request.Reason, CurrentUserId, ClientIp));
    }

    /// <summary>Send back to the customer for corrections (e.g. a blurred document).</summary>
    [HttpPost("{id:int}/rework")]
    [Authorize(Roles = Roles.KycReviewers)]
    public IActionResult Rework(int id, [FromBody] KYCDecisionRequest request)
    {
        return FromResult(_kycRepository.UpdateKYCStatus(id, "Rework", request.Reason, CurrentUserId, ClientIp));
    }
}
