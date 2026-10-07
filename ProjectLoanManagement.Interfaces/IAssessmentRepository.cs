using ProjectLoanManagement.Models;
using ProjectLoanManagement.Models.Requests;

namespace ProjectLoanManagement.Interfaces;

public interface IAssessmentRepository
{
    ResultSet CreateAssessment(CreateAssessmentRequest request, int assessedBy, string ipAddress);

    ResultSet GetAssessments(int loanId);
}
