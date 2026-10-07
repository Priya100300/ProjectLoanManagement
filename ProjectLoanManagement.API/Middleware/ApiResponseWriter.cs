using System.Text.Json;
using ProjectLoanManagement.Models;

namespace ProjectLoanManagement.API.Middleware;

/// <summary>Writes a ResultSet as JSON from places that run outside a controller (middleware, JWT events).</summary>
public static class ApiResponseWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public static Task WriteAsync(HttpContext context, int statusCode, ResultSet result)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync(JsonSerializer.Serialize(result, JsonOptions));
    }
}
