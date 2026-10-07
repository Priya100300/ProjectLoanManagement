using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectLoanManagement.API.Requests;
using ProjectLoanManagement.API.Services;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;

namespace ProjectLoanManagement.API.Controllers;

/// <summary>Loan documents (bank statements, salary slips...). Files are stored privately and streamed through the API.</summary>
[Route("api/document")]
[Authorize]
public class DocumentController : BaseApiController
{
    private const string StorageCategory = "loan-documents";

    private readonly IDocumentRepository _documentRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly IFileStorageService _storage;
    private readonly FileValidator _fileValidator;

    public DocumentController(IDocumentRepository documentRepository, ILoanRepository loanRepository, IAuditRepository auditRepository,
                              IFileStorageService storage, FileValidator fileValidator)
    {
        _documentRepository = documentRepository;
        _loanRepository = loanRepository;
        _auditRepository = auditRepository;
        _storage = storage;
        _fileValidator = fileValidator;
    }

    /// <summary>appliesTo: KYC, LOAN or empty for all.</summary>
    [HttpGet("types")]
    public IActionResult GetDocumentTypes([FromQuery] string appliesTo)
    {
        return FromResult(_documentRepository.GetDocumentTypes(appliesTo));
    }

    [HttpPost("loan/{loanId:int}")]
    [Authorize(Roles = Roles.LoanDesk)]
    [Consumes("multipart/form-data")]
    public IActionResult Upload(int loanId, [FromForm] UploadLoanDocumentRequest request)
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

        ResultSet result = _documentRepository.UploadLoanDocument(loanId, request.DocumentTypeId.Value, stored, CurrentUserId, ClientIp);
        if (!result.Status)
        {
            _storage.Delete(stored.StorageKey);   // do not leave orphan files behind
        }

        return FromResult(result, StatusCodes.Status201Created);
    }

    [HttpGet("loan/{loanId:int}")]
    [Authorize(Roles = Roles.StaffAndCustomer)]
    public IActionResult GetLoanDocuments(int loanId, [FromQuery] bool includeInactive = false)
    {
        IActionResult denied = DenyIfNotOwnLoan(_loanRepository, loanId);
        return denied ?? FromResult(_documentRepository.GetLoanDocuments(loanId, includeInactive));
    }

    /// <summary>Streams the file. Every download is written to the audit log.</summary>
    [HttpGet("{documentId:int}/download")]
    [Authorize(Roles = Roles.Staff)]
    [Produces("application/octet-stream", "application/json")]
    public IActionResult Download(int documentId)
    {
        ResultSet result = _documentRepository.GetLoanDocumentById(documentId);
        if (!result.Status)
        {
            return FromResult(result);
        }

        LoanDocument document = (LoanDocument)result.Data;
        Stream stream;
        try
        {
            stream = _storage.OpenRead(document.FilePath);
        }
        catch (FileNotFoundException)
        {
            return FromResult(ResultSet.Failure("The stored file could not be found.", "DOC_404"));
        }

        _auditRepository.WriteLog(CurrentUserId, "DOCUMENT_DOWNLOAD", "DOCUMENT", documentId.ToString(), null, ClientIp);
        return File(stream, document.ContentType, document.FileName);
    }

    /// <summary>Verify or reject a document. The uploader cannot verify their own upload.</summary>
    [HttpPost("{documentId:int}/verify")]
    [Authorize(Roles = Roles.KycReviewers)]
    public IActionResult Verify(int documentId, [FromBody] VerifyDocumentRequest request)
    {
        return FromResult(_documentRepository.VerifyLoanDocument(documentId, request, CurrentUserId, ClientIp));
    }
}
