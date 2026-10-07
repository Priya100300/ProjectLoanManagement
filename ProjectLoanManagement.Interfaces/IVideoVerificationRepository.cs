using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;

namespace ProjectLoanManagement.Interfaces;

public interface IVideoVerificationRepository
{
    ResultSet CreateVideoVerification(CreateVideoVerificationRequest request, int createdBy, string ipAddress);

    ResultSet GetVideoVerification(int verificationId);

    ResultSet StartVideoVerification(int verificationId, string externalSessionId, int userId, string ipAddress);

    ResultSet SaveRecording(int verificationId, StoredFile file, int durationSeconds, DateTime? recordingStartTime,
                            DateTime? recordingEndTime, int uploadedBy, string ipAddress);

    ResultSet CompleteVideoVerification(int verificationId, CompleteVideoRequest request, int userId, string ipAddress);

    ResultSet GetRecordingById(int recordingId);
}
