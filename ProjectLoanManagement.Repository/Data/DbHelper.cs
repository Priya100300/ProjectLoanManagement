using System.Data;
using Microsoft.Data.SqlClient;

namespace ProjectLoanManagement.Repository.Data;

/// <summary>
/// Thin synchronous ADO.NET wrapper. Every call opens a connection, runs ONE stored procedure
/// and disposes everything with "using" (connection pooling makes this cheap).
/// Parameters are always SqlParameters, so there is no string-built SQL anywhere.
/// </summary>
public sealed class DbHelper
{
    private const int CommandTimeoutSeconds = 30;
    private readonly string _connectionString;

    public DbHelper(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string 'DefaultConnection' is missing.", nameof(connectionString));
        }

        _connectionString = connectionString;
    }

    /// <summary>Runs a procedure and hands the reader to <paramref name="map"/> (supports multiple result sets).</summary>
    public T Query<T>(string procedureName, Action<SqlParameterCollection> addParameters, Func<SqlDataReader, T> map)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = CreateCommand(connection, procedureName, addParameters))
        {
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                return map(reader);
            }
        }
    }

    /// <summary>Runs a procedure that returns no rows. Returns the rows-affected count.</summary>
    public int NonQuery(string procedureName, Action<SqlParameterCollection> addParameters)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = CreateCommand(connection, procedureName, addParameters))
        {
            connection.Open();
            return command.ExecuteNonQuery();
        }
    }

    /// <summary>Runs a procedure and returns the first column of the first row.</summary>
    public object Scalar(string procedureName, Action<SqlParameterCollection> addParameters)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = CreateCommand(connection, procedureName, addParameters))
        {
            connection.Open();
            object value = command.ExecuteScalar();
            return value == DBNull.Value ? null : value;
        }
    }

    private static SqlCommand CreateCommand(SqlConnection connection, string procedureName, Action<SqlParameterCollection> addParameters)
    {
        SqlCommand command = new SqlCommand(procedureName, connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = CommandTimeoutSeconds
        };

        addParameters?.Invoke(command.Parameters);
        return command;
    }
}
