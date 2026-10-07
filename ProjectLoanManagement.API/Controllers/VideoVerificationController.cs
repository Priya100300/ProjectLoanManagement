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
/// Video KYC. The live call itself runs on a WebRTC provider; this API schedules the session,
/// tracks its status and stores the recording privately.
/// Flow: schedule -> start -> upload recording -> complete (Verified / Rejected).
/// </summary>
[Route("api/video-verification")]
[Authorize(Roles = Roles.Staff)]
public class VideoVerificationController : BaseApiController
{
    private const string StorageCategory = "video-recordings";
    private const long MaxRecordingRequestBytes = 524_288_000; // 500 MB, keep in sync with FileStorage:MaxVideoSizeMB

    private readonly IVideoVerificationRepository _videoRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly IFileStorageService _storage;
    private readonly FileValidator _fileValidator;

    public VideoVerificationController(IVideoVerificationRepository videoRepository, IAuditRepository auditRepository,
                                       IFileStorageService storage, FileValidator fileValidator)
    {
        _videoRepository = videoRepository;
        _auditRepository = auditRepository;
        _storage = storage;
        _fileValidator = fileValidator;
    }

    [HttpPost]
    [Authorize(Roles = Roles.KycTeam)]
    public IActionResult Schedule([FromBody] CreateVideoVerificationRequest request)
    {
        return FromResult(_videoRepository.CreateVideoVerification(request, CurrentUserId, ClientIp), StatusCodes.Status201Created);
    }

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        return FromResult(_videoRepository.GetVideoVerification(id));
    }

    /// <summary>Only the assigned officer can start the session.</summary>
    [HttpPost("{id:int}/start")]
    [Authorize(Roles = Roles.KycTeam)]
    public IActionResult Start(int id, [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] StartVideoRequest request)
    {
        return FromResult(_videoRepository.StartVideoVerification(id, request?.ExternalSessionId, CurrentUserId, ClientIp));
    }

    /// <summary>Upload the recording (mp4 / webm). A SHA-256 hash is stored so later tampering can be detected.</summary>
    [HttpPost("{id:int}/recording")]
    [Authorize(Roles = Roles.KycTeam)]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaxRecordingRequestBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxRecordingRequestBytes)]
    public IActionResult UploadRecording(int id, [FromForm] UploadRecordingRequest request)
    {
        ResultSet validation = _fileValidator.ValidateVideo(request.File);
        if (!validation.Status)
        {
            return FromResult(validation);
        }

        StoredFile stored;
        using (Stream content = request.File.OpenReadStream())
        {
            stored = _storage.Save(content, request.File.FileName, FileValidator.ContentTypeFor(request.File.FileName), StorageCategory);
        }

        ResultSet result = _videoRepository.SaveRecording(id, stored, request.DurationSeconds.Value,
            request.RecordingStartTime, request.RecordingEndTime, CurrentUserId, ClientIp);
        if (!result.Status)
        {
            _storage.Delete(stored.StorageKey);
        }

        return FromResult(result, StatusCodes.Status201Created);
    }

    /// <summary>Outcome: Completed (decide later), Verified (needs a recording) or Rejected (needs remarks).</summary>
    [HttpPost("{id:int}/complete")]
    [Authorize(Roles = Roles.KycTeam)]
    public IActionResult Complete(int id, [FromBody] CompleteVideoRequest request)
    {
        return FromResult(_videoRepository.CompleteVideoVerification(id, request, CurrentUserId, ClientIp));
    }

    /// <summary>Streams a recording to an authorised user. Every access is audited.</summary>
    [HttpGet("recording/{recordingId:int}/download")]
    [Authorize(Roles = "Admin,Verifier,Manager")]
    [Produces("application/octet-stream", "application/json")]
    public IActionResult DownloadRecording(int recordingId)
    {
        ResultSet result = _videoRepository.GetRecordingById(recordingId);
        if (!result.Status)
        {
            return FromResult(result);
        }

        VideoRecording recording = (VideoRecording)result.Data;
        Stream stream;
        try
        {
            stream = _storage.OpenRead(recording.FilePath);
        }
        catch (FileNotFoundException)
        {
            return FromResult(ResultSet.Failure("The stored recording could not be found.", "VIDEO_404"));
        }

        _auditRepository.WriteLog(CurrentUserId, "RECORDING_ACCESS", "VIDEO", recordingId.ToString(), null, ClientIp);
        return File(stream, recording.ContentType, recording.FileName, enableRangeProcessing: true);
    }
}
