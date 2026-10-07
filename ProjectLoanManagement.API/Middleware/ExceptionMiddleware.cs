using ProjectLoanManagement.Models;

namespace ProjectLoanManagement.API.Middleware;

/// <summary>
/// Safety net for anything unexpected. Logs the full exception and returns a clean ResultSet;
/// stack traces are never sent to the client (detail is added only in Development).
///
/// NOTE on Task/async: ASP.NET Core requires middleware to have the signature
/// "Task InvokeAsync(HttpContext)". This framework contract is the only place Task/await
/// appears in the project - controllers and repositories are fully synchronous.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();
            object detail = _environment.IsDevelopment() ? new { exception = ex.GetType().Name, ex.Message } : null;
            ResultSet result = ResultSet.Failure("Something went wrong. Please try again later.", "INTERNAL_ERROR", detail);
            await ApiResponseWriter.WriteAsync(context, StatusCodes.Status500InternalServerError, result);
        }
    }
}
