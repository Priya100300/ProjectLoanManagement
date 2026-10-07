using System.Diagnostics;

namespace ProjectLoanManagement.API.Middleware;

/// <summary>
/// Logs one line per request: method, path, user, status and time taken.
/// Bodies, headers and query strings are NOT logged, so tokens, passwords, PAN, Aadhaar
/// and account numbers never end up in log files.
/// </summary>
public class RequestResponseMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseMiddleware> _logger;

    public RequestResponseMiddleware(RequestDelegate next, ILogger<RequestResponseMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            string user = context.User?.Identity?.IsAuthenticated == true ? context.User.Identity.Name : "anonymous";
            _logger.LogInformation("HTTP {Method} {Path} by {User} -> {StatusCode} in {ElapsedMs} ms",
                context.Request.Method, context.Request.Path.Value, user, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
        }
    }
}
