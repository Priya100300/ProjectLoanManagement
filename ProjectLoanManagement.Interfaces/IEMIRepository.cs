using ProjectLoanManagement.Models;

namespace ProjectLoanManagement.Interfaces;

public interface IEMIRepository
{
    ResultSet GenerateSchedule(int loanId, int userId, string ipAddress);

    ResultSet GetSchedule(int loanId);

    ResultSet MarkOverdue(int userId, string ipAddress);
}
