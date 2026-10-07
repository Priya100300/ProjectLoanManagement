namespace ProjectLoanManagement.Models;

/// <summary>
/// The single response envelope returned by every repository method and every API endpoint.
/// </summary>
public class ResultSet
{
    public bool Status { get; set; }

    public string Message { get; set; }

    public object Data { get; set; }

    public string ErrorCode { get; set; }

    public static ResultSet Success(object data, string message)
    {
        return new ResultSet { Status = true, Message = message, Data = data, ErrorCode = null };
    }

    public static ResultSet Failure(string message, string errorCode, object data = null)
    {
        return new ResultSet { Status = false, Message = message, Data = data, ErrorCode = errorCode };
    }
}
