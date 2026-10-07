using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ProjectLoanManagement.Models;

namespace ProjectLoanManagement.Repository.Data;

/// <summary>
/// Turns every database outcome into a ResultSet, so no repository method throws:
///   THROW 50001 'CODE|Message' from a procedure -> business error (safe to show the user)
///   unique key violation (2627/2601)           -> MODULE_409
///   foreign key / check violation (547)        -> MODULE_400
///   anything else                              -> logged, generic MODULE_500 message
/// </summary>
public abstract class RepositoryBase
{
    protected readonly DbHelper Db;
    private readonly ILogger _logger;

    protected RepositoryBase(DbHelper db, ILogger logger)
    {
        Db = db;
        _logger = logger;
    }

    protected ResultSet Execute(string module, string failureMessage, Func<ResultSet> operation)
    {
        try
        {
            return operation();
        }
        catch (SqlException ex) when (ex.Number >= 50000)
        {
            return FromBusinessError(ex.Message, module);
        }
        catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
        {
            _logger.LogWarning("Unique constraint violation in {Module} (SQL {Number})", module, ex.Number);
            return ResultSet.Failure("A record with the same unique value already exists.", module + "_409");
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            _logger.LogWarning("Constraint violation in {Module} (SQL {Number})", module, ex.Number);
            return ResultSet.Failure("The request breaks a data rule or refers to a record that does not exist.", module + "_400");
        }
        catch (Exception ex)
        {
            // Full details go to the log only; the client gets a generic message.
            _logger.LogError(ex, "Unexpected error in {Module}", module);
            return ResultSet.Failure(failureMessage, module + "_500");
        }
    }

    private static ResultSet FromBusinessError(string rawMessage, string module)
    {
        int separator = rawMessage.IndexOf('|');
        if (separator > 0)
        {
            string code = rawMessage.Substring(0, separator).Trim();
            string message = rawMessage.Substring(separator + 1).Trim();
            return ResultSet.Failure(message, code);
        }

        return ResultSet.Failure(rawMessage, module + "_400");
    }
}
