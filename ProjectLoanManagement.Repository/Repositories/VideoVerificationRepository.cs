using Microsoft.Extensions.Logging;
using ProjectLoanManagement.Interfaces;
using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;
using ProjectLoanManagement.Repository.Data;

namespace ProjectLoanManagement.Repository.Repositories;

public class VideoVerificationRepository : RepositoryBase, IVideoVerificationRepository
{
    public VideoVerificationRepository(DbHelper db, ILogger<VideoVerificationRepository> logger) : base(db, logger)
    {
    }

    public ResultSet CreateVideoVerification(CreateVideoVerificationRequest request, int createdBy, string ipAddress)
    {
        return Execute("VIDEO", "Unable to schedule the video verification.", () =>
        {
            VideoVerification video = Db.Query("dbo.sp_CreateVideoVerification", p =>
            {
                p.AddInt("@KYCVerificationId", request.KYCVerificationId);
                p.AddInt("@OfficerId", request.OfficerId);
                p.AddDateTime2("@ScheduledDate", ToUtc(request.ScheduledDate));
                p.AddNVarChar("@Remarks", request.Remarks, 500);
                p.AddInt("@CreatedBy", createdBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            }, r => r.ReadSingle(EntityMapper.MapVideo));

            return ResultSet.Success(video, "Video verification scheduled");
        });
    }

    public ResultSet GetVideoVerification(int verificationId)
    {
        return Execute("VIDEO", "Unable to fetch the video verification.", () =>
        {
            VideoVerification video = Db.Query("dbo.sp_GetVideoVerification", p => p.AddInt("@VerificationId", verificationId), r =>
            {
                VideoVerification result = r.ReadSingle(EntityMapper.MapVideo);
                r.NextResult();
                result.Recordings = r.ReadList(x => EntityMapper.MapRecording(x, false));
                return result;
            });

            return ResultSet.Success(video, "Video verification fetched successfully");
        });
    }

    public ResultSet StartVideoVerification(int verificationId, string externalSessionId, int userId, string ipAddress)
    {
        return Execute("VIDEO", "Unable to start the video verification.", () =>
        {
            VideoVerification video = Db.Query("dbo.sp_StartVideoVerification", p =>
            {
                p.AddInt("@VerificationId", verificationId);
                p.AddNVarChar("@ExternalSessionId", externalSessionId, 200);
                p.AddInt("@UserId", userId);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            }, r => r.ReadSingle(EntityMapper.MapVideo));

            return ResultSet.Success(video, "Video verification started");
        });
    }

    public ResultSet SaveRecording(int verificationId, StoredFile file, int durationSeconds, DateTime? recordingStartTime,
                                   DateTime? recordingEndTime, int uploadedBy, string ipAddress)
    {
        return Execute("VIDEO", "Unable to save the recording.", () =>
        {
            VideoRecording recording = Db.Query("dbo.sp_SaveVideoRecording", p =>
            {
                p.AddInt("@VerificationId", verificationId);
                p.AddNVarChar("@FileName", file.FileName, 255);
                p.AddNVarChar("@FilePath", file.StorageKey, 500);
                p.AddVarChar("@ContentType", file.ContentType, 100);
                p.AddBigInt("@FileSize", file.SizeBytes);
                p.AddInt("@DurationSeconds", durationSeconds);
                p.AddChar("@FileHashSha256", file.Sha256, 64);
                p.AddDateTime2("@RecordingStartTime", ToUtc(recordingStartTime));
                p.AddDateTime2("@RecordingEndTime", ToUtc(recordingEndTime));
                p.AddInt("@UploadedBy", uploadedBy);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            }, r => r.ReadSingle(x => EntityMapper.MapRecording(x, false)));

            return ResultSet.Success(recording, "Recording saved successfully");
        });
    }

    public ResultSet CompleteVideoVerification(int verificationId, CompleteVideoRequest request, int userId, string ipAddress)
    {
        return Execute("VIDEO", "Unable to complete the video verification.", () =>
        {
            VideoVerification video = Db.Query("dbo.sp_CompleteVideoVerification", p =>
            {
                p.AddInt("@VerificationId", verificationId);
                p.AddNVarChar("@Outcome", request.Outcome, 20);
                p.AddNVarChar("@Remarks", request.Remarks, 500);
                p.AddInt("@UserId", userId);
                p.AddVarChar("@IPAddress", ipAddress, 45);
            }, r => r.ReadSingle(EntityMapper.MapVideo));

            return ResultSet.Success(video, "Video verification marked " + request.Outcome);
        });
    }

    public ResultSet GetRecordingById(int recordingId)
    {
        return Execute("VIDEO", "Unable to fetch the recording.", () =>
        {
            VideoRecording recording = Db.Query("dbo.sp_GetVideoRecordingById", p => p.AddInt("@RecordingId", recordingId),
                r => r.ReadSingle(x => EntityMapper.MapRecording(x, true)));
            return ResultSet.Success(recording, "Recording fetched successfully");
        });
    }

    /// <summary>The database stores UTC. Values without a zone are assumed to be UTC already.</summary>
    private static DateTime? ToUtc(DateTime? value)
    {
        if (!value.HasValue)
        {
            return null;
        }

        return value.Value.Kind == DateTimeKind.Local ? value.Value.ToUniversalTime() : value.Value;
    }
}
